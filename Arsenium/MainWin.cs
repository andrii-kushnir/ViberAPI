using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Models;
using Models.Messages;
using Models.Messages.Requests;
using Models.Messages.Responses;
using Models.Network;
using Newtonsoft.Json;
using NLog;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using Microsoft.Win32;

namespace Arsenium
{
    public partial class MainWin : Form
    {

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

        private bool _awaiEnable = false;
        private int awaitClick;

        static Logger Logger = LogManager.GetCurrentClassLogger();

        private Image _imageDefault;
        private readonly ImageList _iconsList = new ImageList() { ImageSize = new Size(20, 20) };
        private readonly Timer _timerBlink = new Timer();

        public int MessageWinWidth = 0;
        public int MessageWinHeight = 0;

        public MainWin()
        {
            _imageDefault = Image.FromFile(@"MainFormIcon256.ico");
            InitializeComponent();
        }

        private void MessageShow(string str)
        {
            if (this.InvokeRequired)
            {
                this.SafeInvoke(MessageShow, str);
                return;
            }
            MessageBox.Show(str);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            _timerBlink.Interval = 400;
            _timerBlink.Tick += timer_Tick;
            _timerBlink.Start();

            this.treeMain.ImageList = _iconsList;
            _iconsList.Images.Add("Default", _imageDefault);
            _iconsList.Images.Add("Viber", "https://viber.ars.ua/free-icon-viber-3670059.png"?.GetIconFromWebImage() ?? _imageDefault);
            _iconsList.Images.Add("Rozetka", "https://viber.ars.ua/Rozetka.png"?.GetIconFromWebImage() ?? _imageDefault);
            _iconsList.Images.Add("Prom", "https://viber.ars.ua/Prom.png"?.GetIconFromWebImage() ?? _imageDefault);
            _iconsList.Images.Add("Ars", "https://viber.ars.ua/OldArs.png"?.GetIconFromWebImage() ?? _imageDefault);

            this.treeMain.Nodes.Add(new TreeNode("Клієнти(Вайбер)", _iconsList.Images.IndexOfKey("Viber"), _iconsList.Images.IndexOfKey("Viber")) { Name = "ViberClients" });
            this.treeMain.Nodes.Add(new TreeNode("Клієнти(Розетка)", _iconsList.Images.IndexOfKey("Rozetka"), _iconsList.Images.IndexOfKey("Rozetka")) { Name = "RozetkaClients" });
            this.treeMain.Nodes.Add(new TreeNode("Клієнти(Prom)", _iconsList.Images.IndexOfKey("Prom"), _iconsList.Images.IndexOfKey("Prom")) { Name = "PromClients" });
            this.treeMain.Nodes.Add(new TreeNode("Працівники(ARSenium)", _iconsList.Images.IndexOfKey("Ars"), _iconsList.Images.IndexOfKey("Ars")) { Name = "ARSeniumClients" });
            //this.treeMain.Nodes.Add(new TreeNode("Інші контакти", _iconsList.Images.IndexOfKey("Ars"), _iconsList.Images.IndexOfKey("Ars")) { Name = "OtherClients" });
            foreach (var client in ClientManager.GetAllClients().OrderBy(cl => (cl.DetailsInfo == null) ? DateTime.Now : cl.DetailsInfo.MessageList.Max(m => m.DateCreate)).ToList())
                AddClient(client);

            this.treeMain.ExpandAll();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (ClientManager.GetAllClients().Count(c => c.Blinking != BlinkType.Normal) != 0)
            {
                this.treeMain.BeginUpdate();
                foreach (var client in ClientManager.GetAllClients().Where(c => c.Blinking != BlinkType.Normal).ToList())
                {
                    if (client.Node.BackColor == Color.White)
                    {
                        if (client.Blinking == BlinkType.Hot)
                            client.Node.BackColor = Color.Red;
                        else if (client.Blinking == BlinkType.NotHot)
                            client.Node.BackColor = Color.Yellow;
                    }
                    else
                        client.Node.BackColor = Color.White;
                }
                this.treeMain.EndUpdate();
            }
        }

        public void AddClient(Client client)
        {
            string clientName;
            switch (client.Type)
            {
                case UserTypes.Viber:
                    _iconsList.Images.Add(client.Id.ToString(), client.AvatarImage ?? _imageDefault);
                    clientName = $"{client.NameShow} [{client.DetailsInfo.OperatorName}]";
                    client.Node = new TreeNode(clientName, _iconsList.Images.IndexOfKey(client.Id.ToString()), _iconsList.Images.IndexOfKey(client.Id.ToString())) { Tag = client, Name = client.Id.ToString(), BackColor = Color.White };
                    this.treeMain.Nodes.Find("ViberClients", false)[0].Nodes.Insert(0, client.Node);
                    break;
                case UserTypes.Asterium:
                    _iconsList.Images.Add(client.Id.ToString(), client.AvatarImage ?? _imageDefault);
                    clientName = $"{client.NameShow}";
                    client.Node = new TreeNode(clientName, _iconsList.Images.IndexOfKey(client.Id.ToString()), _iconsList.Images.IndexOfKey(client.Id.ToString())) { Tag = client, Name = client.Id.ToString(), BackColor = Color.White };
                    this.treeMain.Nodes.Find("ARSeniumClients", false)[0].Nodes.Add(client.Node);
                    break;
                case UserTypes.Rozetka:
                    clientName = $"{client.NameShow}";
                    client.Node = new TreeNode(clientName, _iconsList.Images.IndexOfKey("Rozetka"), _iconsList.Images.IndexOfKey("Rozetka")) { Tag = client, Name = client.Id.ToString(), BackColor = Color.White };
                    this.treeMain.Nodes.Find("RozetkaClients", false)[0].Nodes.Add(client.Node);
                    break;
                case UserTypes.Prom:
                    clientName = $"{client.NameShow}";
                    client.Node = new TreeNode(clientName, _iconsList.Images.IndexOfKey("Prom"), _iconsList.Images.IndexOfKey("Prom")) { Tag = client, Name = client.Id.ToString(), BackColor = Color.White };
                    this.treeMain.Nodes.Find("PromClients", false)[0].Nodes.Add(client.Node);
                    break;
                    //case UserTypes.Telegram:
                    //    this.treeMain.Nodes.Find("OtherClients", false)[0].Nodes.Add(client.Node);
                    //    break;
                    //case UserTypes.Unknown:
                    //    this.treeMain.Nodes.Find("OtherClients", false)[0].Nodes.Add(client.Node);
                    //    break;
            }
            this.treeMain.ExpandAll();
        }

        public void ClientToTop(Client client)
        {
            switch (client.Type)
            {
                case UserTypes.Viber:
                    this.treeMain.Nodes.Find("ViberClients", false)[0].Nodes.Remove(client.Node);
                    this.treeMain.Nodes.Find("ViberClients", false)[0].Nodes.Insert(0, client.Node);
                    break;
                //case UserTypes.Asterium:
                //    this.treeMain.Nodes.Find("ARSeniumClients", false)[0].Nodes.Insert(0, client.Node);
                //    break;
            }
        }

        public void RemoveClient(Client client)
        {
            //switch (client.Type)
            //{
            //    case UserTypes.Viber:
            //        this.treeMain.Nodes.Find("ViberClients", false)[0].Nodes.Remove(client.Node);
            //        break;
            //    case UserTypes.Asterium:
            //        this.treeMain.Nodes.Find("ARSeniumClients", false)[0].Nodes.Remove(client.Node);
            //        break;
            //    case UserTypes.Telegram:
            //        this.treeMain.Nodes.Find("OtherClients", false)[0].Nodes.Remove(client.Node);
            //        break;
            //    case UserTypes.Unknown:
            //        this.treeMain.Nodes.Find("OtherClients", false)[0].Nodes.Remove(client.Node);
            //        break;
            //}
            client.Node.Remove();
        }

        private void treeMain_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var node = e.Node;
            var client = (Client)node.Tag;
            if (client == null) return;
            switch (client.Type)
            {
                case UserTypes.Viber:
                    ClientManager.SetNodeUnblink(client);
                    if (client.DetailsInfo == null)
                    {
                        Program.Session.Send(new UserDetailsRequest(new User(client.Id, null, null, UserTypes.Viber)));
                        var i = 0;
                        while (client.DetailsInfo == null && i <= 3000)
                        {
                            System.Threading.Thread.Sleep(50);
                            i += 50;
                        }
                    }

                    if (client.ClientWindow == null || client.ClientWindow.IsDisposed)
                    {
                        if (client.DetailsInfo == null)
                        {
                            MessageBox.Show("Дані по контакту не можуть бути отримані.");
                            return;
                        }
                        client.ClientWindow = new ClientSessionWin(client) { Text = $"Далог з {client.NameShow}" };
                        client.ClientWindow.Show();
                    }
                    else
                    {
                        client.ClientWindow.Focus();
                    }
                    break;
                case UserTypes.Rozetka:
                    ClientManager.SetNodeUnblink(client);
                    if (client.RozetkaWindow == null || client.RozetkaWindow.IsDisposed)
                    {
                        client.RozetkaWindow = new ClientRozetkaWin(client);
                        client.RozetkaWindow.Show();
                    }
                    else
                    {
                        client.RozetkaWindow.Focus();
                    }
                    break;
                case UserTypes.Prom:
                    ClientManager.SetNodeUnblink(client);
                    if (client.PromWindow == null || client.PromWindow.IsDisposed)
                    {
                        client.PromWindow = new ClientPromWin(client);
                        client.PromWindow.Show();
                    }
                    else
                    {
                        client.PromWindow.Focus();
                    }
                    break;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!ClientManager.GetAllClients().TrueForAll(c => (c as Client).ClientWindow == null || (c as Client).ClientWindow.IsDisposed))
            //{
            //    e.Cancel = true;
            //    MessageBox.Show("Закрийте усі діалогові вікна з клієнтами!");
            //}
            if (WindowState != FormWindowState.Minimized)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void MainWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Session.Send(new LogoutRequest());
            foreach (var client in ClientManager.GetAllClients())
            {
                if (client.PopUpWindow != null && !client.PopUpWindow.IsDisposed)
                {
                    client.PopUpWindow.Close();
                }
            }
            Application.Exit();
        }

        private void btAway_Click(object sender, EventArgs e)
        {
            if (btAway.ForeColor == Color.Red)
            {
                tmLastInput.Enabled = false;
                btAway.Text = "Відійти";
                btAway.ForeColor = Color.Black;
                Program.Session.Send(new ReturnAwayRequest());
            }
            else
            {
                Program.Session.Send(new AwayRequest());
                btAway.Text = "Неактивний";
                btAway.ForeColor = Color.Red;

                awaitClick = Environment.TickCount;
                _awaiEnable = false;
                tmLastInput.Enabled = true;
            }
        }

        private void tmLastInput_Tick(object sender, EventArgs e)
        {
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            //lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(out lastInputInfo))
            {
                //label2.Text = ((Environment.TickCount - lastInputInfo.dwTime) / 1000).ToString();

                if (_awaiEnable)
                {
                    if (Environment.TickCount - lastInputInfo.dwTime < 200)
                    {
                        tmLastInput.Enabled = false;
                        btAway.Text = "Відійти";
                        btAway.ForeColor = Color.Black;
                        Program.Session.Send(new ReturnAwayRequest());
                    }
                }
                else
                {
                    if (Environment.TickCount - awaitClick > 20000)
                        _awaiEnable = true;
                }
            }
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://viber.ars.ua/Files/Release/Інструкція для оператора бота.txt");
        }

        private void miUpdate_Click(object sender, EventArgs e)
        {
            if (!ClientManager.GetAllClients().TrueForAll(c => (c as Client).ClientWindow == null || (c as Client).ClientWindow.IsDisposed))
            {
                MessageBox.Show("Закрийте усі діалогові вікна з клієнтами!");
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                var webClient = new WebClient();
                var fileSetup = Path.GetTempPath() + Guid.NewGuid().ToString() + ".exe";
                webClient.DownloadFile("http://viber.ars.ua/Files/Release/setup.exe", fileSetup);
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (key.GetValue("Arsenium") != null)
                    key.DeleteValue("Arsenium");
                key.Close();
                System.Diagnostics.Process.Start(fileSetup);
                Application.Exit();
            }
        }

        private void miReConnect_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ця операція тимчасово заблокована");
            //Program.Session.ReConnect();
            //Program.Session.Send(new ReConnectRequest(Program.Session.SessionID));
        }

        private void miFind_Click(object sender, EventArgs e)
        {
            string phone = null;
            var clipboard = Regex.Replace(Clipboard.GetText(), @"[ A-Za-z(),.+-]+", "");
            if (clipboard.Length >= 10)
                if (clipboard.Length >= 12)
                    phone = clipboard.Substring(clipboard.Length - 12, 12);
                else
                    phone = "38" + clipboard.Substring(clipboard.Length - 10, 10);

            var findWin = new FindFromPhoneWin(phone)
            {
                Location = new Point(Cursor.Position.X - 50, Cursor.Position.Y)
            };
            findWin.ShowDialog();
        }

        private void miPools_Click(object sender, EventArgs e)
        {
            Program.Session.Send(new PoolsListRequest());
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Close();
        }
    }
}
