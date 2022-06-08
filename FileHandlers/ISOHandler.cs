using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.FileHandlers
{
    internal class ISOHandler
    {
        public static void Extract(string path, bool wait =false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = MainWindow.settings.ZipPath;
            string test = Path.Combine(MainWindow.workspacePath, "");
            startInfo.Arguments = "x \"" + path + "\" *.* -o\"" + test + "\" -r -y";
            Process.Start(startInfo);
            if (wait)
            {

            }
        }

        public static void Build(string path, bool wait = false)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = MainWindow.settings.ImgBurnPath;
            string test = Path.Combine(MainWindow.workspacePath, "");
            if (MainWindow.settings.Game == 0)
            {
                MainWindow.settings.SSXISOPath = path;
            }
            else if (MainWindow.settings.Game == 1)
            {
                MainWindow.settings.SSX2ISOPath = path;
            }
            else if (MainWindow.settings.Game == 2)
            {
                MainWindow.settings.SSX3ISOPath = path;
            }
            else if (MainWindow.settings.Game == 3)
            {
                MainWindow.settings.SSX4ISOPath = path;
            }
            MainWindow.settings.Save();
            startInfo.Arguments = "/MODE BUILD /BUILDINPUTMODE IMAGEFILE /SRC \"" + test + "\" /DEST \"" + path + "\" /FILESYSTEM \"ISO9660 + UDF\" /UDFREVISION \"1.02\" /VOLUMELABEL \"SSX3\" /ERASE /OVERWITE YES /START /NOIMAGEDETAILS /ROOTFOLDER /CLOSE";
            var temp = Process.Start(startInfo);
            if (wait)
            {
                temp.WaitForExit();
            }
        }

        public static void Run()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = MainWindow.settings.Pcsx2Path;

            string path = "";
            if (MainWindow.settings.Game == 0)
            {
                path = MainWindow.settings.SSXISOPath;
            }
            else if (MainWindow.settings.Game == 1)
            {
                path = MainWindow.settings.SSX2ISOPath;
            }
            else if (MainWindow.settings.Game == 2)
            {
                path = MainWindow.settings.SSX3ISOPath;
            }
            else if (MainWindow.settings.Game == 3)
            {
                path = MainWindow.settings.SSX4ISOPath;
            }


            if (File.Exists(path))
            {
                if (path.ToLower().Contains(".iso"))
                {
                    startInfo.Arguments = "\"" + path + "\"";
                }
                else if (path.ToLower().Contains(".elf"))
                {
                    startInfo.Arguments = "-elf \"" + path + "\"";
                }
            }
            Process.Start(startInfo);
        }
    }
}
