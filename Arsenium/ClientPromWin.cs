using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arsenium
{
    public partial class ClientPromWin : Form
    {
        private const int TimeWidth = 28;
        private const int ScrollWidth = 19;
        private Size MaxSize;

        private int _posOnPanel = 0;

        private readonly Client _client;

        public ClientPromWin()
        {
            InitializeComponent();
        }

        public ClientPromWin(Client client) : this()
        {
            _client = client;
        }

        private void ClientPromWin_Load(object sender, EventArgs e)
        {
            this.Text = "Чат з клієнтом з Prom: " + _client.NameShow;
            MaxSize = new Size(_pTalk.Width - TimeWidth - ScrollWidth - 20, 0);
            _client.MessagesProm = _client.MessagesProm.OrderBy(m => m.date_sent).ToList();
            ShowMessages(_client);
        }

        private void ShowMessages(Client client)
        {
            var msgCount = client.MessagesProm.Count;
            DateTime dateMessage = DateTime.MinValue;
            foreach (var message in client.MessagesProm)
            {
                if (dateMessage != message.date_sent)
                {
                    Infrastructure.ShowDateLabel(_pTalk, message.date_sent, ref _posOnPanel);
                    dateMessage = message.date_sent;
                }

                if (message.is_sender)
                {
                    //наше повідомлення
                    ShowMessageOwn(message);
                }
                else
                {
                    //повідомлення клієнта
                    ShowMessageClient(message);
                }
            }
        }

        public void RefreshMessages(List<PromAPI.ModelsProm.Message> chat)
        {
            _pTalk.Controls.Clear();
            _client.MessagesProm = chat.OrderBy(m => m.date_sent).ToList();
            ShowMessages(_client);
        }

        private Control ShowMessageOwn(PromAPI.ModelsProm.Message message)
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
            var time = Infrastructure.ShowMessageTime(_pTalk, labelMessage, message.date_sent);
            _posOnPanel += labelMessage.Height + 1;
            return labelMessage;
        }

        public void ShowMessageClient(PromAPI.ModelsProm.Message message)
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
            Infrastructure.ShowMessageTime(_pTalk, labelMessage, message.date_sent);
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
            var message = new PromAPI.ModelsProm.Message()
            {
                room_ident = _client.MessagesProm[0].room_ident,
                body = text
            };
            var labelIcon = ShowMessageOwn(message);
            _pTalk.ScrollControlIntoView(labelIcon);
            _client.MessagesProm.Add(message);
#warning Prom: оператор написав клієнту!!!
            //var request = new MessageToViberRequest(message, new User(_client.Id));
            //Program.Session.Send(request);
        }

        private void _bSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_tbSendMessage.Text))
            {
                SendMessage();
                ActiveControl = _tbSendMessage;
            }
        }
    }
}
