using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Models.Messages;
using NLog;

namespace Models.Network
{
    public class SessionManager
    {
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public List<Session> Sessions { get; private set; }
        public static SessionManager Current { get; private set; }

        public SessionManager(string ipAddress, int port)
        {
            Logger.Info($"Start SessionManager...");
            if (Current == null)
                Current = this;
            _clientCount = 0;
            Sessions = new List<Session>();
            Start(new IPEndPoint(IPAddress.Parse(ipAddress), port));
        }

        private Int32 _clientCount;
        //private SocketListener _socketListener;
        private TCPServer _socketServer;

        //private HttpSocketListener _httpSocketListener;

        //public Session StartNewSession(String userName, bool supressEventHandle = false)
        //{
        //    var session = new Session(new MessageHandler());
        //    if (!supressEventHandle)
        //    {
        //        session.SessionAuthenticated += (s, e) => SessionAuthenticated.Raise(this, new SessionEventArgs(s as Session));
        //        session.UserName = userName;
        //        session.SessionClosed += OnSessionClosed;
        //        session.SessionOpening += RaiseSessionOpening;
        //        session.SessionOpened += OnSessionOpened;
        //    }
        //    return session;
        //}

        public Session FindSession(String usid)
        {
            return Sessions.FirstOrDefault(s => s.IdentifierUserSession == usid);
        }

        private void Stop(SocketListener listener)
        {
            listener.Stop();
            listener.Dispose();
            //if (_httpSocketListener != null)
            //    _httpSocketListener.Stop();
        }

        private void Start(IPEndPoint endPoint)
        {
            try
            {
                _socketServer = new TCPServer(endPoint);
                _socketServer.StartServer();
                _socketServer.Accepted += Current.SocketAccepted;

                //_socketListener = new SocketListener(endPoint);
                //_socketListener.Start();
                //_socketListener.Accepted += Current.SocketAccepted;
            }
            catch (Exception ex)
            {
                Logger.Error($"SessionManager.Start(): {ex.Message} Endpoint {endPoint}");
                throw new InvalidOperationException("End point not supported.");
            }
        }

        //private void InitHttpSocketListener(EndPoint endPoint)
        //{
        //    try
        //    {
        //        if (_httpSocketListener == null)
        //        {
        //            _httpSocketListener = new HttpSocketListener();
        //            _httpSocketListener.RequestReceived += RaiseHttpRequestReceived;
        //        }
        //        else
        //            _httpSocketListener.Stop();

        //        _httpSocketListener.AddEndpoint(endPoint.Host, endPoint.Port);
        //        _httpSocketListener.Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        private void SocketAccepted(Object sender, SocketEventArgs args)
        {
            Interlocked.Increment(ref _clientCount);
            Logger.Info($"SocketAccepted(): {args.Socket.RemoteEndPoint}");
            var session = new Session(args.Socket, new MessageHandler());
            session.SessionClosed += OnSessionClosed;
            session.SessionAuthenticated += (s, e) => SessionAuthenticated(this, new SessionEventArgs(s as Session));
            OnSessionOpened(session, EventArgs.Empty);  // we have to put session into collection and raise event
        }

        private void OnSessionClosed(Object sender, EventArgs args)
        {
            lock (Sessions)
                Sessions.Remove((Session)sender);

            SessionClosed.Raise(this, new SessionEventArgs((Session)sender));
        }

        private void OnSessionOpened(Object sender, EventArgs e)
        {
            var session = (Session)sender;

            // if the session is succesfully opened - notifying everybody about!
            lock (Sessions)
                Sessions.Add(session);

            NewSessionConnected.Raise(this, new SessionEventArgs(session));
        }

        private void RaiseSessionOpening(Object sender, EventArgs e)
        {
            SessionOpening.Raise(this, new SessionEventArgs(sender as Session));
        }

        /// <summary>Terminate specified Session.</summary>
        /// <param name="sessionId"></param>
        public void CloseSession(Guid sessionId)
        {
            var session = Sessions.FirstOrDefault(s => s.SessionID == sessionId);
            if (session != null)
            {
                session.CloseSession();
                OnSessionClosed(session, new SocketErrorEventArgs());
            }
        }

        /// <summary>
        /// Search and return session with specified Id.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public Session FindSession(Guid sessionId)
        {
            return Sessions.FirstOrDefault(s => s.SessionID == sessionId);
        }

        /// <summary>
        /// Raised when Session is terminated
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionClosed;
        /// <summary>
        /// Events for HandlerManager, report status changes.
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionOpening;
        public event EventHandler<SessionEventArgs> NewSessionConnected;
        /// <summary>
        /// Raised when new income session is connected
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionAuthenticated;
        //public event EventHandler<HttpRequestReceivedEventArgs> HttpRequestReceived;
    }

    public class SessionEventArgs : EventArgs
    {
        public Session Session { get; private set; }

        /// <summary>
        /// Constructor for type SessionEventArgs
        /// </summary>
        /// <param name="session"></param>
        public SessionEventArgs(Session session)
            : base()
        {
            this.Session = session;
        }
    }

    /// <summary>
    /// Сlass that contains information about the event of type SessionMessageEventArgs, inherited from SessionEventArgs
    /// </summary>
    public class SessionMessageEventArgs : SessionEventArgs
    {
        public MessageTypes MessageType { get; private set; }
        public string MessageJSON { get; private set; }

        public SessionMessageEventArgs(Session session, MessageTypes messageType, string messageJSON)
            : base(session)
        {
            MessageType = messageType;
            MessageJSON = messageJSON;
        }
    }
}
