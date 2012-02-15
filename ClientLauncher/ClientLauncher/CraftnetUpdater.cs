using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace CraftnetUpdater
{
    public static class CraftnetUpdater
    {
        private static string AppName = "ZEPHCLIENTLNCHR";
        private static string VersionStr = "version: ";
        private static int AppVersion = 1;

        public static void CheckUpdates()
        {
            using (WebClient wc = new WebClient())
            {
                using (var stream = wc.OpenRead(string.Format("http://www.craftnet.nl/app_updates/updates.php?appname={0}", AppName)))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        string line = sr.ReadLine();
                        if (line.Contains(VersionStr) && line.Replace(VersionStr, "") != AppVersion.ToString())
                        {
                            string file = sr.ReadLine();
                            string tmpfile = Path.GetTempFileName();
                            wc.DownloadFile(file, tmpfile);
                            string updater = ExtractResource();
                            Process proc = new Process();
                            proc.StartInfo.FileName = updater;
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.Verb = "runas";
                            proc.StartInfo.Arguments = string.Format("-f \"{0}\" \"{1}\" -t -b -r", Process.GetCurrentProcess().MainModule.FileName, tmpfile);
                            proc.Start();
                        }
                    }
                }
            }
        }


        private static string ExtractResource()
        {
            string tempfile = Path.GetTempFileName();
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("ClientLauncher.Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());
            object obj = rm.GetObject("TemporaryUpdater");
            byte[] source = (byte[])(obj);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(tempfile));
            bw.Write(source);
            bw.Flush();
            bw.Close();
            return tempfile;
        }
    }
}
