using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Models.Network
{
    /// <summary>
    /// Клієнтський клас, який містить в собі набір методів для виконання 
    /// асинхронних операцій (ConnectAsync, ReceiveAsync, SendAsync) 
    /// </summary>
    public class SocketClient
    {
        static Logger Logger = LogManager.GetCurrentClassLogger();

        public Socket Socket { get; private set; }
        public MessageHandler MessageHandler { get; private set; }

        /// <summary>Raising when session has some message received.</summary>
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<SocketErrorEventArgs> ConnectionLost;
        public event EventHandler<EventArgs> ConnectionSet;

        /// <summary>
        /// Instantiate new SocketClient object with default signal socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="messageHandler"></param>
        public SocketClient(Socket socket, MessageHandler messageHandler)
        {
            Socket = socket;
            MessageHandler = messageHandler;

            _receiveEventArgs.Completed += OnSocketAsyncOperationCompleted;
            _sendEventArgs.Completed += OnSocketAsyncOperationCompleted;
            MessageHandler.MessageReceived += RaiseMessageReceived;
        }

        public Task<Int32> WriteAsync(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags)
        {
            var tcs = new TaskCompletionSource<Int32>(Socket);
            Socket.BeginSend(buffer, offset, size, socketFlags, iar =>
            {
                var t = (TaskCompletionSource<Int32>)iar.AsyncState;
                var s = (Socket)t.Task.AsyncState;
                try { t.TrySetResult(s.EndSend(iar)); }
                catch (Exception ex) { t.TrySetException(ex); }
            }, tcs);
            return tcs.Task;
        }

        private void SendNextMessage()
        {
            Byte[] bytes;

            lock (_sendQueue)
            {
                if (_sendQueue.Count == 0) return;
                bytes = _sendQueue.Peek();
            }

            try
            {
                _sendEventArgs.SetBuffer(bytes, 0, bytes.Length);
                if (!Socket.SendAsync(_sendEventArgs)) ProcessSendCompleted();
            }
            catch (Exception ex)
            {
                Logger.Error($"SocketClient.SendNextMessage(): {ex.Message}");
                Disconnect();
                RaiseConnectionLost(SocketError.Success, ex);
            }
        }

        private void ReceiveNextMessage()
        {
            _receiveEventArgs.SetBuffer(0, _receiveEventArgs.Buffer.Length);

            if (Socket.ReceiveAsync(_receiveEventArgs)) 
                return;

            if (Socket.Connected)
            {
#warning це я закомітив, чомусь переповнення стеку в цьому методі(через рекурсію):
                //ProcessReceiveCompleted(_receiveEventArgs);
            }
            else
                Disconnect();
        }

        private void ProcessConnectCompleted(SocketAsyncEventArgs e)
        {
            if (Socket.Connected)
            {
                Attach();
                RaiseConnectionSet();
            }
            else
                Disconnect();
        }

        private void ProcessSendCompleted()
        {
            lock (_sendQueue)
            {
                if (_sendQueue.Count > 0)
                    _sendQueue.Dequeue();

                if (_sendQueue.Count == 0)
                    return;
            }

            SendNextMessage();
        }

        private void ProcessReceiveCompleted(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0)
                MessageHandler.ProcessRawData(e.Buffer, e.Offset, e.BytesTransferred, e);

            ReceiveNextMessage();
        }

        /// <summary>
        /// Handle last operation complete event from socket.
        /// </summary>
        /// <param name="sender">Socket instance</param>
        /// <param name="e"></param>
        private void OnSocketAsyncOperationCompleted(Object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    switch (e.LastOperation)
                    {
                        case SocketAsyncOperation.Receive:
                            ProcessReceiveCompleted(e);
                            break;
                        case SocketAsyncOperation.Send:
                            ProcessSendCompleted();
                            break;
                        case SocketAsyncOperation.Connect:
                            ProcessConnectCompleted(e);
                            break;
                    }
                }
                else
                {
                    Disconnect();
                    RaiseConnectionLost(e.SocketError, null);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                RaiseConnectionLost(e.SocketError, ex);
            }
        }

        private void RaiseMessageReceived(Object sender, MessageEventArgs e)
        {
            MessageReceived.Raise(this, e);
        }

        private void RaiseConnectionLost(SocketError socketError, Exception ex)
        {
            ConnectionLost.Raise(this, new SocketErrorEventArgs(socketError, ex));
        }

        private void RaiseConnectionSet()
        {
            ConnectionSet.Raise(this, EventArgs.Empty);
        }

        /// <summary>
        /// Creating socket to specified endpoint.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void Connect(String ipAddress, Int32 port)
        {
            try
            {
                var connectEventArgs = new SocketAsyncEventArgs();
                connectEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                connectEventArgs.Completed += OnSocketAsyncOperationCompleted;
                if (!Socket.ConnectAsync(connectEventArgs))
                    ProcessConnectCompleted(connectEventArgs);
            }
            catch (Exception ex)
            {
                Logger.Error($"SocketClient.Connect(): {ex.Message}");
                Disconnect();
                throw;
            }
        }

        public void Disconnect()
        {
            lock (_sendQueue)
                _sendQueue.Clear();

            try
            {
#warning Це я видалив тому що неправильно розривало конекшн:
                //Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
            catch (ObjectDisposedException) { }
            catch (SocketException) { }
        }

        public void Attach()
        {
            try
            {
                _receiveEventArgs.SetBuffer(new Byte[BufferSize], 0, BufferSize);
                ReceiveNextMessage();
            }
            catch (Exception ex)
            {
                Logger.Error($"SocketClient.Attach(): {ex.Message}");
                Disconnect();
                throw;
            }
        }

        public void Send(Byte[] bytes)
        {
            var message = MessageHandler.PackData(bytes, 0, bytes.Length);
            var tottalSize = 0;
            do
            {
                if (tottalSize + BufferSize <= message.Length)
                {
                    var data = new Byte[BufferSize];
                    Array.Copy(message, tottalSize, data, 0, BufferSize);
                    tottalSize += BufferSize;
                    SendMessage(data);
                }
                else
                {
                    var data = new Byte[message.Length - tottalSize];
                    Array.Copy(message, tottalSize, data, 0, message.Length - tottalSize);
                    tottalSize += (message.Length - tottalSize);
                    SendMessage(data);
                }
            } while (tottalSize != message.Length);
        }

        /// <summary>
        /// Sends the message in asynchronous non-blocking way with preserving the correct order.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public async Task SendAsync(Byte[] bytes)
        {
            var message = MessageHandler.PackData(bytes, 0, bytes.Length);
            _messageQueue.Enqueue(message);

            do
            {
                if (Interlocked.Exchange(ref _socketStatus, 1) != 0) return;
                _messageQueue.TryPeek(out message);

                for (var tSize = 0; tSize < message.Length;)
                {
                    var len = tSize + BufferSize <= message.Length ? BufferSize : message.Length - tSize;
                    var data = new Byte[len];
                    Array.Copy(message, tSize, data, 0, len);
                    await SendPartAsync(data).ConfigureAwait(false);
                    tSize += len;
                }

                _messageQueue.TryDequeue(out message);
                Interlocked.Decrement(ref _socketStatus);
                _messageQueue.TryPeek(out message);
            } while (message != null);
        }

        private Int32 _socketStatus; // 0=free, 1=busy
        private const Int32 BufferSize = 8192;
        private readonly Queue<Byte[]> _sendQueue = new Queue<Byte[]>();
        private readonly SocketAsyncEventArgs _sendEventArgs = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _receiveEventArgs = new SocketAsyncEventArgs();
        private readonly ConcurrentQueue<Byte[]> _messageQueue = new ConcurrentQueue<Byte[]>();

        private void SendMessage(Byte[] bytes)
        {
            lock (_sendQueue)
            {
                _sendQueue.Enqueue(bytes);
                if (_sendQueue.Count != 1)
                    return;
            }

            SendNextMessage();
        }

        private async Task SendPartAsync(Byte[] bytes)
        {
            try
            {
                await WriteAsync(bytes, 0, bytes.Length, SocketFlags.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error($"SendPartAsync.Attach(): {ex.Message}");
                Disconnect();
                RaiseConnectionLost(SocketError.Success, ex);
            }
        }

    }
}

