using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ClientLauncher
{
    static class Program
    {
        public static string Filename { get; private set; }
        public static string Username { get; private set; }
        static string ConfigName = "Configuration.inf";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CraftnetUpdater.CraftnetUpdater.CheckUpdates();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        public static void Load()
        {
            if (File.Exists(ConfigName))
            {
                using (var file = File.Open(ConfigName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        Filename = sr.ReadLine();
                        Username = sr.ReadLine();
                    }
                }
            }
            else
            {
                Save("", "");
            }
        }

        public static void Save(string filename, string username)
        {
            using (var file = File.OpenWrite(ConfigName))
            {
                using (var sr = new StreamWriter(file))
                {
                    sr.WriteLine(filename);
                    sr.WriteLine(username);
                    sr.Flush();
                }
            }
            Filename = filename;
            Username = username;
        }
    }
}
