using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Models.Network
{
    /// <summary>
    /// Listens the client connections.
    /// </summary>
    public class SocketListener : IDisposable
    {
        public Int32 PendingConnectionsCount { get; set; }

        public SocketListener(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            PendingConnectionsCount = DefaultPendingConnectionsCount;

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketEventArgs = new SocketAsyncEventArgs();
            _socketEventArgs.Completed += OnSocketAsyncOperationCompleted;
        }

        //public SocketListener(String ipAddress, Int32 port)
        //{
        //    var ip = IPAddress.Parse(ipAddress);
        //    EndPoint = new IPEndPoint(ip, port);
        //    PendingConnectionsCount = DefaultPendingConnectionsCount;

        //    _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    _socketEventArgs = new SocketAsyncEventArgs();
        //    _socketEventArgs.Completed += OnSocketAsyncOperationCompleted;
        //}

        private Boolean _disposed;
        private readonly Socket _listener;
        private readonly SocketAsyncEventArgs _socketEventArgs;
        private const Int32 DefaultPendingConnectionsCount = 100;

        private void AcceptNextClient()
        {
            while (true)
            {
                if (_listener.AcceptAsync(_socketEventArgs)) 
                    return;
                ProccessAcceptCompleted(_socketEventArgs);
            }
        }

        private void ProccessAcceptCompleted(SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    if (e.AcceptSocket.Connected)
                    {
                        var socket = e.AcceptSocket;
                        Task.Run(() => Accepted.Raise(this, new SocketEventArgs(socket)));
                        e.AcceptSocket = null;
                    }
                }
                else
                {
                    AcceptError.Raise(this, new SocketErrorEventArgs(e.SocketError, null));
                }
            }
            catch (Exception ex)
            {
                AcceptError.Raise(this, new SocketErrorEventArgs(e.SocketError, ex));
            }
        }

        private void OnSocketAsyncOperationCompleted(Object sender, SocketAsyncEventArgs e)
        {
            if (e.LastOperation == SocketAsyncOperation.Accept)
            {
                if (e.SocketError == SocketError.Success)
                    ProccessAcceptCompleted(e);
                try
                {
                    AcceptNextClient();
                }
                catch (Exception ex)
                {
                    Stop();
                    Stopped.Raise(this, new SocketErrorEventArgs(e.SocketError, ex));
                }
            }
        }

        public Socket Socket
        {
            get { return _listener; }
        }

        public event EventHandler<SocketEventArgs> Accepted;
        public event EventHandler<SocketErrorEventArgs> AcceptError;
        public event EventHandler<SocketErrorEventArgs> Stopped;

        public IPEndPoint EndPoint { get; private set; }

        public void Start()
        {
            try
            {
                _listener.Bind(EndPoint);
                _listener.Listen(PendingConnectionsCount);
                AcceptNextClient();
            }
            catch
            {
                Stop();
                throw;
            }
        }

        public virtual void Stop()
        {
            _listener.Close();
        }
 
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Stop();
            _listener.Dispose();

            _disposed = true;
        }
    }
}

