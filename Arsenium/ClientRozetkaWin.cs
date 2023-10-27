using Models;
using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatMessage = RozetkaAPI.ModelsRozetka.ChatMessage;

namespace Arsenium
{
    public partial class ClientRozetkaWin : Form
    {
        private const int TimeWidth = 28;
        private const int ScrollWidth = 19;
        private Size MaxSize;

        private int _posOnPanel = 0;

        private readonly Client _client;

        public ClientRozetkaWin()
        {
            InitializeComponent();
        }

        public ClientRozetkaWin(Client client) : this()
        {
            _client = client;
        }

        private void ClientRozetkaWin_Load(object sender, EventArgs e)
        {
            _lClient.Text = "Клієнт: " + _client.NameShow;
            _lChatType.Text = _client.ChatRozetka.subject;
            switch (_client.ChatRozetka.type)
            {
                case RozetkaAPI.ModelsRozetka.ChatType.Order:
                    _lOrder.Text = "Замовлення №" + _client.ChatRozetka.order_id ?? "Невідоме";
                    _lItem.Text = "";
                    break;
                case RozetkaAPI.ModelsRozetka.ChatType.Item:
                    _lOrder.Text = "";
                    _lItem.Text = "Код товару: " + _client.ChatRozetka.item_id ?? "Невідомий";
                    break;
                case RozetkaAPI.ModelsRozetka.ChatType.Seller:
                    _lOrder.Text = "";
                    _lItem.Text = "";
                    break;
            }
            this.Text = "Чат з клієнтом з Розетки: " + _client.NameShow;
            MaxSize = new Size(_pTalk.Width - TimeWidth - ScrollWidth - 20, 0);
            _client.ChatRozetka.messages = _client.ChatRozetka.messages.OrderBy(m => m.created).ToList();
            ShowMessages(_client);
            ActiveControl = _tbSendMessage;
        }

        private void ClientRozetkaWin_Activated(object sender, EventArgs e)
        {
            if (_client.PopUpRozetkaWindow != null && !_client.PopUpRozetkaWindow.IsDisposed)
                _client.PopUpRozetkaWindow.Close();

            if (_client.WaitOperator)
            {
                var request = new ReadHotRequest(new User(_client.Id, UserTypes.Rozetka));
                Program.Session.Send(request);
                _client.WaitOperator = false;
            }
        }

        private void ShowMessages(Client client)
        {
            var msgCount = client.ChatRozetka.messages.Count;
            var dateMessage = DateTime.MinValue.ToString("yyyy.MM.dd");
            foreach (var message in client.ChatRozetka.messages)
            {
                var dateCreated = DateTime.Parse(message.created);
                if (dateMessage != dateCreated.ToString("yyyy.MM.dd"))
                {
                    Infrastructure.ShowDateLabel(_pTalk, dateCreated, ref _posOnPanel);
                    dateMessage = dateCreated.ToString("yyyy.MM.dd");
                }

                switch (message.sender)
                {
                    case 3:
                        ShowMessageClient(message);
                        break;
                    case 2:
                        //вивід імені оператора, можливо в майбутньому буде:
                        //if (message.Owner != ClientManager.Myself.Id)
                        //    ShowOperatorName(message.OwnerName);
                        ShowMessageOwn(message);
                        break;
                    case 0:
                    case 1:
                        //Системні повідомлення, можливо в майбутньому виводитимуться
                        break;
                }
            }
        }

        public void RefreshMessages(RozetkaAPI.ModelsRozetka.Chat chat)
        {
            _pTalk.Controls.Clear();
            _posOnPanel = 0;
            _client.ChatRozetka.messages = chat.messages.OrderBy(m => m.created).ToList();
            ShowMessages(_client);
        }

        private Control ShowMessageOwn(ChatMessage message)
        {
            var labelMessage = new TextBox()
            {
                Parent = _pTalk,
                Text = message.body,
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
            labelMessage.Location = new Point(_pTalk.Width - labelMessage.Width - TimeWidth - ScrollWidth - 7, _posOnPanel - _pTalk.VerticalScroll.Value);
            Infrastructure.SetRoundedShape(labelMessage);
            var time = Infrastructure.ShowMessageTime(_pTalk, labelMessage, DateTime.Parse(message.created));
            _posOnPanel += labelMessage.Height + 1;
            return labelMessage;
        }

        public void ShowMessageClient(ChatMessage message)
        {
            //message.body = Regex.Replace(message.body, "\t", " ").Replace("\r\n", "\n").Replace("\n", Environment.NewLine); ;
            var font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
            var backColor = Color.SkyBlue;
            var labelMessage = new TextBox()
            {
                Parent = _pTalk,
                Text = message.body,
                Font = font,
                ForeColor = Color.Black,
                BackColor = backColor,
                Location = new Point(0, _posOnPanel - _pTalk.VerticalScroll.Value),
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
            Infrastructure.ShowMessageTime(_pTalk, labelMessage, DateTime.Parse(message.created));
            _pTalk.ScrollControlIntoView(labelMessage);
            _posOnPanel += labelMessage.Height + 1;
        }

        private void SendMessage()
        {
            AddMyMessage(_tbSendMessage.Text);
            _tbSendMessage.Text = "";
        }

        private void AddMyMessage(string text)
        {
            var message = new ChatMessage()
            {
                body = text,
                created = DateTime.Now.ToString(),
                sender = 2
            };
            var labelIcon = ShowMessageOwn(message);
            _pTalk.ScrollControlIntoView(labelIcon);
            _client.ChatRozetka.messages.Add(message);
            var request = new MessageToRozetkaRequest(new User(_client.Id, UserTypes.Rozetka), text);
            Program.Session.Send(request);
        }

        private void _bSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_tbSendMessage.Text))
            {
                SendMessage();
                ActiveControl = _tbSendMessage;
            }
        }

        private void _tbSendMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && (!string.IsNullOrWhiteSpace(_tbSendMessage.Text)))
            {
                SendMessage();
                e.Handled = true;
            }
        }
    }
}
