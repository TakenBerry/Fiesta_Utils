using SharpPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MapleShark
{
    public partial class SetupForm : Form
    {
        Boolean isWinPcapInstalled = false;
        public SetupForm()
        {
            InitializeComponent();

            Text = "FiestaShark " + Program.AssemblyVersion + ", " + Program.AssemblyCopyright + ", Modified by NextIdea";
            bool selected = false;
            int localAreaConnection = -1;
            try {
                foreach( PcapDevice device in new PcapDeviceList() ) {
                    if( !device.Interface.Addresses.Exists(a => a != null && a.Addr != null && a.Addr.ipAddress != null && a.Addr.ipAddress.AddressFamily == AddressFamily.InterNetwork) ) continue;
                    int index = mInterfaceCombo.Items.Add(device.Interface.FriendlyName);
                    if( device.Interface.FriendlyName == "Local Area Connection" ) localAreaConnection = index;
                    if( !selected && (selected = (device.Interface.FriendlyName == Config.Instance.Interface)) ) mInterfaceCombo.SelectedIndex = index;
                }
                if( !selected && localAreaConnection >= 0 ) mInterfaceCombo.SelectedIndex = localAreaConnection;
                else if( !selected && mInterfaceCombo.Items.Count > 0 ) mInterfaceCombo.SelectedIndex = 0;
                mLowPortNumeric.Value = Config.Instance.LowPort;
                mHighPortNumeric.Value = Config.Instance.HighPort;
                this.isWinPcapInstalled = true;
            } catch {       // Not installed
                this.isWinPcapInstalled = false;
            }
        }

        private void SetupForm_Load(object pSender, EventArgs pArgs)
        {
            if( !isWinPcapInstalled ) {
                MessageBox.Show(this, "You don't have WinPcap installed. Go download and install it.", "WinPcap not installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }else if( mInterfaceCombo.Items.Count == 0 ) {
                MessageBox.Show(this, "There are no usable interfaces", "Interface Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
                return;
            }
        }

        private void mInterfaceCombo_SelectedIndexChanged(object pSender, EventArgs pArgs)
        {
            mOKButton.Enabled = mInterfaceCombo.SelectedIndex >= 0;
        }

        private void mLowPortNumeric_ValueChanged(object pSender, EventArgs pArgs)
        {
            if (mLowPortNumeric.Value > mHighPortNumeric.Value) mLowPortNumeric.Value = mHighPortNumeric.Value;
        }

        private void mHighPortNumeric_ValueChanged(object pSender, EventArgs pArgs)
        {
            if (mHighPortNumeric.Value < mLowPortNumeric.Value) mHighPortNumeric.Value = mLowPortNumeric.Value;
        }

        private void mOKButton_Click(object pSender, EventArgs pArgs)
        {
            Config.Instance.Interface = (string)mInterfaceCombo.SelectedItem;
            Config.Instance.LowPort = (ushort)mLowPortNumeric.Value;
            Config.Instance.HighPort = (ushort)mHighPortNumeric.Value;
            Config.Instance.Save();
            DialogResult = DialogResult.OK;
        }
    }
}
