using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TemporaryUpdater
{
    class Program
    {
        static void GetStringFromArgs(string[] args, ref int i, out string val)
        {
            i++;
            if (i >= args.Length)
            {
                val = "";
                return;
            }
            string ret = "";
            if (args[i].StartsWith("\""))
            {
                for (; i < args.Length; i++)
                {
                    if (args[i].EndsWith("\""))
                    {
                        ret += args[i].Remove(args[i].Length - 1);
                    }
                    else
                    {
                        ret += args[i];
                    }
                }
            }
            else
            {
                ret = args[i];
            }
            val = ret;
        }


        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Temporary Update Thing from Diamondo25");
                Console.WriteLine("This thing is able to replace files and terminate programs with command line arguments:");
                Console.WriteLine("\t-f\t- Sets the filename that will be replaced");
                Console.WriteLine("\t--au\t- Set arguments to add when the program is restarted");
                Console.WriteLine("\t--auf\t- Set arguments to add when the program is restarted, from file");
                Console.WriteLine("\t-t\t- Will terminate -p when it's still running, instead of waiting.");
                Console.WriteLine("\t-b\t- Will backup the file first.");
                Console.WriteLine("Examples:");
                Console.WriteLine("tu.exe -f \"C:\\windows\\explorer.exe\" \"C:\\myevilfile.exe\" -t");
                Console.WriteLine("tu.exe -f \"C:\\MyProgram.exe\" \"C:\\myfile.exe\" -t --auf \"C:\\StartupArgs.txt\"");
                Console.WriteLine("tu.exe -f \"C:\\MyProgram.exe\" \"C:\\myfile.exe\" -t --au \"hai\"");
                Console.ReadLine();
                Environment.Exit(1);
            }

            string replacewhat = "";
            string replacewith = "";
            string argsafterupdate = "";
            string argsafterupdatefile = "";
            bool restart = false;
            bool terminate = true;
            bool backup = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-f")
                {
                    GetStringFromArgs(args, ref i, out replacewhat);
                    GetStringFromArgs(args, ref i, out replacewith);
                }
                else if (args[i] == "--au")
                {
                    GetStringFromArgs(args, ref i, out argsafterupdate);
                }
                else if (args[i] == "--auf")
                {
                    GetStringFromArgs(args, ref i, out argsafterupdatefile);
                }
                else if (args[i] == "-r")
                {
                    restart = true;
                }
                else if (args[i] == "-t")
                {
                    terminate = true;
                }
                else if (args[i] == "-b")
                {
                    backup = true;
                }
            }

            if (replacewhat == "" || !File.Exists(replacewhat) || replacewith == "" || !File.Exists(replacewith) || (argsafterupdatefile != "" && !File.Exists(argsafterupdatefile)))
            {
                Console.WriteLine("Please set the replacement shit!!");
                Environment.Exit(3);
            }

            foreach (Process p in Process.GetProcesses())
            {
                if (p.MainWindowTitle != "")
                {
                    try
                    {
                        //Console.WriteLine(p.MainModule.FileName);
                        if (p.MainModule.FileName.Contains(replacewith) || p.MainModule.FileName.Contains(replacewhat))
                        {
                            Console.WriteLine("d");
                            if (terminate)
                            {
                                p.Kill();
                            }
                            else
                            {
                                p.WaitForExit();
                            }
                        }
                    }
                    catch { }
                }
            }
            // Lets replace

            if (File.Exists(replacewhat))
            {
                if (backup)
                {
                    string f = Path.GetFileNameWithoutExtension(replacewhat);
                    File.Move(replacewhat, replacewhat.Replace(f, "backup_" + DateTime.Now.ToString("d_M_yyyy__HH_mm_ss") + f));
                }
                else
                {
                    File.Delete(replacewhat);
                }
            }

            File.Move(replacewith, replacewhat);

            if (restart)
            {
                string arg = "";
                if (argsafterupdatefile != "")
                {
                    using (var file = File.Open(argsafterupdatefile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var derp = new StreamReader(file))
                        {
                            arg = derp.ReadToEnd().Replace(Environment.NewLine, "");
                        }
                    }
                }
                else if (argsafterupdate != "")
                {
                    arg = argsafterupdate;
                }

                Process.Start(replacewhat, arg);
            }
        }
    }
}
