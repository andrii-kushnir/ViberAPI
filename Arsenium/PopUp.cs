using Models;
using Models.Messages.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arsenium
{
    public partial class PopUp : Form
    {
        private readonly Client _client;
        private readonly string _message;
        private readonly string _operatorName;
        private PopUpBack _backForm = new PopUpBack();
        public PopUp()
        {
            InitializeComponent();
        }

        public PopUp(Client client, string operatorName, string message = null) : this()
        {
            _client = client;
            _message = message;
            _operatorName = operatorName;
        }

        private void PopUp_Load(object sender, EventArgs e)
        {
            switch (_client.Type)
            {
                case UserTypes.Viber:
                    this.BackColor = Color.FromArgb(100, 90, 170);
                    break;
                case UserTypes.Prom:
                    this.BackColor = Color.FromArgb(0, 170, 220);
                    break;
                case UserTypes.Rozetka:
                    this.BackColor = Color.FromArgb(0, 192, 80);
                    break;
            }
            this.FormBorderStyle = FormBorderStyle.None;

            //Прозорість:
            this.AllowTransparency = true;
            this.TransparencyKey = Color.AliceBlue;//он же будет заменен на прозрачный цвет; AliceBlue - це колір control-а тексту 

            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            //Rectangle workingArea = Screen.AllScreens[0].WorkingArea;
            Left = workingArea.Left + workingArea.Width - Size.Width - 2;
            Top = workingArea.Top + workingArea.Height - Size.Height - 2;

            _backForm.BackColor = this.BackColor;
            _backForm.Left = Left;
            _backForm.Top = Top;
            _backForm.Show();

            lbMessage.Text = ((_operatorName == null) ? "" : $"Оператор : {_operatorName}\n") + ((_message == null) ? $"{_client.Name} хоче поговорити!" : $"{_client.Name}: {_message}");
            lbMessage.Location = new Point((Size.Width - lbMessage.Size.Width) / 2, (Size.Height - lbMessage.Size.Height) / 2);
        }

        private void PopUp_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Black, 1), 1, 1, Size.Width - 3, Size.Height - 3);
        }

        private void PopUp_MouseDown(object sender, MouseEventArgs e)
        {
            ClientManager.SetNodeUnblink(_client);
            if (_client.Type == UserTypes.Viber && _client.DetailsInfo == null)
            {
                Program.Session.Send(new UserDetailsRequest(new User(_client.Id, UserTypes.Viber)));
                var i = 0;
                while (_client.DetailsInfo == null && i <= 3000)
                {
                    System.Threading.Thread.Sleep(50);
                    i += 50;
                }
            }

            switch (_client.Type)
            {
                case UserTypes.Viber:
                    if (_client.ClientWindow == null || _client.ClientWindow.IsDisposed)
                        if (_client.DetailsInfo == null)
                        {
                            MessageBox.Show("Дані по контакту не можуть бути отримані.");
                            Close();
                        }
                        else
                        {
                            _client.ClientWindow = new ClientSessionWin(_client) { Text = $"Далог з {_client.NameShow}" };
                            _client.ClientWindow.Show();
                        }
                    else
                        _client.ClientWindow.Focus();
                    break;
                case UserTypes.Rozetka:
                    if (_client.RozetkaWindow == null || _client.RozetkaWindow.IsDisposed)
                    {
                        _client.RozetkaWindow = new ClientRozetkaWin(_client);
                        _client.RozetkaWindow.Show();
                    }
                    else
                        _client.RozetkaWindow.Focus();
                    break;
                case UserTypes.Prom:
                    if (_client.PromWindow == null || _client.PromWindow.IsDisposed)
                    {
                        _client.PromWindow = new ClientPromWin(_client);
                        _client.PromWindow.Show();
                    }
                    else
                    {
                        _client.PromWindow.Focus();
                    }
                    break;
            }
        }

        //private bool Drag;
        //private int MouseX;
        //private int MouseY;

        //private const int WM_NCHITTEST = 0x84;
        //private const int HTCLIENT = 0x1;
        //private const int HTCAPTION = 0x2;

        //private bool m_aeroEnabled = true;

        //private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        //private const int WM_ACTIVATEAPP = 0x001C;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]

        //public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        //[System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        m_aeroEnabled = CheckAeroEnabled();
        //        CreateParams cp = base.CreateParams;
        //        if (!m_aeroEnabled)
        //            cp.ClassStyle |= CS_DROPSHADOW; return cp;
        //    }
        //}
        //private bool CheckAeroEnabled()
        //{
        //    if (Environment.OSVersion.Version.Major >= 6)
        //    {
        //        int enabled = 0; 
        //        DwmIsCompositionEnabled(ref enabled);
        //        return (enabled == 1) ? true : false;
        //    }
        //    return false;
        //}

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    //if (m_aeroEnabled)
                    //{
                    var v = 2;
                    DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                    MARGINS margins = new MARGINS()
                    {
                        bottomHeight = 1,
                        leftWidth = 1,
                        rightWidth = 1,
                        topHeight = 1
                    };
                    DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    //}
                    break;
                default: break;
            }
            base.WndProc(ref m);
            //if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backForm.Close();
        }

        private void lbMessage_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(lbMessage.BackColor);
            using (var sf = new StringFormat())
            using (var br = new SolidBrush(lbMessage.ForeColor))
            {
                sf.Alignment = sf.LineAlignment = StringAlignment.Center;
                e.Graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                e.Graphics.DrawString(lbMessage.Text, lbMessage.Font, br, lbMessage.ClientRectangle, sf);
            }
        }

        //private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        //{
        //    Drag = true;
        //    MouseX = Cursor.Position.X - this.Left;
        //    MouseY = Cursor.Position.Y - this.Top;
        //}
        //private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (Drag)
        //    {
        //        this.Top = Cursor.Position.Y - MouseY;
        //        this.Left = Cursor.Position.X - MouseX;
        //    }
        //}
        //private void PanelMove_MouseUp(object sender, MouseEventArgs e) { Drag = false; }
    }
}
