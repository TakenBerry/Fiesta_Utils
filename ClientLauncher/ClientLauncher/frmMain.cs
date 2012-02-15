using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;

namespace ClientLauncher
{
    public partial class frmMain : Form
    {
        private bool running = false;
        static bool Multiclient = false;
        static string IPAddr = "83.80.148.175";
        static string OskStore = "http://www.google.nl/";
        private string BinFilename = "derp.exe";


        public frmMain()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Program.Filename))
            {
                MessageBox.Show("Did you forget to set the Fiesta.bin location? I cannot find it.");
                return;
            }
            if (txtUsername.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter a correct username.");
                return;
            }

            if (Multiclient || !running)
            {
                ChangeStatusText("Generating hash..");
                string md5hash = Md5Hash(txtPassword.Text);
                ChangeStatusText("Creating process..");
                Process proc = new Process();
                proc.StartInfo.Arguments = string.Format("-osk_server {0} -osk_token {1}{2} -osk_store {3}", IPAddr, md5hash, txtUsername.Text, OskStore);
                proc.StartInfo.FileName = Program.Filename;
                proc.StartInfo.Verb = "runas";
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(Program.Filename);
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                ChangeStatusText("Started!");
                Program.Save(Program.Filename, txtUsername.Text);
            }
        }

        public static string Md5Hash(string pass)
        {
            using (MD5 md5 = MD5CryptoServiceProvider.Create())
            {
                byte[] dataMd5 = md5.ComputeHash(Encoding.Default.GetBytes(pass));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < dataMd5.Length; i++)
                    sb.AppendFormat("{0:x2}", dataMd5[i]);
                return sb.ToString();
            }
        }

        private void tmrCheckExe_Tick(object sender, EventArgs e)
        {
            var processes = Process.GetProcesses();
            if (processes.Count(p => p.ProcessName == BinFilename) > 0)
            {
                if (!running)
                {
                    ChangeStatusText("Client is running.");
                    if (!Multiclient)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            txtPassword.Enabled = txtUsername.Enabled = btnStart.Enabled = btnSelectBin.Enabled = false;
                        });
                    }
                    running = true;
                }
            }
            else
            {
                if (running)
                {
                    ChangeStatusText("Ready to start.");
                    if (!Multiclient)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            txtPassword.Enabled = txtUsername.Enabled = btnStart.Enabled = btnSelectBin.Enabled = true;
                        });
                    }
                    running = false;
                }
            }
        }

        private void ChangeStatusText(string what, params object[] extra)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.tsslStatus.Text = string.Format(what, extra);
            });
        }

        private void btnSelectBin_Click(object sender, EventArgs e)
        {
            if (ofdBin.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBinLoc.Text = ofdBin.FileName;
                Program.Save(ofdBin.FileName, txtUsername.Text);
                BinFilename = Path.GetFileName(Program.Filename);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Program.Load();
            txtBinLoc.Text = Program.Filename;
            txtUsername.Text = Program.Username;
            BinFilename = Path.GetFileName(Program.Filename);
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void niTray_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }
    }
}
