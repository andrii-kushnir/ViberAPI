using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using Models.Messages;
using Models.Messages.Requests;
using Models.Messages.Responses;

namespace Arsenium
{
    public partial class ClientSessionWin : Form
    {
        private const int TimeWidth = 28;
        private const int IconWidth = 18;
        private const int IconHeight = 18;
        private const int ScrollWidth = 19;
        private Size MaxSize;

        private int _oldWidth;
        private int _oldHeight;
        private int _posOnPanel = 0;

        private readonly Client _client;
        private readonly Dictionary<Guid, Control> _checkList = new Dictionary<Guid, Control>();

        public ClientSessionWin()
        {
            InitializeComponent();
            cbClientType.DataSource = Enum.GetValues(typeof(InviteType));
        }

        public ClientSessionWin(Client client) : this()
        {
            _client = client;
        }

        private void ClientSessionWin_Load(object sender, EventArgs e)
        {
            MaxSize = new Size(pTalk.Width - TimeWidth - IconWidth - ScrollWidth - 20, 0);

            _oldWidth = Width;
            _oldHeight = Height;

            if (Program.MainWin.MessageWinWidth != 0) Width = Program.MainWin.MessageWinWidth;
            if (Program.MainWin.MessageWinHeight != 0) Height = Program.MainWin.MessageWinHeight;

            int tempWidth = lbName.Width;
            lbName.Text = _client.Name;
            lbName.Location = new Point(lbName.Location.X - lbName.Width + tempWidth, lbName.Location.Y);

            tempWidth = lbPhone.Width;
            lbPhone.Text = _client.DetailsInfo.Phone ?? "Не надав телефону";
            lbPhone.Location = new Point(lbPhone.Location.X - lbPhone.Width + tempWidth, lbPhone.Location.Y);

            tempWidth = lbCountry.Width;
            lbCountry.Text = "Країна/мова: " + _client.DetailsInfo.Country + "/" + _client.DetailsInfo.Language;
            lbCountry.Location = new Point(lbCountry.Location.X - lbCountry.Width + tempWidth, lbCountry.Location.Y);

            tempWidth = lbDevice.Width;
            lbDevice.Text = "Девайс: " + _client.DetailsInfo.Device_type;
            lbDevice.Location = new Point(lbDevice.Location.X - lbDevice.Width + tempWidth, lbDevice.Location.Y);

            tempWidth = lbOS.Width;
            lbOS.Text = "OS: " + _client.DetailsInfo.Primary_device_os;
            lbOS.Location = new Point(lbOS.Location.X - lbOS.Width + tempWidth, lbOS.Location.Y);

            if (_client.DetailsInfo.Subscribed)
            {
                lbSubscribed.Visible = false;
                tempWidth = lbInvite.Width;
                switch (_client.DetailsInfo.InviteType)
                { 
                    case InviteType.DirectLink:
                        lbInvite.Text = "Прийшов по прямому посиланню";
                        break;
                    case InviteType.Pool:
                        lbInvite.Text = "Відгук про покупку товару";
                        break;
                    case InviteType.Site:
                        lbInvite.Text = "Прийшов з сайту";
                        break;
                    case InviteType.Buhnet:
                        lbInvite.Text = "Прийшов по запрошенню з Buhnet";
                        break;
                    case InviteType.Worker:
                        lbInvite.Text = "Запрошений, як працівник";
                        break;
                    case InviteType.Unknown:
                        lbInvite.Text = "Звідки прийшов невідомо";
                        break;
                }
                cbClientType.SelectedItem = _client.DetailsInfo.InviteType;
                lbInvite.Location = new Point(lbInvite.Location.X - lbInvite.Width + tempWidth, lbInvite.Location.Y);
            }
            else
            {
                lbInvite.Visible = false;
                tempWidth = lbSubscribed.Width;
                lbOS.Location = new Point(lbSubscribed.Location.X - lbSubscribed.Width + tempWidth, lbSubscribed.Location.Y);
            }

            pbPhoto.Image = _client.AvatarImage;
            lbOperator.Text = $"Оператор: {_client.DetailsInfo.OperatorName ?? "Немає прив'язки"}";
            lbClientBuhnet.Text = _client.DetailsInfo.BuhnetName ?? "Не знайдений в базі наших клієнтів";
            ShowMessages(_client);
            pTalk.AutoScrollPosition = new Point(0, pTalk.DisplayRectangle.Height);
        }

        public void ShowPhone(string phone)
        {
            lbPhone.Text = phone;
        }

        private void ShowMessages(Client client, bool all = false)
        {
            var msgCount = client.DetailsInfo.MessageList.Count;
            if (msgCount > 50 && !all)
            {
                var buttonAll = new Button()
                {
                    Parent = pTalk,
                    Text = "Показати усі повідомлення",
                    AutoSize = true
                };
                buttonAll.Location = new Point((pTalk.Width - buttonAll.Width) / 2, _posOnPanel - pTalk.VerticalScroll.Value);
                buttonAll.Click += (object sender, EventArgs e) =>
                {
                    pTalk.Controls.Clear();
                    ShowMessages(client, true);
                };
            }
            else
            {
                msgCount = 50;
            }
            var dateMessage = DateTime.MinValue.ToString("yyyy.MM.dd");
            foreach (var message in client.DetailsInfo.MessageList.Skip(msgCount - 50))
            {
                
                if (dateMessage != message.DateCreate.ToString("yyyy.MM.dd"))
                {
                    //ShowDateLabel(message.DateCreate);
                    Infrastructure.ShowDateLabel(pTalk, message.DateCreate, ref _posOnPanel);
                    dateMessage = message.DateCreate.ToString("yyyy.MM.dd");
                }

                if (message.Owner == client.Id)
                {
                    //message Client:
                    switch (message.ChatMessageType)
                    {
                        case ChatMessageTypes.MessageFromViber:
                            ShowMessageClient(message);
                            break;
                        case ChatMessageTypes.ImageFromViber:
                            ShowImageClient(message);
                            break;
                        case ChatMessageTypes.VideoFromViber:
                        case ChatMessageTypes.FileFromViber:
                            ShowFileClient(message);
                            break;
                        default:
                            ShowMessageClient(message);
                            break;
                    }
                }
                else
                {
                    //message Operator:
                    if (message.Owner != ClientManager.Myself.Id)
                        ShowOperatorName(message.OwnerName);

                    switch (message.ChatMessageType)
                    {
                        case ChatMessageTypes.MessageToViber:
                            ShowMessageOwn(message);
                            break;
                        case ChatMessageTypes.ImageToViber:
                            ShowMessageImageOwn(message);
                            break;
                        case ChatMessageTypes.VideoToViber:
                        case ChatMessageTypes.FileToViber:
                            ShowMessageFileOwn(message);
                            break;
                        case ChatMessageTypes.LinkAsImageToViber:
                            ShowMessageImageOwn(message, false);
                            break;
                        default:
                            ShowMessageOwn(message);
                            break;
                    }
                }
            }
        }

        private void label_Click(object sender, EventArgs e)
        {
            if (sender is Label label)
            {
                try { Clipboard.SetText(label.Text, TextDataFormat.UnicodeText); } catch { return; }
                string[] words = label.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string link = null;
                foreach (var word in words)
                {
                    if (!String.IsNullOrEmpty(word))
                    {
                        var start = word.IndexOf("http://");
                        if (start > -1)
                            link = word.Substring(start);

                        start = word.IndexOf("https://");
                        if (start > -1)
                            link = word.Substring(start);
                    }
                }
                if (!String.IsNullOrEmpty(link))
                {
                    System.Diagnostics.Process.Start(link);
                }
            } else if (sender is TextBox textBox)
            {
                string[] words = textBox.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string link = null;
                foreach (var word in words)
                {
                    if (!String.IsNullOrEmpty(word))
                    {
                        var start = word.IndexOf("http://");
                        if (start > -1)
                            link = word.Substring(start);

                        start = word.IndexOf("https://");
                        if (start > -1)
                            link = word.Substring(start);
                    }
                }
                if (!String.IsNullOrEmpty(link))
                {
                    System.Diagnostics.Process.Start(link);
                }
            }
            else if (sender is PictureBox pictureBox)
            {
                string link = pictureBox.ImageLocation;
                if (!String.IsNullOrEmpty(link))
                {
                    System.Diagnostics.Process.Start(link);
                }
            }

        }

        private void ClientSession_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                return;

            var width = _oldWidth - Width;
            var height = _oldHeight - Height;

            pInfo.Width -= width;
            pMessages.Top += height;
            pMessages.Height -= height;
            pMessages.Width -= width;
            tbSendMessage.Width -= width;

            //btFix1.Top -= height;
            //btFix2.Top -= height;
            //btFix3.Top -= height;

            _oldWidth = Width;
            _oldHeight = Height;

            Program.MainWin.MessageWinWidth = Width;
            Program.MainWin.MessageWinHeight = Height;
        }

        private void tbSendMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && (!string.IsNullOrWhiteSpace(tbSendMessage.Text) || pbClipboardImage.Visible))
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void tbSendMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                if (Clipboard.ContainsImage())
                {
                    pbClipboardImage.Visible = true;
                    pbClipboardImage.SizeMode = PictureBoxSizeMode.Zoom;
                    pbClipboardImage.Image = Clipboard.GetImage();
                }
        }

        private void btClearInput_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            tbSendMessage.Text = "";
            pbClipboardImage.Visible = false;
        }

        private void bSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSendMessage.Text) || pbClipboardImage.Visible)
            {
                SendMessage();
                ActiveControl = tbSendMessage;
            }
        }

        private void SendMessage()
        {
            if (pbClipboardImage.Visible)
            {
                var file = Path.GetTempPath() + Guid.NewGuid() + ".jpg";
                pbClipboardImage.Image.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);
                var fileInfo = new FileInfo(file);
                File.Copy(file, Program.Files + fileInfo.Name, true);
                var message = new ChatMessage()
                {
                    MessageId = Guid.NewGuid(),
                    DateCreate = DateTime.Now,
                    Owner = ClientManager.Myself.Id,
                    Text = fileInfo.Name,
                    Token = 0,
                    Receiver = _client.Id,
                    OwnerName = ClientManager.Myself.Name
                };
                Control labelIcon;
                message.ChatMessageType = ChatMessageTypes.ImageToViber;
                labelIcon = ShowMessageImageOwn(message);

                pTalk.ScrollControlIntoView(labelIcon);
                _client.DetailsInfo.MessageList.Add(message);
                var request = new FileToViberRequest(message, new User(_client.Id));
                _checkList.Add(message.MessageId, labelIcon);
                Program.Session.Send(request);

                pbClipboardImage.Visible = false;
            }
            else
            {
                AddMyMessage(tbSendMessage.Text);
            }
            tbSendMessage.Text = "";
        }

        private void AddMyMessage(string text)
        {
            var message = new ChatMessage() 
            {
                MessageId = Guid.NewGuid(), 
                DateCreate = DateTime.Now, 
                Owner = ClientManager.Myself.Id, 
                Text = text, 
                Token = 0, 
                ChatMessageType = ChatMessageTypes.MessageToViber, 
                Receiver = _client.Id,
                OwnerName = ClientManager.Myself.Name
            };
            var labelIcon = ShowMessageOwn(message);
            pTalk.ScrollControlIntoView(labelIcon);
            _client.DetailsInfo.MessageList.Add(message);
            var request = new MessageToViberRequest(message, new User(_client.Id));
            _checkList.Add(message.MessageId, labelIcon);
            Program.Session.Send(request);
        }

        //private void ShowDateLabel(DateTime date)
        //{
        //    var label = new Label()
        //    {
        //        Parent = pTalk,
        //        Text = $"  {date.Day} {MonthNames[date.Month - 1]} {date.Year}  ",
        //        Font = new Font("Times New Roman", 9.75F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(204))),
        //        ForeColor = Color.Black,
        //        BackColor = Color.Plum,
        //        AutoSize = true
        //    };
        //    label.Location = new Point((pTalk.Width - label.Width) / 2, _posOnPanel - pTalk.VerticalScroll.Value);
        //    Infrastructure.SetRoundedShape(label);
        //    _posOnPanel += label.Height + 1;
        //}

        private Control ShowMessageOwn(ChatMessage message)
        {
            //var labelMessage = new Label()
            //{
            //    Parent = pTalk,
            //    Text = message.Text,
            //    Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204))),
            //    ForeColor = Color.Black,
            //    BackColor = Color.LightGreen,
            //    MaximumSize = new Size(pTalk.Width - TimeWidth - IconWidth - ScrollWidth - 20, 0),
            //    AutoSize = true
            //};
            var labelMessage = new TextBox()
            {
                Parent = pTalk,
                Text = message.Text,
                Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = Color.Black,
                BackColor = Color.LightGreen,
                MaximumSize = MaxSize,
                AutoSize = true,
                ReadOnly = true,
                BorderStyle = 0,
                TabStop = false,
                Multiline = true,
                WordWrap = true
            };

            Size size = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font, MaxSize, TextFormatFlags.WordBreak);
            labelMessage.ClientSize = new Size(size.Width, size.Height);
            labelMessage.Location = new Point(pTalk.Width - labelMessage.Width - TimeWidth - IconWidth - ScrollWidth - 7, _posOnPanel - pTalk.VerticalScroll.Value);
            Infrastructure.SetRoundedShape(labelMessage);
            var time = Infrastructure.ShowMessageTime(pTalk, labelMessage, message.DateCreate);
            var labelIcon = ShowMessageIcon(time, message);
            labelMessage.Click += new System.EventHandler(this.label_Click);
            _posOnPanel += labelMessage.Height + 1;
            return labelIcon;
        }

        private Control ShowMessageFileOwn(ChatMessage message, bool own = true)
        {
            var sizeMax = new Size(pTalk.Width - TimeWidth - IconWidth - ScrollWidth - 20, 0);
            var labelMessage = new TextBox()
            {
                Parent = pTalk,
                Text = own ? "Відіслано файл: https://viber.ars.ua/Files/" + message.Text : "Відіслано файл як зображення: " + message.Text,
                Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = Color.Black,
                BackColor = Color.LightGreen,
                MaximumSize = MaxSize,
                AutoSize = true,
                ReadOnly = true,
                BorderStyle = 0,
                TabStop = false,
                Multiline = true,
                WordWrap = true,
            };
            Size size = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font, MaxSize, TextFormatFlags.WordBreak);
            labelMessage.ClientSize = new Size(size.Width, size.Height);

            labelMessage.Location = new Point(pTalk.Width - labelMessage.Width - TimeWidth - IconWidth - ScrollWidth - 7, _posOnPanel - pTalk.VerticalScroll.Value);
            Infrastructure.SetRoundedShape(labelMessage);
            var time = Infrastructure.ShowMessageTime(pTalk, labelMessage, message.DateCreate);
            var labelIcon = ShowMessageIcon(time, message);
            labelMessage.Click += new System.EventHandler(this.label_Click);
            _posOnPanel += labelMessage.Height + 1;
            return labelIcon; 
        }

        private Control ShowMessageImageOwn(ChatMessage message, bool own = true)
        {
            var imageMessage = new PictureBox()
            {
                Parent = pTalk,
                SizeMode = PictureBoxSizeMode.Zoom,
                MaximumSize = new Size(300, 300),
                BorderStyle = 0,
                TabStop = false
            };
            try
            {
                imageMessage.Load("https://viber.ars.ua/Files/" + message.Text);
            }
            catch
            {
                imageMessage.Load(@"NotNet.png");
            }
            imageMessage.Size = imageMessage.Image.Size;
            imageMessage.Location = new Point(pTalk.Width - imageMessage.Width - TimeWidth - IconWidth - ScrollWidth - 7, _posOnPanel - pTalk.VerticalScroll.Value);
            var time = Infrastructure.ShowMessageTime(pTalk, imageMessage, message.DateCreate);
            var labelIcon = ShowMessageIcon(time, message);
            imageMessage.Click += new System.EventHandler(this.label_Click);
            _posOnPanel += imageMessage.Height + 1;
            return labelIcon;
        }

        private void ShowOperatorName(string oper)
        {
            var labelMessage = new Label()
            {
                Parent = pTalk,
                Text = oper + ":",
                Font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = Color.Black,
                BackColor = Color.White,
                MaximumSize = new Size(pTalk.Width - TimeWidth - IconWidth - ScrollWidth - 20, 0),
                AutoSize = true
            };
            labelMessage.Location = new Point(pTalk.Width - labelMessage.Width - ScrollWidth - 1, _posOnPanel - pTalk.VerticalScroll.Value);
            Infrastructure.SetRoundedShape(labelMessage);
            _posOnPanel += labelMessage.Height + 1;
        }

        public void ShowMessageClient(ChatMessage message)
        {
            Font font;
            Color backColor;
            message.Text = Regex.Replace(message.Text, "\t", " ").Replace("\r\n", "\n").Replace("\n", Environment.NewLine); ;
            if (message.ChatMessageType.MsgServices())
            {
                font = new Font("Times New Roman", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
                backColor = Color.White;
            }
            else
            {
                font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
                backColor = Color.SkyBlue;
            }
            //var labelMessage = new Label()
            //{
            //    Parent = pTalk,
            //    Text = message.Text,
            //    Font = font,
            //    ForeColor = Color.Black,
            //    BackColor = backColor,
            //    Location = new Point(0, _posOnPanel - pTalk.VerticalScroll.Value),
            //    MaximumSize = new Size(pTalk.Width - TimeWidth - IconWidth - ScrollWidth - 20, 0),
            //    AutoSize = true
            //};
            var labelMessage = new TextBox()
            {
                Parent = pTalk,
                Text = message.Text,
                Font = font,
                ForeColor = Color.Black,
                BackColor = backColor,
                Location = new Point(0, _posOnPanel - pTalk.VerticalScroll.Value),
                MaximumSize = MaxSize,
                AutoSize = true,
                ReadOnly = true,
                BorderStyle = 0,
                TabStop = false,
                Multiline = true,
                WordWrap = true,
            };
            Size size = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font, MaxSize, TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
            labelMessage.ClientSize = new Size(size.Width, size.Height);
            Infrastructure.SetRoundedShape(labelMessage);
            Infrastructure.ShowMessageTime(pTalk, labelMessage, message.DateCreate);
            pTalk.ScrollControlIntoView(labelMessage);
            labelMessage.Click += new System.EventHandler(this.label_Click);
            _posOnPanel += labelMessage.Height + 1;
        }

        public void ShowFileClient(ChatMessage message)
        {
            var font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            var backColor = Color.SkyBlue;
            string msg = null;
            switch (message.ChatMessageType)
            {
                //case ChatMessageTypes.ImageFromViber:
                //    msg = "Отримано зображення: " + Path.GetTempPath() + message.Text.Substring(36);
                //    break;
                case ChatMessageTypes.VideoFromViber:
                    msg = "Отримано відео: " + Path.GetTempPath() + message.Text.Substring(36);
                    break;
                case ChatMessageTypes.FileFromViber:
                    msg = "Отримано файл: " + Path.GetTempPath() + message.Text.Substring(36);
                    break;
            }
            var labelMessage = new TextBox()
            {
                Parent = pTalk,
                Text = msg,
                Font = font,
                ForeColor = Color.Black,
                BackColor = backColor,
                Location = new Point(0, _posOnPanel - pTalk.VerticalScroll.Value),
                MaximumSize = MaxSize,
                AutoSize = true,
                ReadOnly = true,
                BorderStyle = 0,
                TabStop = false,
                Multiline = true,
                WordWrap = true,
            };
            Size size = TextRenderer.MeasureText(labelMessage.Text, labelMessage.Font, MaxSize, TextFormatFlags.WordBreak);
            labelMessage.ClientSize = new Size(size.Width, size.Height);

            Infrastructure.SetRoundedShape(labelMessage);
            Infrastructure.ShowMessageTime(pTalk, labelMessage, message.DateCreate);
            pTalk.ScrollControlIntoView(labelMessage);
            labelMessage.Click += (object sender, EventArgs e) =>
            {
                File.Copy(Program.Files + message.Text, Path.GetTempPath() + message.Text.Substring(36), true);
                System.Diagnostics.Process.Start(Path.GetTempPath() + message.Text.Substring(36));
            };
            _posOnPanel += labelMessage.Height + 1;
        }

        public void ShowImageClient(ChatMessage message)
        {
            var fileImage = Path.GetTempPath() + message.Text.Substring(36);
            if (!File.Exists(fileImage))
                File.Copy(Program.Files + message.Text, Path.GetTempPath() + message.Text.Substring(36));
            Image image;
            try
            {
                image = Image.FromFile(fileImage, true);
            }
            catch
            {
                image = Image.FromFile(@"NotNet.png");
            }
            var imageMessage = new PictureBox()
            {
                Parent = pTalk,
                Image = image,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(0, _posOnPanel - pTalk.VerticalScroll.Value),
                Size = image.Size,
                MaximumSize = new Size(300, 300),
                BorderStyle = 0,
                TabStop = false
            };

            Infrastructure.ShowMessageTime(pTalk, imageMessage, message.DateCreate);
            pTalk.ScrollControlIntoView(imageMessage);
            imageMessage.Click += (object sender, EventArgs e) =>
            {
                System.Diagnostics.Process.Start(fileImage);
            };
            _posOnPanel += imageMessage.Height + 1;
        }

        public void ChangeIcon(Guid messageGuid)
        {
            if (_checkList.ContainsKey(messageGuid))
            {
                var label = _checkList[messageGuid];
                label.ForeColor = Color.White;
            }
        }

        public void ChangeOperator(string operatorName)
        {
            lbOperator.Text = $"Оператор: {operatorName ?? "Немає прив'язки"}";
        }

        public void ShowMessageDelivered(Guid messageGuid)
        {
            if (_checkList.ContainsKey(messageGuid))
            {
                var label = _checkList[messageGuid];
                label.ForeColor = Color.MediumPurple;
            }
        }

        public void ShowMessageSeen(Guid messageGuid)
        {
            if (_checkList.ContainsKey(messageGuid))
            {
                var label = _checkList[messageGuid];
                label.ForeColor = Color.Green;
                _checkList.Remove(messageGuid);
            }
        }

        //private Control ShowMessageTime(Control control, DateTime date)
        //{
        //    var labelTime = new Label()
        //    {
        //        Parent = pTalk,
        //        Text = date.ToString("HH:mm"),
        //        Font = new Font("Times New Roman", 6.75F, FontStyle.Italic, GraphicsUnit.Point, ((byte)(204))),
        //        ForeColor = Color.Black,
        //        BackColor = Color.White,
        //        Location = new Point(control.Right + 1, control.Bottom - TimeHeight),
        //        AutoSize = true
        //    };
        //    Infrastructure.SetRoundedShape(labelTime);
        //    return labelTime;
        //}

        private Control ShowMessageIcon(Control control, ChatMessage message)
        {
            var color = Color.Black;
            if (message.DateSeen != null && message.DateSeen > new DateTime(2000, 1, 1))
                color = Color.Green;
            else if (message.DateDelivered != null && message.DateDelivered > new DateTime(2000, 1, 1))
                color = Color.MediumPurple;
            else if (message.Token != 0)
                color = Color.White;
            var labelIcon = new Label()
            {
                Parent = pTalk,
                Text = "✔️",
                Font = new Font("Times New Roman", 11.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204))),
                ForeColor = color,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(control.Right + 1, control.Bottom - IconHeight),
                Size = new Size(IconWidth, IconHeight),
                AutoSize = true
            };
            return labelIcon;
        }

        private void btDetailsInfo_Click(object sender, EventArgs e)
        {
            Program.Session.Send(new UserDetailsRequest(new User(_client.Id, null, null, UserTypes.Viber)));
            Close();
        }

        private void ClientSessionWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            _checkList.Clear();
        }

        private void btFix1_Click(object sender, EventArgs e)
        {
            var request = new FixMessageRequest("MainMenu", new User(_client.Id));
            Program.Session.Send(request);
        }

        private void btFix2_Click(object sender, EventArgs e)
        {
            AddMyMessage(btFix2.Text);
        }

        private void btFix3_Click(object sender, EventArgs e)
        {
            var request = new FixMessageRequest("EndСonversation", new User(_client.Id));
            Program.Session.Send(request);
        }

        private void ClientSessionWin_Activated(object sender, EventArgs e)
        {
            if (_client.PopUpWindow != null && !_client.PopUpWindow.IsDisposed)
                _client.PopUpWindow.Close();

            if (_client.WaitOperator)
            {
                var response = new FindOperatorResponse(new User(_client.Id));
                Program.Session.Send(response);
                _client.WaitOperator = false;
            }
        }

        private void cbClientType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Enum.TryParse<InviteType>(cbClientType.SelectedValue.ToString(), out InviteType clientType);
            if (_client.DetailsInfo.InviteType != clientType)
            {
                if (MessageBox.Show($"Дійсно змінити тип клієнта на {clientType}?", "Зміна типу клієнта", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    _client.DetailsInfo.InviteType = clientType;
                    var tempWidth = lbInvite.Width;
                    switch (_client.DetailsInfo.InviteType)
                    {
                        case InviteType.DirectLink:
                            lbInvite.Text = "Прийшов по прямому посиланню";
                            break;
                        case InviteType.Pool:
                            lbInvite.Text = "Відгук про покупку товару";
                            break;
                        case InviteType.Site:
                            lbInvite.Text = "Прийшов з сайту";
                            break;
                        case InviteType.Buhnet:
                            lbInvite.Text = "Прийшов по запрошенню з Buhnet";
                            break;
                        case InviteType.Worker:
                            lbInvite.Text = "Запрошений, як працівник";
                            break;
                        case InviteType.Unknown:
                            lbInvite.Text = "Звідки прийшов невідомо";
                            break;
                    }
                    lbInvite.Location = new Point(lbInvite.Location.X - lbInvite.Width + tempWidth, lbInvite.Location.Y);
                    var request = new ChangeTypeRequest(new User(_client.Id), clientType);
                    Program.Session.Send(request);
                }
                else
                    cbClientType.SelectedItem = _client.DetailsInfo.InviteType;
            }
        }

        private void btOperator_Click(object sender, EventArgs e)
        {
            lbOperator.Text = String.IsNullOrWhiteSpace(ClientManager.Myself.Name) ? $"Оператор: Немає прив'язки" : $"Оператор: {ClientManager.Myself.Name}";
            _client.DetailsInfo.OperatorName = ClientManager.Myself.Name;
            var request = new ChangeOperatorRequest(new User(_client.Id));
            Program.Session.Send(request);
        }

        private void tbSendMessage_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(DataFormats.FileDrop);
            if (data != null)
            {
                var filens = data as string[];
                if (filens.Length > 0)
                {
                    foreach (var file in filens)
                    {
                        var fileInfo = new FileInfo(file);
                        var fileWithoutSpace = Regex.Replace(fileInfo.Name, " ", "");
                        File.Copy(file, Program.Files + fileWithoutSpace, true);
                        var message = new ChatMessage()
                        {
                            MessageId = Guid.NewGuid(),
                            DateCreate = DateTime.Now,
                            Owner = ClientManager.Myself.Id,
                            Text = fileWithoutSpace,
                            Token = 0,
                            Receiver = _client.Id,
                            OwnerName = ClientManager.Myself.Name
                        };
                        Control labelIcon;
                        switch (Path.GetExtension(file))
                        {
                            case ".jpg":
                            case ".jpeg":
                            case ".png":
                            case ".gif":
                                message.ChatMessageType = ChatMessageTypes.ImageToViber;
                                labelIcon = ShowMessageImageOwn(message);
                                break;
                            case ".mp4":
                                message.ChatMessageType = ChatMessageTypes.VideoToViber;
                                labelIcon = ShowMessageFileOwn(message);
                                break;
                            default:
                                message.ChatMessageType = ChatMessageTypes.FileToViber;
                                labelIcon = ShowMessageFileOwn(message);
                                break;
                        }
                        pTalk.ScrollControlIntoView(labelIcon);
                        _client.DetailsInfo.MessageList.Add(message);
                        var request = new FileToViberRequest(message, new User(_client.Id));
                        _checkList.Add(message.MessageId, labelIcon);
                        Program.Session.Send(request);
                    }
                }
            }
        }

        private void tbSendMessage_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void bSendImage_Click(object sender, EventArgs e)
        {
            var message = new ChatMessage()
            {
                MessageId = Guid.NewGuid(),
                DateCreate = DateTime.Now,
                Owner = ClientManager.Myself.Id,
                Text = tbSendMessage.Text,
                Token = 0,
                ChatMessageType = ChatMessageTypes.LinkAsImageToViber,
                Receiver = _client.Id,
                OwnerName = ClientManager.Myself.Name
            };
            var labelIcon = ShowMessageImageOwn(message, false);
            pTalk.ScrollControlIntoView(labelIcon);
            _client.DetailsInfo.MessageList.Add(message);
            var request = new ImageToViberRequest(message, new User(_client.Id));
            _checkList.Add(message.MessageId, labelIcon);
            Program.Session.Send(request);
            tbSendMessage.Text = "";
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Дякуємо за оплату! Передаю інформацію в бухгалтерію. Гарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Добрий день! Щодо Вашого замовлення на \"*****\". Зорієнтуйте, будь ласка, чи для вас воно ще актуальне, оскільки неоплачені резерви зберігаються лише 3 дні.\r\nВаш ars.ua(smiley)";
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Добрий день! Ваше замовлення знаходиться у відділенні Нової Пошти вже 5 днів.\nОтримайте, будь ласка, товар в точці видачі, інакше товар буде повернено відправнику через 2 дні.\r\nГарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Добрий день! Отримайте Ваше замовлення №***** за адресою *****.\r\nДо оплати готівкою $$$ грн.\r\nГарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Добрий день! Ви зробили замовлення на \"*****\" в інтернет-магазині АРС.\r\nНа жаль, ми не змогли з Вами зв'язатися для підтвердження замовлення. Перетелефонуйте, будь ласка, нам за номером 0973334300.\r\nГарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Для оформлення замовлення вкажіть, будь ласка, дані отримувача:\r\nПІБ;\r\nНомер телефону;\r\nМісто;\r\nЯким перевізником бажаєте отримати замовлення;\r\nНомер відділення, чи адресу доставки.";
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Бажаєте здійснити попередню оплату, чи накладеним платежем при отриманні?\r\nВрахуйте, що при накладеному платежі компанія - перевізник бере додаткову комісію.";
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Дякую! Отже ваше замовлення:\r\n******\r\nВсе вірно?";
        }

        private void toolStripMenuItem20_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Оформляю, очікуйте номер ТТН для відстеження.\r\nГарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Номер вашої декларації для відстеження: *****. Обов'язково перевіряйте цілісність замовлення та повноту комплектації в присутності працівника служби доставки.\r\nДякуємо за замовлення! Гарного дня! Ваш ars.ua(smiley)";
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Надсилайте товар за адресою:\r\nс.Біла(Тернопільський р - н) НП 1\r\nОтримувач: юридична особа ТОВ Торгова група \"АРС Кераміка\"\r\nЄДРПОУ: 32549732\r\nКонтактна особа: Маринюк Андрій Анатолійович 0638135464\r\nПереконайтеся, що на товарі відсутні ознаки використання та пошкодження, наявна повна комплектація товару(як на момент придбання), збережено його товарний вигляд та всі споживчі властивості, а також товарну упаковку.\r\nПісля відправки повідомте нам номер ТТН.";
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Для повернення коштів заповніть, будь ласка, заяву (зразок додаю), і надішліть нам електронний варіант(скан-копію, або фото). У заяві обов'язково вкажіть реквізити карти, а саме номер рахунку у форматі UA... (побачити можна у мобільному додатку Вашого банку), і також Ваш ІПН. Кошти будуть повернені Вам не пізніше 30 днів з моменту розгляду.";
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Доброго дня. Для повернення коштів заповніть, будь ласка, форму за посиланням — https://bit.ly/3orTmV0. Вкажіть номер замовлення 0000000 та суму поверення 00000000. Реквізити карти, а саме номер рахунку IBAN та ІПН можна побачити у мобільному додатку Вашого банку. Кошти будуть повернені Вам не пізніше 14 днів з моменту розгляду заяви.";
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "На жаль, ми не знаходимося на складі, тому не маємо змоги підібрати потрібний Вам товар. Весь асортимент доступної продукції Ви можете переглянути на нашому сайті ars.ua. Дякуємо за розуміння!";
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, при оплаті Ви обрали не інтернет магазин, а інший підрозділ, тому кошти нам не будуть зараховані. Передаю інформацію в бухгалтерію, щоб зробили перенесення грошей. Після цього Ваше замовлення буде передано на пакування.";
        }

        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, міжбанківське зарахування коштів може тривати до трьох робочих днів. Щойно кошти буде зараховано, замовлення одразу буде передано на пакування.";
        }

        private void toolStripMenuItem22_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, зарахування коштів банком відбувається тільки в робочі дні. Щойно кошти буде зараховано, одразу передаю замовлення на пакування.";
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, при оплати Ви допустили помилку в реквізитах, тому кошти до нас не надійдуть. Зверніться, будь ласка, до Вашого банку для уточнення реквізитів.";
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, Ваше замовлення вже пакується і буде відправлено найближчим часом. Очікуйте інформацію про відправку.";
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = $"{_client.NameShow}, по Вашому замовленні відбувається переміщення товару між нашими складами, це може зайняти до 5 робочих днів. Очікуйте інформацію про відправку.";
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "На сайті представлено інформацію про товар, яку ми отримуємо від постачальника. Але кожен виробник залишає за собою право змінювати характеристики та комплектацію без попереднього узгодження. Оскільки пакуванням і відправкою займається інший відділ, немає можливості звірити товар на сайті і в наявності.Саме тому ми закликаємо всіх покупців перевіряти своє замовлення при отриманні в присутності працівника служби доставки.";
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Добрий день! Чим можу допомогти?";
        }

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Чи можу ще чимось вам допомогти?";
        }

        private void toolStripMenuItem30_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Можливо у вас є інші запитання?";
        }

        private void btProforms_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Незабаром буде реалізовано");
        }

        private void btInfoBuhnet_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Незабаром буде реалізовано");
        }

        private void lbPhone_Click(object sender, EventArgs e)
        {
            try { Clipboard.SetText(lbPhone.Text.Substring(2), TextDataFormat.UnicodeText); } catch { return; }
        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Перевіряю інформацію, очікуйте будь ласка.";
        }

        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            tbSendMessage.Text = "Для уточнення всіх деталей та надання вам відповіді мені знадобиться більше часу, ніж зазвичай. Будь ласка, очікуйте, обов'язково вам повідомлю.";
        }
    }
}
