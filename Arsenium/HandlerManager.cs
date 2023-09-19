using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using Models.Messages;
using Models.Messages.Requests;
using Models.Messages.Responses;
using Models.Network;
using Newtonsoft.Json;
using NLog;

namespace Arsenium
{
    public class HandlerManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Session _session;
        //private MainWin _mainWin;
        public static object lockObject = new object();

        public event EventHandler<EventArgs> LoginAccessResponse;
        public event EventHandler<SocketErrorEventArgs> ConnectionClosed;

        public HandlerManager(Session session)
        {
            _session = session;
            _session.MessageReceive += SessionOnMessageReceive;
            _session.ConnectionClosed += SessionOnConnectionClosed;
            //Session.SessionOpening += SessionOnSessionOpening;
            _session.Connect();
        }

        private void SessionOnMessageReceive(object sender, SessionMessageEventArgs e)
        {
            if (Program.MainWin == null && e.MessageType != MessageTypes.PingResponse && e.MessageType != MessageTypes.LoginResponse)
                return;

            switch (e.MessageType)
            {
                case MessageTypes.PingResponse:
                    {
                        var message = JsonConvert.DeserializeObject<PingResponse>(e.MessageJSON);
                        Logger.Info($"Session {e.Session.SessionID}; Ping: відгук - {Convert.ToInt32((DateTime.Now - message.Time).TotalMilliseconds)} ms");
                        //Logger.Debug($"Потік: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                        //MessageShow($"Ping: відгук - {Convert.ToInt32((DateTime.Now - message.Time).TotalMilliseconds)} ms");
                    }
                    break;
                case MessageTypes.LoginResponse:
                    {
                        var message = JsonConvert.DeserializeObject<LoginResponse>(e.MessageJSON);
                        if (!message.Access)
                        {
                            MessageBox.Show("Сервер відмовив у доступі. Перевірте логін/пароль");
                            break;
                        }
                        e.Session.SessionID = message.Myself.Id;
                        ClientManager.Myself = message.Myself;
                        foreach (var user in message.UserList)
                        {
                            if (user.Id != message.Myself.Id)
                            {
                                lock (lockObject)
                                {
                                    var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == user.Id);
                                    if (client == null)
                                    {
                                        ClientManager.Add(new Client(user.Id, user.Name, user.Avatar, user.UserType));
                                    }
                                }
                            }
                        }

                        foreach (var user in message.LastClient)
                        {
                            var client = new Client(user.Id, user.Name, user.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(user)
                            };
                            ClientManager.Add(client);
                        }

                        foreach (var user in message.NightClient)
                        {
                            var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == user.Id);
                            if (client == null)
                            {
                                client = new Client(user.Id, user.Name, user.Avatar, UserTypes.Viber)
                                {
                                    DetailsInfo = new DetailsInfo(user)
                                };
                                ClientManager.Add(client);
                            }
                            if (user.operatoId == Guid.Empty || user.operatoId == message.Myself.Id)
                                ClientManager.SetNodeBlink(client);
                            else
                                ClientManager.SetNodeBlink(client);
                        }

                        LoginAccessResponse.Raise(this, new EventArgs());
                    }
                    break;
                case MessageTypes.ArseniumOnlineRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ArseniumOnlineRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.User.Id);
                        if (client == null)
                        {
                            client = new Client(message.User.Id, message.User.Name, message.User.Avatar, UserTypes.Asterium);
                            ClientManager.Add(client);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                    }
                    break;
                case MessageTypes.ArseniumOfflineRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ArseniumOfflineRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.User.Id);
                        if (client != null)
                        {
                            ClientManager.Remove(client);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.RemoveClient(client);
                            }));
                        }
                    }
                    break;
                case MessageTypes.FindOperatorRequest:
                    {
                        var message = JsonConvert.DeserializeObject<FindOperatorRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client == null)
                        {
                            client = new Client(message.UserViber.Id, message.UserViber.Name, message.UserViber.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(message.UserViber)
                            };

                            ClientManager.Add(client);

                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                        else
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.ClientToTop(client);
                            }));
                        }

                        if (client.ClientWindow == null || client.ClientWindow.IsDisposed || Form.ActiveForm != client.ClientWindow)
                        {
                            client.WaitOperator = true;
                            ClientManager.SetNodeBlink(client);
                            if (Form.ActiveForm != Program.MainWin)
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    if (client.PopUpWindow != null && !client.PopUpWindow.IsDisposed)
                                    {
                                        client.PopUpWindow.Close();
                                    }
                                    client.PopUpWindow = new PopUp(client, message.UserViber.operatoName);
                                    client.PopUpWindow.Show();
                                }));
                        }
                        else
                        {
                            var response = new FindOperatorResponse(message, new User(client.Id));
                            Program.Session.Send(response);
                        }
                    }
                    break;
                case MessageTypes.ClientBusyRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ClientBusyRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            client.WaitOperator = false;
                            ClientManager.SetNodeUnblink(client);
                            if (client.PopUpWindow != null && !client.PopUpWindow.IsDisposed)
                            {
                                client.PopUpWindow.Close();
                            }
                        }
                    }
                    break;
                case MessageTypes.AttachOperatorRequest:
                    {
                        var message = JsonConvert.DeserializeObject<AttachOperatorRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            ClientManager.SetNodeUnblink(client);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                client.Node.Text = $"{client.NameShow} [{client.DetailsInfo.OperatorName}]";
                                if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                                {
                                    client.ClientWindow.ChangeOperator(client.DetailsInfo.OperatorName);
                                }
                            }));
                        }
                    }
                    break;
                case MessageTypes.MessageFromViberRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageFromViberRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client == null)
                        {
                            client = new Client(message.UserViber.Id, message.UserViber.Name, message.UserViber.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(message.UserViber)
                            };

                            ClientManager.Add(client);

                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                        else
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.ClientToTop(client);
                            }));
                        }

                        if (message.Message.ChatMessageType == ChatMessageTypes.MessageFromViber && (client.ClientWindow == null || client.ClientWindow.IsDisposed || Form.ActiveForm != client.ClientWindow))
                        {
                            ClientManager.SetNodeBlink(client);

                            if ((client.PopUpWindow == null || client.PopUpWindow.IsDisposed) && (Form.ActiveForm != Program.MainWin))
                            {
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    client.PopUpWindow = new PopUp(client, message.UserViber.operatoName, message.Message.Text);
                                    client.PopUpWindow.Show();
                                }));
                            }
                        }

                        if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                        {
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                client.ClientWindow.ShowMessageClient(message.Message);
                            }));
                        }

                        //if (client.DetailsInfo != null)
                        //    client.DetailsInfo.MessageList.Add(message.Message);
                    }
                    break;
                case MessageTypes.MessageToViberResponse:
                    {
                        var message = JsonConvert.DeserializeObject<MessageToViberResponse>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            var chatMessage = client.DetailsInfo.MessageList.FirstOrDefault(msg => msg.MessageId == message.Message.MessageId);
                            if (chatMessage != null)
                            {
                                chatMessage.Token = message.Message.Token;
                                if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                                {
                                    Program.MainWin.Invoke(new Action(() =>
                                    {
                                        client.ClientWindow.ChangeIcon(message.Message.MessageId);
                                    }));
                                }
                            }
                        }
                    }
                    break;
                case MessageTypes.FileToViberResponse:
                    {
                        var message = JsonConvert.DeserializeObject<FileToViberResponse>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            var chatMessage = client.DetailsInfo.MessageList.FirstOrDefault(msg => msg.MessageId == message.Message.MessageId);
                            if (chatMessage != null)
                            {
                                chatMessage.Token = message.Message.Token;
                                if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                                {
                                    Program.MainWin.Invoke(new Action(() =>
                                    {
                                        client.ClientWindow.ChangeIcon(message.Message.MessageId);
                                    }));
                                }
                            }
                        }
                    }
                    break;
                    case MessageTypes.ImageToViberResponse:
                    {
                        var message = JsonConvert.DeserializeObject<ImageToViberResponse>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            var chatMessage = client.DetailsInfo.MessageList.FirstOrDefault(msg => msg.MessageId == message.Message.MessageId);
                            if (chatMessage != null)
                            {
                                chatMessage.Token = message.Message.Token;
                                if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                                {
                                    Program.MainWin.Invoke(new Action(() =>
                                    {
                                        client.ClientWindow.ChangeIcon(message.Message.MessageId);
                                    }));
                                }
                            }
                        }
                    }
                    break;
                case MessageTypes.MessageDeliveredRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageDeliveredRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null && client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                        {
                            var chatMessage = client.DetailsInfo.MessageList.FirstOrDefault(msg => msg.Token == message.Token);
                            if (chatMessage != null)
                            {
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    client.ClientWindow.ShowMessageDelivered(chatMessage.MessageId);
                                }));
                            }
                        }

                        if (client?.DetailsInfo != null)
                        {
                            var msg = client.DetailsInfo.MessageList.FirstOrDefault(m => m.Token == message.Token);
                            if (msg != null)
                                msg.DateDelivered = message.DateDelivered;
                        }
                    }
                    break;
                case MessageTypes.MessageSeenRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageSeenRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null && client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                        {
                            var chatMessage = client.DetailsInfo.MessageList.FirstOrDefault(msg => msg.Token == message.Token);
                            if (chatMessage != null)
                            {
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    client.ClientWindow.ShowMessageSeen(chatMessage.MessageId);
                                }));
                            }
                        }

                        if (client?.DetailsInfo != null)
                        {
                            var msg = client.DetailsInfo.MessageList.FirstOrDefault(m => m.Token == message.Token);
                            if (msg != null)
                                msg.DateSeen = message.DateSeen;
                        }
                    }
                    break;
                case MessageTypes.MessageViberPhone:
                    {
                        var message = JsonConvert.DeserializeObject<MessageViberPhone>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client?.DetailsInfo != null)
                        {
                            client.DetailsInfo.Phone = message.UserViber.phone;
                            if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                            {
                                void ShowPhone()
                                {
                                    if (client.ClientWindow.InvokeRequired)
                                    {
                                        client.ClientWindow.SafeInvoke(ShowPhone);
                                        return;
                                    }
                                    client.ClientWindow.ShowPhone(message.UserViber.phone);
                                }
                                ShowPhone();
                            }
                        }
                    }
                    break;
                case MessageTypes.UserListResponse:
                    {
                        var message = JsonConvert.DeserializeObject<UserListResponse>(e.MessageJSON);
                        foreach (var user in message.Users)
                        {
                            var client = new Client(user.Id, user.Name, user.Avatar, user.UserType);
                            ClientManager.Add(client);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                //напевно переробити коли буде використовуватись:
                                Program.MainWin.AddClient(client);
                            }));
                        }
                    }
                    break;
                case MessageTypes.UserDetailsResponse:
                    {
                        var message = JsonConvert.DeserializeObject<UserDetailsResponse>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.User.Id);
                        if (client != null)
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                client.ClientWindow = new ClientSessionWin(client) { Text = $"Далог з {client.NameShow}" };
                                client.ClientWindow.Show();
                            }));
                        }
                    }
                    break;
                case MessageTypes.UserDetailsBuhnetResponse:
                    {
                        var message = JsonConvert.DeserializeObject<UserDetailsBuhnetResponse>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client != null)
                        {
                            if (client.ClientWindow != null && !client.ClientWindow.IsDisposed && Form.ActiveForm == client.ClientWindow)
                            {
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    client.ClientWindow = new ClientSessionWin(client) { Text = $"Далог з {client.NameShow}" };
                                    client.ClientWindow.Show();
                                }));
                            }
                        }
                    }
                    break;
                case MessageTypes.NewConversationRequest:
                    {
                        var message = JsonConvert.DeserializeObject<NewConversationRequest>(e.MessageJSON);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client == null)
                        {
                            client = new Client(message.UserViber.Id, message.UserViber.Name, message.UserViber.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(message.UserViber)
                            };
                            ClientManager.Add(client);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                        else
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.ClientToTop(client);
                            }));
                        }
                    }
                    break;
                case MessageTypes.FindUserResponse:
                    {
                        var message = JsonConvert.DeserializeObject<FindUserResponse>(e.MessageJSON);
                        if (message.UserViber == null)
                        {
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                var findWin = new FindFromPhoneWin()
                                {
                                    Location = new Point(Cursor.Position.X - 200, Cursor.Position.Y)
                                };
                                findWin.ShowDialog();
                            }));
                            break;
                        }
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client == null)
                        {
                            client = new Client(message.UserViber.Id, message.UserViber.Name, message.UserViber.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(message.UserViber)
                            };

                            ClientManager.Add(client);

                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                        else
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                        }

                        Program.MainWin.Invoke(new Action(() =>
                        {
                            client.ClientWindow = new ClientSessionWin(client) { Text = $"Далог з {client.NameShow}" };
                            client.ClientWindow.Show();
                        }));
                    }
                    break;
                case MessageTypes.FileFromViberRequest:
                    {
                        var message = JsonConvert.DeserializeObject<FileFromViberRequest>(e.MessageJSON);
                        //File.Copy(Program.Files + message.Message.Text, Path.GetTempPath() + message.Message.Text.Substring(36), true);
                        var client = ClientManager.GetAllClients().FirstOrDefault(c => c.Id == message.UserViber.Id);
                        if (client == null)
                        {
                            client = new Client(message.UserViber.Id, message.UserViber.Name, message.UserViber.Avatar, UserTypes.Viber)
                            {
                                DetailsInfo = new DetailsInfo(message.UserViber)
                            };

                            ClientManager.Add(client);

                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.AddClient(client);
                            }));
                        }
                        else
                        {
                            client.DetailsInfo = new DetailsInfo(message.UserViber);
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                Program.MainWin.ClientToTop(client);
                            }));
                        }

                        if (client.ClientWindow == null || client.ClientWindow.IsDisposed || Form.ActiveForm != client.ClientWindow)
                        {
                            ClientManager.SetNodeBlink(client);

                            if ((client.PopUpWindow == null || client.PopUpWindow.IsDisposed) && (Form.ActiveForm != Program.MainWin))
                            {
                                Program.MainWin.Invoke(new Action(() =>
                                {
                                    string msg = null;
                                    switch (message.Message.ChatMessageType)
                                    {
                                        case ChatMessageTypes.ImageFromViber:
                                            msg = "Прийшло зображення";
                                            break;
                                        case ChatMessageTypes.VideoFromViber:
                                            msg = "Прийшло відео";
                                            break;
                                        case ChatMessageTypes.FileFromViber:
                                            msg = "Прийшов файл";
                                            break;
                                    }
                                    client.PopUpWindow = new PopUp(client, message.UserViber.operatoName, msg);
                                    client.PopUpWindow.Show();
                                }));
                            }
                        }

                        if (client.ClientWindow != null && !client.ClientWindow.IsDisposed)
                        {
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                switch (message.Message.ChatMessageType)
                                {
                                    case ChatMessageTypes.MessageFromViber:
                                        client.ClientWindow.ShowMessageClient(message.Message);  // на майбутнє
                                        break;
                                    case ChatMessageTypes.ImageFromViber:
                                        client.ClientWindow.ShowImageClient(message.Message);
                                        break;
                                    case ChatMessageTypes.VideoFromViber:
                                    case ChatMessageTypes.FileFromViber:
                                        client.ClientWindow.ShowFileClient(message.Message);
                                        break;
                                    default:
                                        client.ClientWindow.ShowMessageClient(message.Message); // на майбутнє
                                        break;
                                }
                            }));
                        }
                    }
                    break;
                case MessageTypes.PoolsListResponse:
                    {
                        var message = JsonConvert.DeserializeObject<PoolsListResponse>(e.MessageJSON);
                        if (message.Pools == null || message.Pools.Count == 0)
                            MessageBox.Show(message.Result);
                        else
                            Program.MainWin.Invoke(new Action(() =>
                            {
                                var poolWin = new PoolsWin(message.Pools);
                                poolWin.Show();
                            }));
                    }
                    break;
            }
        }

        private void SessionOnConnectionClosed(object sender, SocketErrorEventArgs e)
        {
            ConnectionClosed.Raise(this, e);
        }

        private static void Session_SessionOpening(object sender, EventArgs e)
        {
            (sender as Session).Send(new PingRequest());
            //MessageBox.Show($"Адрес подключения {(sender as Session).GetSocketClient().Socket.RemoteEndPoint}");
            //MessageBox.Show($"Адрес приложения {(sender as Session).GetSocketClient().Socket.LocalEndPoint}");
        }
    }
}
