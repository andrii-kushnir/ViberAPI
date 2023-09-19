using System;
using System.Net.Sockets;

namespace Models.Network
{
    /// <summary>
    /// Клас, який містить дані для події SocketAccepted  
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }

        public SocketEventArgs(Socket socket)
        {
            Socket = socket;
        }
    }

    /// <summary>
    /// Клас, який містить дані для події SocketAcceptError 
    /// </summary>
    public class SocketErrorEventArgs : EventArgs
    {
        public SocketError SocketError { get; private set; }
        public Exception Exception { get; private set; }

        public SocketErrorEventArgs()
        {
            SocketError = new SocketError();
            Exception = null;
        }

        public SocketErrorEventArgs(SocketError error)
        {
            SocketError = error;
        }

        public SocketErrorEventArgs(SocketError error, Exception ex)
            : this(error)
        {
            Exception = ex;
        }
    }

    /// <summary>
    /// Клас, який містить дані для події MessageReceived 
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public byte[] Message { get; private set; }

        public MessageEventArgs(byte[] message)
        {
            Message = message;
        }
    }
}
