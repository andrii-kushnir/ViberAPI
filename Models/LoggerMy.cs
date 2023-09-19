using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class LoggerMy : IDisposable
    {
        public static void Write(string text)
        {
            Logs.Add(new Log { Text = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss,FF} : {text}", Writed = false });
            if (Logs.Count >= countMinLog)
                RealWrite();
        }

        public void Dispose()
        {
            RealWrite(true);
        }

        private const string logFileName = "Log.txt";
        //private static Logger logger = new Logger(logFileName);

        private const int countMinLog = 1;

        private class Log
        {
            internal string Text;
            internal bool Writed;
        }

        private static readonly StreamWriter Writer = new StreamWriter(Path.GetTempPath() + logFileName, append: true);
        private static readonly List<Log> Logs = new List<Log>();
        private static readonly object sync = new object();

        private static void RealWrite(bool fix = false)
        {
            lock (sync)
                if (Logs.Count >= countMinLog || (fix && Logs.Count != 0))
                {
                    Logs.ForEach(log => { Writer.WriteLine(log.Text); log.Writed = true; });
                    Writer.Flush();
                    Logs.RemoveAll(log => log.Writed);
                }
        }
    }
}
