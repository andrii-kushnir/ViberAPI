using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Models.Messages;
using Newtonsoft.Json;
using NLog;

namespace Models.Network
{
    public class Session
    {
        static Logger Logger = LogManager.GetCurrentClassLogger();

        public Session(MessageHandler messageHandler, string url, int port) : this(messageHandler)
        {
            Url = url;
            Port = port;
        }

        public Session(MessageHandler messageHandler) : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp), messageHandler) { }

        public Session(Socket socket, MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            //CurrentRequestsCollection = new List<IRequest>();
            //CurrentRequestCallbacksCollection = new Dictionary<IRequest, List<Action<IResponse>>>();
            SessionID = Guid.NewGuid();
            SessionStartDateTime = DateTime.Now;
            InitSocketClient(socket);
        }

        private Boolean _isAutentificated;
        private const String UnameDefault = "UNKNOWN";
        private readonly MessageHandler _messageHandler;
        private SocketClient _socketClient;

        private readonly string Url;
        private readonly int Port;

        public SocketClient GetSocketClient()
        {
            return _socketClient;
        }

        public void ReConnect()
        {
            if (_socketClient.Socket.Connected)
                return;

            DetachSocketClient();

            var newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            InitSocketClient(newsocket);
            Connect();
        }


        /// <summary>
        /// Safely sends data through the specified socket.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="data"></param>
        /// <remarks>(to send the message correctly, the socket will be locked until all parts of message will be sent)</remarks>
        private static void SafeSend(SocketClient socket, Byte[] data)
        {
            lock (socket)
                socket.Send(data);
        }

        private String GenerateSessionIdentifier(String ipAddress, Int32 port)
        {
            return String.Concat(String.Format(ipAddress + "-" + port), String.IsNullOrEmpty(UserName) ? UnameDefault : UserName);
        }

        /// <summary>
        /// Initialize new instance of the SocketClient Type. Put it into internal collection.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns>Index of the just created SocketClient.</returns>
        private void InitSocketClient(Socket socket)
        {
            _socketClient = new SocketClient(socket, _messageHandler);
            AttachSocketClient();

            try
            {
                if (socket.Connected)
                    _socketClient.Attach();
            }
            catch (Exception ex)
            {
                Logger.Error($"Session.InitSocketClient(): {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Attach event handlers to specified SocketClient.
        /// </summary>
        /// <param name="sc"></param>
        private void AttachSocketClient()
        {
            _socketClient.ConnectionLost += OnConnectionLost;
            _socketClient.ConnectionSet += OnConnectionSet;
            _socketClient.MessageReceived += OnMessageReceived;
        }

        private void DetachSocketClient()
        {
            _socketClient.ConnectionLost -= OnConnectionLost;
            _socketClient.ConnectionSet -= OnConnectionSet;
            _socketClient.MessageReceived -= OnMessageReceived;
        }

        /// <summary>
        /// Notify classes above about Session Status Changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionLost(Object sender, SocketErrorEventArgs e)
        {
            var socketClient = (SocketClient)sender;    // attention! if we receive some other sender - it will be very bad.
            SessionClosed.Raise(this, e);

            ConnectionClosed.Raise(sender, e);
        }

        /// <summary>
        /// Notify classes above about Session Status Changing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionSet(Object sender, EventArgs e)
        {
            var socketClient = (SocketClient)sender;    // attention! if we receive some other sender - it will be very bad.
            SessionOpened.Raise(this, EventArgs.Empty);

            ConnectionOpened.Raise(sender, new SessionEventArgs(this));
        }

        /// <summary>
        /// Raised when some message is received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMessageReceived(Object sender, MessageEventArgs args)
        {
            try
            {
                var messageJSON = Encoding.UTF8.GetString(args.Message);
                var message = JsonConvert.DeserializeObject<Message>(messageJSON);
                //Logger.Info($"Отримано: {messageJSON}");
                MessageReceive.Raise(this, new SessionMessageEventArgs(this, message.MessageType, messageJSON));
            }
            catch (Exception ex)
            {
                Logger.Error($"Session.OnMessageReceived(): {ex.Message}");
            }
        }

        public Guid SessionID { get; set; }
        public String UserName { get; set; }
        public DateTime SessionStartDateTime { get; private set; }
        public Boolean IsAutenticated
        {
            get { return _isAutentificated; }
            set
            {
                if (_isAutentificated == value) return;
                _isAutentificated = value;
                if (_isAutentificated)
                    SessionAuthenticated.Raise(this, EventArgs.Empty);
            }
        }
        public String IdentifierUserSession { get; set; }
        ///// <summary>
        ///// Collection of the processed requests, that are waiting for server's response.
        ///// </summary>
        //public List<IRequest> CurrentRequestsCollection { get; private set; }
        ///// <summary>
        ///// Collection of callbacks which are called when response is received.
        ///// </summary>
        //public Dictionary<IRequest, List<Action<IResponse>>> CurrentRequestCallbacksCollection { get; private set; }

        public void Send(Message message)
        {
            if (message == null) return;
            try
            {
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var socket = GetSocketClient();
                SafeSend(socket, buffer);
            }
            catch (Exception ex)
            {
                Logger.Error($"Session.Send(): {ex.Message}");
            }
        }

        public async Task SendAsync(Message message)
        {
            if (message == null) return;

            try
            {
                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                await GetSocketClient().SendAsync(buffer);
            }
            catch (Exception ex)
            {
                Logger.Error($"Session.SendAsync(): {ex.Message}");
            }
        }

        public void Connect()
        {
            try
            {
                var socket = GetSocketClient();
                IdentifierUserSession = GenerateSessionIdentifier(Url, Port);
                lock (socket)
                    socket.Connect(Url, Port);
                SessionOpening.Raise(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Logger.Error($"Session.Connect(): {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Close current session.
        /// </summary>
        /// <remarks>(all connections will be closed)</remarks>
        public void CloseSession()
        {
            _socketClient.Disconnect();
        }

        /// <summary>
        /// Raises when user is authenticated.
        /// </summary>
        public event EventHandler SessionAuthenticated;
        /// <summary>
        /// Raised when new session is opened.
        /// \note Actually this event will be raised when the first connection (socket) will be opened. \nSo in this case two events will be appeared: SessionOpened and ConnectionOpened
        /// </summary>
        public event EventHandler<EventArgs> SessionOpened;
        /// <summary>
        /// Event that reports the closing a session. 
        /// </summary>
        public event EventHandler<EventArgs> SessionClosed;
        /// <summary>
        /// Raised when new session is opening.
        /// </summary>
        public event EventHandler<EventArgs> SessionOpening;
        /// <summary>
        /// Raised when new connection is opened. 
        /// \note sender is SocketClient. It has Index property (if its necessary to know which session's connection is source of the event)
        /// </summary>
        /// <remarks>(session can handle few connections)</remarks>
        public event EventHandler<SessionEventArgs> ConnectionOpened;
        /// <summary>
        /// Raised when connection is terminated.
        /// </summary>
        public event EventHandler<SocketErrorEventArgs> ConnectionClosed;
        /// <summary>
        /// Event that reports the receipt a message.
        /// </summary>
        public event EventHandler<SessionMessageEventArgs> MessageReceive;
    }
}
