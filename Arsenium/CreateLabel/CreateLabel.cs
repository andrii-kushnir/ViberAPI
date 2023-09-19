using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vbAccelerator.Components.Shell;

namespace Arsenium
{
    public class CreateLabel
    {
        public static string CreateLabelDo()
        {
            
            var executablePath = Application.ExecutablePath;
            //var currentDirectory = (new FileInfo(executablePath)).DirectoryName;
            var currentDirectory = Directory.GetCurrentDirectory();
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string link = desktopPath + @"\Arsenium.lnk";

            var shortcut = new ShellLink()
            {
                ShortCutFile = link,
                Target = executablePath,
                WorkingDirectory = currentDirectory,
                IconPath = currentDirectory + @"\MainFormIcon256.ico",
                Description = "Arsenium",
                DisplayMode = ShellLink.LinkDisplayMode.edmNormal
            };

            shortcut.Save();

            return link;

            //shortcut.IconPath = executablePath;
            //shortcut.IconIndex = 1;
            //shortcut.Arguments = "file.txt";
        }
    }
}
