using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Models;
using Models.Messages.Requests;
using Models.Network;

namespace Arsenium
{
    public partial class LogIn : Form
    {
        public static object lockObject = new object();

        public LogIn()
        {
            InitializeComponent();
        }

        public LogIn(HandlerManager HandlerManager) : this()
        {
            HandlerManager.LoginAccessResponse += HandlerManager_LoginAccessResponse;
            HandlerManager.ConnectionClosed += HandlerManager_ConnectionClosed;
        }

        private void HandlerManager_LoginAccessResponse(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                if (Program.MainWin == null)
                {
                    void CreateMainWin()
                    {
                        if (this.InvokeRequired)
                        {
                            this.SafeInvoke(CreateMainWin);
                            return;
                        }
                        var mainWin = new MainWin();
                        Program.MainWin = mainWin;
                        this.Close();
                        mainWin.Show();
                    }
                    CreateMainWin();
                }
            }
        }

        private void HandlerManager_ConnectionClosed(object sender, SocketErrorEventArgs e)
        {
            if (e.SocketError == System.Net.Sockets.SocketError.TimedOut)
                MessageBox.Show($"Час очікувння сервера минув");
            else
                MessageBox.Show($"Немає/обрив зв'язку з сервером");
        }

        private void _btLogIn_Click(object sender, EventArgs e)
        {
            var request = new LoginRequest(Program.Session.SessionID, _tbLogin.Text.Trim(), HashPassword.Hash(_tbPassword.Text.Trim()));
            Program.Session.Send(request);
            AddAutoload();
        }

        private void AddAutoload()
        {
            var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (key.GetValue("Arsenium") != null)
                key.DeleteValue("Arsenium");
            if (_cbAutoload.Checked)
                key.SetValue("Arsenium", Program.CurrentLink);
                //key.SetValue("Arsenium", Application.ExecutablePath + " -s");
            key.Close();
        }

        private void _btExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _btLogIn_Click(sender, EventArgs.Empty);
            }
        }

        private void _tbLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                _tbPassword.Focus();
            }
        }

        private void LogIn_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Program.MainWin == null)
                Application.Exit();
        }

        private void LogIn_Load(object sender, EventArgs e)
        {
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;

            //ver = ApplicationDeployment.IsNetworkDeployed
            //    ? ApplicationDeployment.CurrentDeployment.CurrentVersion
            //    : Assembly.GetExecutingAssembly().GetName().Version;

            lVersion.Text = $"Version: {ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        }
    }
}
