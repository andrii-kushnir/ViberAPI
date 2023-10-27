using Models;
using Models.Messages.Requests;
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
            _client.MessagesProm = _client.MessagesProm.OrderBy(m => m.date_sent).ToList();
            var context = _client.MessagesProm.FirstOrDefault(m => m.type == "context" && m.context_item_type != "file");
            if (context != null)
            {
                switch (context.context_item_type)
                {
                    case "product":
                        _lItem.Text = "Запитання про товар " + context.context_item_id.ToString();
                        if (context.context_item_image_url != null)
                            _pbContextImage.Image = context.context_item_image_url.GetIconFromWebImage();
                        break;
                    case "order":
                        _lItem.Text = "Запитання про замовлення " + context.context_item_id.ToString(); break;
                    default:
                        _lItem.Text = ""; break;
                }
            }
            else
            {
                _lItem.Text = "Запитання продавцеві";
                context = _client.MessagesProm.FirstOrDefault(m => !m.is_sender);
            }
            _lClient.Text = "Клієнт: " + context.user_name;
            _lPhone.Text = "Телефон: " + context.user_phone;
            this.Text = "Чат з клієнтом з Prom: " + _client.NameShow;
            MaxSize = new Size(_pTalk.Width - TimeWidth - ScrollWidth - 20, 0);
            ShowMessages(_client);
            ActiveControl = _tbSendMessage;
        }

        private void ClientPromWin_Activated(object sender, EventArgs e)
        {
            if (_client.PopUpPromWindow != null && !_client.PopUpPromWindow.IsDisposed)
                _client.PopUpPromWindow.Close();

            if (_client.WaitOperator)
            {
                var request = new ReadHotRequest(new User(_client.Id, UserTypes.Prom));
                Program.Session.Send(request);
                _client.WaitOperator = false;
            }
        }

        private void ShowMessages(Client client)
        {
            var msgCount = client.MessagesProm.Count;
            var dateMessage = DateTime.MinValue.ToString("yyyy.MM.dd");
            foreach (var message in client.MessagesProm)
            {
                if (dateMessage != message.date_sent.ToString("yyyy.MM.dd"))
                {
                    Infrastructure.ShowDateLabel(_pTalk, message.date_sent, ref _posOnPanel);
                    dateMessage = message.date_sent.ToString("yyyy.MM.dd");
                }

                if (message.is_sender)
                {
                    //наше повідомлення
                    switch (message.type)
                    {
                        case "notification":
                            ShowMessageOwn(message);
                            break;
                        case "message":
                            ShowMessageOwn(message);
                            break;
                        default:
                            if (!String.IsNullOrEmpty(message.body))
                                ShowMessageOwn(message);
                            break;
                    }
                }
                else
                {
                    //повідомлення клієнта
                    switch (message.type)
                    {
                        case "context":
                            //Це переданий файл(зображення):
                            if (message.context_item_type == "file")
                                ShowMessageImageClient(message);
                            break;
                        case "message":
                            ShowMessageClient(message);
                            break;
                        default:
                            if (!String.IsNullOrEmpty(message.body))
                            ShowMessageClient(message);
                            break;
                    }
                }
            }
        }

        public void RefreshMessages(List<PromAPI.ModelsProm.Message> chat)
        {
            _pTalk.Controls.Clear();
            _posOnPanel = 0;
            _client.MessagesProm = chat.OrderBy(m => m.date_sent).ToList();
            ShowMessages(_client);
        }

        private Control ShowMessageOwn(PromAPI.ModelsProm.Message message)
        {
            Font font;
            Color backColor;
            if (message.type == "notification")
            {
                font = new Font("Times New Roman", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
                backColor = Color.White;
            }
            else
            {
                font = new Font("Times New Roman", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(204)));
                backColor = Color.LightGreen;
            }
            var labelMessage = new TextBox()
            {
                Parent = _pTalk,
                Text = message.body,
                Font = font,
                ForeColor = Color.Black,
                BackColor = backColor,
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
            var time = Infrastructure.ShowMessageTime(_pTalk, labelMessage, message.date_sent.AddHours(3));
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
            Infrastructure.ShowMessageTime(_pTalk, labelMessage, message.date_sent.AddHours(3));
            _pTalk.ScrollControlIntoView(labelMessage);
            _posOnPanel += labelMessage.Height + 1;
        }

        private void ShowMessageImageClient(PromAPI.ModelsProm.Message message)
        {
            var imageMessage = new PictureBox()
            {
                Parent = _pTalk,
                SizeMode = PictureBoxSizeMode.Zoom,
                MaximumSize = new Size(300, 300),
                BorderStyle = 0,
                TabStop = false
            };
            try
            {
                imageMessage.Load(message.context_item_image_url);
            }
            catch
            {
                imageMessage.Load(@"NotNet.png");
            }
            imageMessage.Size = imageMessage.Image.Size;
            imageMessage.Location = new Point(_pTalk.Width - imageMessage.Width - TimeWidth - ScrollWidth - 7, _posOnPanel - _pTalk.VerticalScroll.Value);
            var time = Infrastructure.ShowMessageTime(_pTalk, imageMessage, message.date_sent);
            _posOnPanel += imageMessage.Height + 1;
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
                body = text,
                date_sent = DateTime.Now.AddHours(-3),
                is_sender = true,
                type = "message"
            };
            var labelIcon = ShowMessageOwn(message);
            _pTalk.ScrollControlIntoView(labelIcon);
            _client.MessagesProm.Add(message);
            var request = new MessageToPromRequest(new User(_client.Id, UserTypes.Prom), text);
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
