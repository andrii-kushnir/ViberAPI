using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;

namespace Models.Network
{
    public class TCPServer
    {
        static Logger Logger = LogManager.GetCurrentClassLogger();

        public static TCPServer Current { get; private set; }
        private static TcpListener _server;
        private static Thread serverThread { get; set; }
        private static bool stopServer { get; set; }

        public TCPServer(IPEndPoint endPoint)
        {
            if (Current == null)
                Current = this;

            try
            {
                _server = new TcpListener(endPoint);
            }
            catch
            {
                Logger.Error($"Error create server");
                _server = null;
            }
            Logger.Info($"Server created");
        }

        public void StartServer()
        {
            if (_server != null)
            {
                _server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _server.Start();
                stopServer = false;
                serverThread = new Thread(new ThreadStart(ServerThreadStart));
                serverThread.Start();
            }
            else
            {
                Logger.Error($"Error in server connection");
            }
            Logger.Info($"Server started");
        }

        private void ServerThreadStart()
        {
            Socket clientSocket = null;
            while (!stopServer)
            {
                try
                {
                    if (!_server.Pending())
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    clientSocket = _server.AcceptSocket();
                    Accepted.Raise(this, new SocketEventArgs(clientSocket));
                }
                catch
                {
                    Logger.Error($"Not Accepted Socket");
                    stopServer = true;
                }
            }
        }

        public event EventHandler<SocketEventArgs> Accepted;

        public void StopServer()
        {
            if (_server != null)
            {
                stopServer = true;
                _server.Stop();

                // Wait for one second for the the thread to stop.
                serverThread.Join(1000);

                if (serverThread.IsAlive)
                {
                    serverThread.Abort();
                }
                serverThread = null;
                _server = null;
                foreach (var session in SessionManager.Current.Sessions)
                {
                    try
                    {
                        session.GetSocketClient().Disconnect();
                    }
                    catch 
                    {
                        Logger.Error($"StopServer Error");
                    }
                }

                Logger.Info($"Server stoped");
            }
        }


        //public class TCPSocketListener
        //{
        //    public Socket socket { get; set; }
        //    private Thread thread { get; set; }
        //    private bool stopReceive { get; set; }

        //    public TCPSocketListener(Socket socket)
        //    {
        //        this.socket = socket;
        //    }

        //    public void StartSocketListener()
        //    {
        //        stopReceive = false;
        //        thread = new Thread(new ThreadStart(ReceiveThread));
        //        thread.Start();

        //    }

        //    private void ReceiveThread()
        //    {
        //        byte[] bytes = null;
        //        while (!stopReceive)
        //        {
        //            try
        //            {
        //                bytes = new byte[1024000];
        //                int bytesRec = socket.Receive(bytes);
        //                string data = Encoding.Default.GetString(bytes);
        //                if (data.Length == 0)
        //                {
        //                    break;
        //                }
        //                AddPacket(data, (socket.RemoteEndPoint as IPEndPoint).Address.ToString());
        //                new string[] { data });
        //            }
        //            catch (Exception ex)
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    public void StopReceive()
        //    {
        //        acceptedSockets.Remove(socket.RemoteEndPoint);
        //        stopReceive = true;
        //        thread.Abort();
        //        socket.Shutdown(SocketShutdown.Receive);
        //        socket.Close();
        //    }

        //    public delegate void DataEventHandler(object sender, DataEventArgs e);
        //    public event DataEventHandler OnPacketReceive;
        //    public void AddPacket(string data, string sourceIp)
        //    {
        //        DataEventArgs args = new DataEventArgs();
        //        args.data = data;
        //        args.sourceIp = sourceIp;
        //        PacketReceive(args);
        //    }
        //    protected virtual void PacketReceive(DataEventArgs e)
        //    {
        //        OnPacketReceive?.Invoke(this, e);
        //    }
        //}

        //public class DataEventArgs : EventArgs
        //{
        //    public string data { get; set; }
        //    public string sourceIp { get; set; }

        //    public DataEventArgs()
        //        : base()
        //    {
        //    }
        //}
    }


}
