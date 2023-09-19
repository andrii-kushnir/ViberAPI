using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models.Messages;
using Models.Messages.Requests;
using Models.Messages.Responses;
using Models.Network;
using Newtonsoft.Json;
using NLog;
using System.Configuration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Arsenium
{
    static class Program
    {
        public static Session Session;
        public static HandlerManager HandlerManager;
        public static MainWin MainWin = null;
        public static string Files;
        public static string CurrentLink;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            CurrentLink = CreateLabel.CreateLabelDo();

            var proccessStartInfo = new ProcessStartInfo("net", @"use W: \\192.168.4.100\Files /user:viberbot viberbot")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var proc = new Process { StartInfo = proccessStartInfo };
            proc.Start();
            //proc.StandardOutput.ReadToEnd();

            var ipAddress = ConfigurationManager.AppSettings.Get("ipAddress");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("port"));
            Files = ConfigurationManager.AppSettings.Get("files");

            LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().WriteToFile("${shortdate}viberlog.txt", "${longdate} | ${uppercase:${level}} |${message}");
            });

            Session = new Session(new MessageHandler(), ipAddress, port);
            HandlerManager = new HandlerManager(Session);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new LogIn(HandlerManager));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var loginForm = new LogIn(HandlerManager);
            loginForm.Show();

            Application.Run();
        }
    }
}

