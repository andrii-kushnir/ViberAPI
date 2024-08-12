using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Messages;
using Models.Network;
using Models.Messages.Requests;
using Newtonsoft.Json;
using Models.Messages.Responses;
using Models;
using NLog;
using ViberAPI.Models;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading;
using RozetkaAPI;
using Chat = RozetkaAPI.ModelsRozetka.Chat;

namespace ViberAPI
{
    public class HandlerManager
    {
        public static HandlerManager Current { get; private set; }

        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string sendMessagePath = "https://chatapi.viber.com/pa/send_message";
        private const string getUserDetails = "https://chatapi.viber.com/pa/get_user_details";
        private readonly HttpClient _httpClient;

        private static readonly List<Click> clicks = new List<Click>();
        private class Click
        {
            public string id;
            public string action;
            public DateTime dateTime;
        }

        private readonly Timer _timerClicks;
        private readonly Timer _timerRozetkaLogin;
        private readonly Timer _timerRozetka;
        private readonly Timer _timerProm;

        private static readonly List<PromAPI.ModelsProm.Message> messagesProm = new List<PromAPI.ModelsProm.Message>();

        private ApiManager _rozetkaApi;
        private PromAPI.ApiManager _promApi;

        private const int intervalClear = 60 * 60 * 1000; // Кількість хвилин неактивності
        private const int intervalChek = 60 * 1000; // Інтервал провірки неактивності
        private const int intervalLoginRozetka = 23 * 60 * 60 * 1000; // Інтервал перелогування Розетка(час життя токену)
        private const int intervalChekRozetka = 2 * 60 * 1000; // Інтервал зачитки повідомлень Розетка
        private const int intervalChekProm = 2 * 60 * 1000; // Інтервал зачитки повідомлень Prom

        public HandlerManager()
        {
            Logger.Info($"Start HandlerManager...");
            if (Current == null)
                Current = this;

            _rozetkaApi = new RozetkaAPI.ApiManager();
            RozetkaLogin(null);

            _promApi = new PromAPI.ApiManager();

            SessionManager.Current.NewSessionConnected += OnNewSessionConnected;
            SessionManager.Current.SessionClosed += OnSessionClosed;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-Viber-Auth-Token", Program.authToken);

            _timerClicks = new Timer(new TimerCallback(SendMainMenu), null, intervalChek, intervalChek);
            _timerRozetkaLogin = new Timer(new TimerCallback(RozetkaLogin), null, intervalLoginRozetka, intervalLoginRozetka);
            _timerRozetka = new Timer(new TimerCallback(ChekMessageRozetka), null, intervalChekRozetka, intervalChekRozetka);
            _timerProm = new Timer(new TimerCallback(ChekMessageProm), null, intervalChekProm, intervalChekProm);
        }

        public void OnNewSessionConnected(object sender, SessionEventArgs e)
        {
            Session session = e.Session;
            if (session != null)
            {
                session.MessageReceive += SessionOnMessageReceive;
            }
        }

        public async void OnSessionClosed(object sender, SessionEventArgs e)
        {
            Session session = e.Session;
            if (session != null)
            {
                session.MessageReceive -= SessionOnMessageReceive;

                var oper = UserManager.Current.FindUserArsenium(session.SessionID);
                if (oper != null && oper.Online)
                {
                    oper.Online = false;
                    Logger.Info($"Logout {oper.Login}; Connection loss");
                    await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ArseniumOfflineRequest(oper), oper);
                }
            }
        }

        private async void SessionOnMessageReceive(object sender, SessionMessageEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageTypes.PingRequest:
                    {
                        var message = JsonConvert.DeserializeObject<PingRequest>(e.MessageJSON);
                        var response = new PingResponse(message);
                        Logger.Info($"Ping. Id: {message.Id}, Time: {message.Time}");
                        //Logger.Debug($"Потік: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                        e.Session.Send(response);
                    }
                    break;
                case MessageTypes.LoginRequest:
                    {
                        var message = JsonConvert.DeserializeObject<LoginRequest>(e.MessageJSON);
                        var oper = UserManager.Current.FindUserArsenium(message.Login);
                        if (oper != null && oper.Active && (oper.Password == message.Password || oper.Id == Guid.Empty))
                        {
                            oper.Online = true;
                            if (oper.Id == Guid.Empty)
                            {
                                oper.Id = Guid.NewGuid();
                                oper.Password = message.Password;
                                DataProvider.Current.SaveOperatorSQL(oper);
                            }
                            e.Session.SessionID = oper.Id;
                            var response = new LoginResponse(message, true, oper, UserManager.Current.GetOnlineOperator(), UserManager.Current.GetNightClients(), UserManager.Current.GetLastClients(oper, 7, oper.Permission.IsRole(Permissions.PermissionRole.p_SeeAllUsers)));
                            await UserManager.Current.DeleteAllNightClientsOperator(oper);
                            Logger.Info($"Login {oper.Login}; success - true");
                            e.Session.Send(response);
                            await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ArseniumOnlineRequest(oper), oper);
                        }
                        else
                        {
                            var response = new LoginResponse(message, false, null, null, null, null);
                            Logger.Info($"Login {oper?.Login}; success - false");
                            e.Session.Send(response);
                        }
                    }
                    break;
                case MessageTypes.LogoutRequest:
                    {
                        var message = JsonConvert.DeserializeObject<LogoutRequest>(e.MessageJSON);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        oper.Online = false;
                        Logger.Info($"Logout {oper.Login}");
                        await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ArseniumOfflineRequest(oper), oper);
                    }
                    break;
                case MessageTypes.ReConnectRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ReConnectRequest>(e.MessageJSON);
                        e.Session.SessionID = message.ClientId;
                    }
                    break;
                case MessageTypes.AwayRequest:
                    {
                        var message = JsonConvert.DeserializeObject<AwayRequest>(e.MessageJSON);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        oper.Online = false;
                        await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ArseniumOfflineRequest(oper), oper);
                    }
                    break;
                case MessageTypes.ReturnAwayRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ReturnAwayRequest>(e.MessageJSON);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        oper.Online = true;
                        await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ArseniumOnlineRequest(oper), oper);
                    }
                    break;
                case MessageTypes.FindOperatorResponse:
                    {
                        var message = JsonConvert.DeserializeObject<FindOperatorResponse>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        var success = await Conversation.JoinedOperator(userViber, oper);
                        Logger.Info($"Connect operator to client - {oper.Name}, ChatId - {userViber.idViber}, Успіх - {success}");

                        //if (userViber.operatoId == Guid.Empty)
                        //    await UserManager.Current.AttachOperator(userViber, oper);
                        //await UserManager.Current.SendToAllOperatorsAsync(new ClientBusyRequest(userViber));
                        //await AddAndSendMessageAsync(userViber, $"Під'єднався оператор {oper.Name}", ChatMessageTypes.Service);
                        //Logger.Info($"Connect operator - {oper.Name}, ChatId - {userViber.idViber}");
                        //await SendMessageAsync(userViber.idViber, "АРС-бот", $"Під'єднався оператор {oper.Name}");
                        //await SendMessageAsync(userViber.idViber, oper.Name, "Що бажаєте? Напишіть 👇🏻", oper.Avatar);
                        //await SendClearKeyboardAsync(userViber.idViber);
                        //await SendKeyboardMessageAsync(userViber.idViber, MessageSend.MessageEndСonversation());
                    }
                    break;
                case MessageTypes.MessageToViberRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageToViberRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        Logger.Info($"MessageToViber. From: {oper.Name}, To: {userViber.idViber}, Text: {message.Message.Text}");
                        await Conversation.OperatorSendMessage(userViber, oper, message.Message);
                        e.Session.Send(new MessageToViberResponse(message, userViber, message.Message));
                    }
                    break;
                case MessageTypes.FileToViberRequest:
                    {
                        var message = JsonConvert.DeserializeObject<FileToViberRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        Logger.Info($"FileToViber. From: {oper.Name}, To: {userViber.idViber}, File: {message.Message.Text}");
                        await Conversation.OperatorSendMessage(userViber, oper, message.Message);
                        e.Session.Send(new FileToViberResponse(message, userViber, message.Message));
                    }
                    break;
                case MessageTypes.ImageToViberRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ImageToViberRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        Logger.Info($"ImageToViber. From: {oper.Name}, To: {userViber.idViber}, File: {message.Message.Text}");
                        await Conversation.OperatorSendMessage(userViber, oper, message.Message);
                        e.Session.Send(new ImageToViberResponse(message, userViber, message.Message));
                    }
                    break;
                case MessageTypes.UserListRequest:
                    {
                        var message = JsonConvert.DeserializeObject<UserListRequest>(e.MessageJSON);
                        var response = new UserListResponse(message, new List<User>());
                        e.Session.Send(response);
                    }
                    break;
                case MessageTypes.UserDetailsRequest:
                    {
                        var message = JsonConvert.DeserializeObject<UserDetailsRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        await GetUserDetailsAsync(userViber);
                        var response = new UserDetailsResponse(message, message.User, userViber);
                        e.Session.Send(response);
                    }
                    break;
                case MessageTypes.ChangeTypeRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ChangeTypeRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        if (userViber.inviteType != message.InviteType)
                        {
                            userViber.inviteType = message.InviteType;
                            DataProvider.Current.SetInviteTypeSQL(userViber.idViber, (int)userViber.inviteType);
                        }
                    }
                    break;
                case MessageTypes.ChangeOperatorRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ChangeOperatorRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        await UserManager.Current.AttachOperator(userViber, oper);
                    }
                    break;
                case MessageTypes.FindUserRequest:
                    {
                        var message = JsonConvert.DeserializeObject<FindUserRequest>(e.MessageJSON);
                        var userViber = message.Phone == null
                            ? UserManager.Current.FindAndBdUserViber(message.Guid)
                            : UserManager.Current.FindUserViberFromPhone(message.Phone);
                        var response = new FindUserResponse(message, userViber);
                        e.Session.Send(response);
                    }
                    break;
                case MessageTypes.FixMessageRequest:
                    {
                        var message = JsonConvert.DeserializeObject<FixMessageRequest>(e.MessageJSON);
                        var userViber = UserManager.Current.FindUserViber(message.User.Id);
                        if (message.Message == "EndСonversation")
                        {
                            await Conversation.EndOperator(userViber, true);
                        }
                        else if (message.Message == "MainMenu")
                        {
                            await Conversation.EndOperator(userViber, false);
                        }
                    }
                    break;
                case MessageTypes.PoolsListRequest:
                    {
                        var message = JsonConvert.DeserializeObject<PoolsListRequest>(e.MessageJSON);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        if (oper.Permission.IsRole(Permissions.PermissionRole.p_Pool))
                        {
                            var poolList = DataProvider.Current.GetPoolsSQL();
                            var response = new PoolsListResponse(message, "", poolList);
                            e.Session.Send(response);
                        }
                        else
                        {
                            e.Session.Send(new PoolsListResponse(message, "У вас немає прав на цю дію."));
                        }
                    }
                    break;
                case MessageTypes.MessageToRozetkaRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageToRozetkaRequest>(e.MessageJSON);
                        var userRozetka = UserManager.Current.FindUserRozetka(message.User.Id);
                        if (userRozetka != null)
                        {
                            var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                            Logger.Info($"MessageToRozetka. From: {oper.Name}, To: {userRozetka.Name}, Text: {message.Text}");
                            _rozetkaApi.SetMessagesAnswer(userRozetka.chat_id, message.Text, userRozetka.user_id);
                        }
                    }
                    break;
                case MessageTypes.MessageToPromRequest:
                    {
                        var message = JsonConvert.DeserializeObject<MessageToPromRequest>(e.MessageJSON);
                        var userProm = UserManager.Current.FindUserProm(message.User.Id);
                        if (userProm != null)
                        {
                            var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                            Logger.Info($"MessageToProm. From: {oper.Name}, To: {userProm.Name}, Text: {message.Text}");
                            //if (userProm.isContext)
                            //    _promApi.SendMessage(userProm.room_ident, null, message.Text, out var error);
                            //else
                            _promApi.SendMessage(userProm.room_ident, userProm.user_ident, message.Text, out var error);
                        }
                    }
                    break;
                case MessageTypes.ReadHotRequest:
                    {
                        var message = JsonConvert.DeserializeObject<ReadHotRequest>(e.MessageJSON);
                        var user = UserManager.Current.FindUser(message.User.Id);
                        var oper = UserManager.Current.FindUserArsenium(e.Session.SessionID);
                        user.operatoId = oper.Id;
                        user.operatoName = oper.Name;
                        await UserManager.Current.SendToAllOperatorsWithoutIAsync(new ClientBusyRequest(user), oper);
                        switch (user.UserType)
                        {
                            case UserTypes.Rozetka:
                                var userRozetka = user as UserRozetka;
                                _rozetkaApi.OpenChat(userRozetka.chat_id);
                                break;
                            case UserTypes.Prom:
                                var userProm = user as UserProm;
                                foreach (var msg in userProm.messages.Where(m => m.status == "new").ToList())
                                {
                                    if (_promApi.MarkMessageRead(msg.id, msg.room_id, out string error))
                                        msg.status = "read";
                                    if (!String.IsNullOrEmpty(error))
                                        Logger.Error("Пром MarkMessageRead: " + error);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public async Task SendFindOperatorAsync(UserViber sender)
        {
            var message = new ChatMessage()
            {
                DateCreate = DateTime.Now,
                Owner = sender.Id,
                Text = "Пошук оператора...",
                ChatMessageType = ChatMessageTypes.FindOperator
            };
            DataProvider.Current.SaveViberMessagesSQL(message);
            await UserManager.Current.SendToOperatorsAsync(sender, new FindOperatorRequest(sender));
        }

        public async Task AddAndSendMessageAsync(UserViber sender, string text, ChatMessageTypes chatMessageType, bool sendOperator = true)
        {
            var message = new ChatMessage()
            {
                DateCreate = DateTime.Now,
                Owner = sender.Id,
                Text = text,
                ChatMessageType = chatMessageType,
            };
            DataProvider.Current.SaveViberMessagesSQL(message, sender);
            if (!chatMessageType.MsgPool())
                sender.messageList.Add(message);
            if (sendOperator)
                await UserManager.Current.SendToOperatorsAsync(sender, new MessageFromViberRequest(message, DateTime.Now, sender));
        }

        public async Task AddAndSendFileAsync(UserViber sender, string fileUrl, string fileName, ChatMessageTypes messageTypes)
        {
            var fileLocalName = Guid.NewGuid().ToString() + fileName;
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(fileUrl, Program.filesPath + fileLocalName);
            }

            var message = new ChatMessage()
            {
                DateCreate = DateTime.Now,
                Owner = sender.Id,
                Text = fileLocalName,
                ChatMessageType = messageTypes
            };
            DataProvider.Current.SaveViberMessagesSQL(message);
            sender.messageList.Add(message);
            await UserManager.Current.SendToOperatorsAsync(sender, new FileFromViberRequest(message, DateTime.Now, sender));
        }

        public async Task SendDeliveredMessageAsync(long token, DateTime date)
        {
            var user = UserManager.Current.FindMessage(token);
            if (user == null)
            {
                DataProvider.Current.DeliveredArseniumMessagesSQL(token, date);
            }
            else
            {
                var message = user.messageList.FirstOrDefault(msg => msg.Token == token);
                if (message != null)
                {
                    message.DateDelivered = date;
                    DataProvider.Current.DeliveredArseniumMessagesSQL(message.Token, date);
                }

                await UserManager.Current.SendToOperatorsAsync(user, new MessageDeliveredRequest(user, date, token));
            }
        }

        public async Task SendSeenMessageAsync(long token, DateTime date)
        {
            var user = UserManager.Current.FindMessage(token);
            if (user == null)
            {
                DataProvider.Current.SeenArseniumMessagesSQL(token, date);
            }
            else
            {
                var message = user.messageList.FirstOrDefault(msg => msg.Token == token);
                if (message != null)
                {
                    message.DateSeen = date;
                    DataProvider.Current.SeenArseniumMessagesSQL(message.Token, date);
                }

                await UserManager.Current.SendToOperatorsAsync(user, new MessageSeenRequest(user, date, token));
            }
        }

        public async Task<HttpResponseMessage> SendMessageAsync(string receiver, string senderName, string text, string icon = null)
        {
            if (!String.IsNullOrEmpty(icon))
                icon = "https://viber.ars.ua/" + icon;

            var message = new MessageSend()
            {
                receiver = receiver,
                sender = new Sender()
                {
                    name = senderName,
                    avatar = icon
                },
                type = "text",
                text = text
            };
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
            return response;
        }

        public async Task<HttpResponseMessage> SendLinkAsPictureToViberAsync(string receiver, string senderName, string picture, string icon = null)
        {
            if (!String.IsNullOrEmpty(icon))
                icon = "https://viber.ars.ua/" + icon;

            var message = new FileSend()
            {
                receiver = receiver,
                sender = new Sender()
                {
                    name = senderName,
                    avatar = icon
                },
                type = "picture",
                text = "",
                media = picture,
                thumbnail = "",
                size = 0,
                file_name = ""
            };
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
            return response;
        }

        public async Task<HttpResponseMessage> SendPictureToViberAsync(string receiver, string senderName, string fileName, string icon = null)
        {
            if (!String.IsNullOrEmpty(icon))
                icon = "https://viber.ars.ua/" + icon;

            var file = "https://viber.ars.ua/Files/" + fileName;

            var message = new FileSend()
            {
                receiver = receiver,
                sender = new Sender()
                {
                    name = senderName,
                    avatar = icon
                },
                type = "picture",
                text = "",
                media = file,
                thumbnail = "",
                size = 0,
                file_name = ""
            };
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
            return response;
        }

        public async Task<HttpResponseMessage> SendVideoToViberAsync(string receiver, string senderName, string fileName, string icon = null)
        {
            if (!String.IsNullOrEmpty(icon))
                icon = "https://viber.ars.ua/" + icon;

            var fileInfo = new FileInfo(Program.filesPath + fileName);
            var file = "https://viber.ars.ua/Files/" + fileName;

            var message = new FileSend()
            {
                receiver = receiver,
                sender = new Sender()
                {
                    name = senderName,
                    avatar = icon
                },
                type = "video",
                text = "",
                media = file,
                thumbnail = "",
                size = fileInfo.Length,
                file_name = ""
            };
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
            return response;
        }

        public async Task<HttpResponseMessage> SendFileToViberAsync(string receiver, string senderName, string fileName, string icon = null)
        {
            if (!String.IsNullOrEmpty(icon))
                icon = "https://viber.ars.ua/" + icon;

            var fileInfo = new FileInfo(Program.filesPath + fileName);
            var file = "https://viber.ars.ua/Files/" + fileName;

            var message = new FileSend()
            {
                receiver = receiver,
                sender = new Sender()
                {
                    name = senderName,
                    avatar = icon
                },
                type = "file",
                text = "",
                media = file,
                thumbnail = "",
                size = fileInfo.Length,
                file_name = fileName
            };
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
            return response;
        }

        public async Task SendMessageAndSaveAsync(string receiver, UserArsenium oper, ChatMessage message)
        {
            var response = await SendMessageAsync(receiver, oper.Name, message.Text, oper.Avatar);
            var responseBody = await response.Content.ReadAsStringAsync();
            var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
            if (messageResponse.status == 0)
            {
                message.Token = messageResponse.message_token;
                message.DateCreate = DateTime.Now;
                message.Owner = oper.Id;
                message.OwnerName = oper.Name;

                var userViber = UserManager.Current.FindUserViber(receiver);
                if (userViber != null)
                {
                    message.Receiver = userViber.Id;
                    userViber.messageList.Add(message);
                    DataProvider.Current.SaveArseniumMessagesSQL(message);
                }
            }
            else if (messageResponse.status == 6)
            {
                var userViber = UserManager.Current.FindUserViber(receiver);
                if (userViber != null)
                {
                    userViber.subscribed = false;
                    DataProvider.Current.SetSubscribedSQL(userViber.idViber, false);
                }
            }
        }

        public async Task SendFileAndSaveAsync(string receiver, UserArsenium oper, ChatMessage message)
        {
            HttpResponseMessage response;
            switch (message.ChatMessageType)
            {
                case ChatMessageTypes.LinkAsImageToViber:
                    response = await SendLinkAsPictureToViberAsync(receiver, oper.Name, message.Text, oper.Avatar);
                    break;
                case ChatMessageTypes.ImageToViber:
                    response = await SendPictureToViberAsync(receiver, oper.Name, message.Text, oper.Avatar);
                    break;
                case ChatMessageTypes.VideoToViber:
                    response = await SendVideoToViberAsync(receiver, oper.Name, message.Text, oper.Avatar);
                    break;
                case ChatMessageTypes.FileToViber:
                    response = await SendFileToViberAsync(receiver, oper.Name, message.Text, oper.Avatar);
                    break;
                default:
                    return;
            }
            var responseBody = await response.Content.ReadAsStringAsync();
            var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseBody);
            if (messageResponse.status == 0)
            {
                message.Token = messageResponse.message_token;
                message.DateCreate = DateTime.Now;
                message.Owner = oper.Id;
                message.OwnerName = oper.Name;

                var userViber = UserManager.Current.FindUserViber(receiver);
                if (userViber != null)
                {
                    message.Receiver = userViber.Id;
                    userViber.messageList.Add(message);
                    DataProvider.Current.SaveArseniumMessagesSQL(message);
                }
            }
            else if (messageResponse.status == 6)
            {
                var userViber = UserManager.Current.FindUserViber(receiver);
                if (userViber != null)
                {
                    userViber.subscribed = false;
                    DataProvider.Current.SetSubscribedSQL(userViber.idViber, false);
                }
            }
        }

        public async Task SendClearKeyboardAsync(string receiver)
        {
#warning Цей метод в майбутньому можна переробити, щоб менше повідомлень посилати клієнту, тому що цей метод шле "додаткове повідомлення", яке в майбутньому може бути платне
            var message = new MessageSend()
            {
                receiver = receiver,
                type = "text",
                keyboard = new Keyboard()
                {
                    Type = "keyboard",
                    InputFieldState = "regular",
                    Buttons = new List<Button>()
                }
            };

            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
        }

        public async Task SendKeyboardMessageAsync(string receiver, MessageSend message)
        {
            message.receiver = receiver;
            var requestJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var response = await _httpClient.PostAsync(sendMessagePath, new StringContent(requestJson));
        }

        private async Task GetUserDetailsAsync(UserViber user)
        {
            var requestJson = $"{{\"id\":\"{user.idViber}\"}}";
            var response = await _httpClient.PostAsync(getUserDetails, new StringContent(requestJson));
            var responseBody = await response.Content.ReadAsStringAsync();
            var userDetails = JsonConvert.DeserializeObject<UserDetails>(responseBody);
            if (userDetails.status == 0)
            {
                user.Name = userDetails.user.name;
                user.Avatar = userDetails.user.avatar;
                user.language = userDetails.user.language;
                user.country = userDetails.user.country;
                user.primary_device_os = userDetails.user.primary_device_os;
                user.device_type = userDetails.user.device_type;
                user.subscribed = true;
                DataProvider.Current.UpdateClientSQL(user);
            }
            else if (userDetails.status == 6)
            {
                user.subscribed = false;
                DataProvider.Current.SetSubscribedSQL(user.idViber, false);
            }
        }

        public bool СheckMooClicks(string id, string action)
        {
            if (id == null) return false;
            var result = clicks.Exists(c => c.id == id && c.action == action && (DateTime.Now - c.dateTime).TotalMilliseconds < 1500);
            return result;
        }

        public string SetMenuClicks(string id, string action = null)
        {
            string result = null;
            if (id != null)
            {
                var click = clicks.FirstOrDefault(c => c.id == id);
                if (click != null)
                {
                    result = click.action;
                    click.action = action ?? "text";
                    click.dateTime = DateTime.Now;
                }
                else
                {
                    clicks.Add(new Click { id = id, action = action ?? "text", dateTime = DateTime.Now });
                }
            }
            return result;
        }

        public void ClearMenuClicks(string id)
        {
            if (id != null)
            {
                clicks.RemoveAll(c => c.id == id);
            }
        }

        private async void SendMainMenu(object obj)
        {
            var noActivity = clicks.Where(c => (DateTime.Now - c.dateTime).TotalMilliseconds > intervalClear).ToList();
            foreach (var click in noActivity)
            {
                var userViber = UserManager.Current.FindUserViber(click.id);
                if (userViber != null && userViber.inviteType != InviteType.Pool && userViber.inviteType != InviteType.Worker && userViber.inviteType != InviteType.WorkerDriver)
                    await Conversation.EndClient(userViber, true);
                click.action = "no activity";
            }
            clicks.RemoveAll(c => c.action == "no activity");
        }

        private void RozetkaLogin(object obj)
        {
            var rozetkaLogin = _rozetkaApi.Login();
            if (rozetkaLogin == null || !rozetkaLogin.success)
            {
                Logger.Error("Помилка генерування токену на Розетці!");
            }
        }

        private async void ChekMessageRozetka(object obj)
        {
            if (!UserManager.Current.IsOnlineOperator()) return;

            var count = _rozetkaApi.GetMessagesCount();
            if (!count.success || (count.content.totalUnreadMessages == 0 && count.content.totalUnread == 0)) return;
            List<Chat> allchat = new List<Chat>();
            if (count.content.ordersChatUnread != 0 || count.content.orderMessagesUnread != 0)
            {
                var chats = ApiManager.Current.GetMessagesOrder("orders")?.content.chats;
                if (chats != null)
                {
                    allchat.AddRange(chats);
                }
            }
            if (count.content.itemsChatUnread != 0 || count.content.sellerChatUnread != 0)
            {
                var chats = ApiManager.Current.GetMessagesOrder("items")?.content.chats;
                if (chats != null)
                {
                    allchat.AddRange(chats);
                }
            }
            foreach (var chat in allchat)
            {
                foreach(var message in chat.messages)
                {
                    if (message.read == null)
                        Logger.Info($"MessageFromRozetka. Client: {chat.user.contact_fio}, Чат: {chat.id}, Text: {message.body}");
                }
                if (chat.type == RozetkaAPI.ModelsRozetka.ChatType.Item && chat.item_id != null)
                {
                    var item = ApiManager.Current.GetItem(chat.item_id.Value);
                    if (Int32.TryParse(item?.article, out var codetvun))
                        chat.item_id = codetvun;
                    else
                        chat.item_id = null;
                }
                var user = new UserRozetka()
                {
                    UserType = UserTypes.Rozetka,
                    Id = Guid.NewGuid(),
                    Name = chat.user.contact_fio,
                    chat_id = chat.id,
                    user_id = chat.user.id
                };
                user = UserManager.Current.AddOrFindUserRozetka(user);
                if (user.operatoId == Guid.Empty)
                {
                    await UserManager.Current.SendToAllOperatorsAsync(new NewMessageRozetkaRequest(user, chat));
                }
                else
                {
                    var oper = UserManager.Current.FindUserArsenium(user.operatoId);
                    await UserManager.Current.SendToAllOperatorsAsync(new NewMessageRozetkaRequest(user, chat));
                }
            }
        }

        private async void ChekMessageProm(object obj)
        {
            if (!UserManager.Current.IsOnlineOperator()) return;
            var messages = _promApi.GetChatMessages(null, out string error);
            if (!String.IsNullOrEmpty(error))
            {
                Logger.Error("Пром GetChatMessages(null): " + error);
                if (error == "Timeout error on handling request")
                {
                    messages = _promApi.GetChatMessages(null, out error);
                    if (!String.IsNullOrEmpty(error))
                    {
                        Logger.Error("Пром GetChatMessages(null повторно): " + error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            if (messages == null || messages.Count == 0) return;
            //"прочитання" пустих і старих повідомлень:
            foreach (var msg in messages.Where(m => !m.is_sender && (m.body == null || m.date_sent < DateTime.Now.AddDays(-3))).ToList())
            {
                if (_promApi.MarkMessageRead(msg.id, msg.room_id, out error))
                    msg.status = "read";
                if (!String.IsNullOrEmpty(error))
                    Logger.Error("Пром MarkMessageRead: " + error);
            }
            //--------------------------------
            messages.RemoveAll(m => m.date_sent < DateTime.Now.AddDays(-3));
            if (messages.Count == 0) return;
            var rooms = messages.GroupBy(m => m.room_ident).Select(g => g.First().room_ident).ToList();
            foreach (var room in rooms)
            {
                var messagesUser = _promApi.GetChatMessages(room, out error);
                if (!String.IsNullOrEmpty(error))
                    Logger.Error("Пром GetChatMessages(room): " + error);
                if (messagesUser.Count == 0) break;
                foreach (var message in messagesUser)
                {
                    if (message.status == "new" && !message.is_sender)
                        Logger.Info($"MessageFromProm. Чат: {room}, Text: {message.body}");
                }
                var context = messagesUser.FirstOrDefault(m => m.type == "context" && !m.is_sender && m.context_item_type != "file") ?? messagesUser.FirstOrDefault(m => (!m.is_sender));
                if (context != null && context.context_item_type == "product" && context.context_item_id != null && context.context_item_id != 0)
                {
                    var product = _promApi.GetProduct(context.context_item_id.Value, out error);
                    if (product != null)
                    {
                        context.body = product.name;
                        if (Int64.TryParse(product.sku, out var codetvun))
                            context.context_item_id = codetvun;
                    }
                }
                var user = new UserProm()
                {
                    UserType = UserTypes.Prom,
                    Id = Guid.NewGuid(),
                    Name = context?.user_name ?? "Невідомо",
                    room_ident = room,
                    room_id = context?.room_id ?? messagesUser[0].room_id,
                    user_ident = context?.user_ident,
                    user_phone = context?.user_phone ?? "Невідомий",
                    messages = messagesUser
                };
                user = UserManager.Current.AddOrFindUserProm(user);
                if (user.operatoId == Guid.Empty)
                {
                    await UserManager.Current.SendToAllOperatorsAsync(new NewMessagePromRequest(user));
                }
                else
                {
                    var oper = UserManager.Current.FindUserArsenium(user.operatoId);
                    await UserManager.Current.SendToOperatorsAsync(oper, new NewMessagePromRequest(user));
                }
            }
        }

    }
}
