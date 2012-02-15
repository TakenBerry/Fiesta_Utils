using ScriptNET;
using SharpPcap.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MapleShark {
    public partial class SessionForm: DockContent {
        private string mFilename = null;
        public bool Saved { get { return mFilename != null; } }
        private bool mTerminated = false;
        private ushort mLocalPort = 0;
        private ushort mRemotePort = 0;
        private uint mOutboundSequence = 0;
        private uint mInboundSequence = 0;
        private bool gotEnc = false;
        private Dictionary<uint, byte[]> mOutboundBuffer = new Dictionary<uint, byte[]>();
        private Dictionary<uint, byte[]> mInboundBuffer = new Dictionary<uint, byte[]>();
        private FiestaStream mOutboundStream = null;
        private FiestaStream mInboundStream = null;
        private List<FiestaPacket> mPackets = new List<FiestaPacket>();
        private List<Pair<bool, ushort>> mOpcodes = new List<Pair<bool, ushort>>();

        internal SessionForm() {
            InitializeComponent();
        }

        public MainForm MainForm { get { return ParentForm as MainForm; } }
        public ListView ListView { get { return mPacketList; } }
        public List<Pair<bool, ushort>> Opcodes { get { return mOpcodes; } }

        internal bool MatchTCPPacket(TCPPacket pTCPPacket) {
            if( mTerminated ) return false;
            if( pTCPPacket.SourcePort == mLocalPort && pTCPPacket.DestinationPort == mRemotePort ) return true;
            if( pTCPPacket.SourcePort == mRemotePort && pTCPPacket.DestinationPort == mLocalPort ) return true;
            return false;
        }
        internal void BufferTCPPacket(TCPPacket pTCPPacket) {
            if( pTCPPacket.Fin || pTCPPacket.Rst ) {
                mTerminated = true;
                Text += " (T.)";
                if( mPackets.Count == 0 ) this.Close();
                return;
            }
            if( pTCPPacket.Syn && !pTCPPacket.Ack ) {
                mLocalPort = (ushort)pTCPPacket.SourcePort;
                mRemotePort = (ushort)pTCPPacket.DestinationPort;
                mOutboundSequence = (uint)(pTCPPacket.SequenceNumber + 1);
                Text = "Port " + mLocalPort.ToString();
                return;
            }
            if( pTCPPacket.Syn && pTCPPacket.Ack ) { mInboundSequence = (uint)(pTCPPacket.SequenceNumber + 1); return; }
            if( pTCPPacket.PayloadDataLength == 0 ) return;
            if( !gotEnc ) {
                byte[] tcpData = pTCPPacket.TCPData;
                short xorKey = 0;
                if( tcpData.Length == 5 && tcpData[0] == 4 && tcpData[1] == 7 && tcpData[2] == 8 ) {
                    xorKey = (short)(tcpData[3] | (tcpData[4] << 8));
                } else {
                    /*FileStream fs = new FileStream("C:\\Documents and Settings\\Removed\\Desktop\\FiestaShark\\PacketTest.txt", FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    for( short i = 0; i < 499; i++ ) {
                        byte[] tmp = PacketCrypt.Decrypt(tcpData, i);
                        bw.Write(i + " - ");
                        bw.Write(tmp, 0, 32);
                        bw.Write(Environment.NewLine);
                        if( tmp[16] == 0 && tmp[17] == 0 ) {
                            xorKey = i;
                            bw.Write("[XorKey] = " + xorKey + Environment.NewLine);
                        }
                    }
                    bw.Close();
                    fs.Close();*/
                    return;
                }
                mOutboundStream = new FiestaStream(xorKey, true);
                mInboundStream = new FiestaStream(xorKey, false);
                mInboundSequence += (uint)tcpData.Length;
                gotEnc = true;
                //return;
            }
            if( pTCPPacket.SourcePort == mLocalPort ) ProcessTCPPacket(pTCPPacket, ref mOutboundSequence, mOutboundBuffer, mOutboundStream); 
            else ProcessTCPPacket(pTCPPacket, ref mInboundSequence, mInboundBuffer, mInboundStream); 
        }
        private void ProcessTCPPacket(TCPPacket pTCPPacket, ref uint pSequence, Dictionary<uint, byte[]> pBuffer, FiestaStream pStream) {
            if( pTCPPacket.SequenceNumber > pSequence ) {
                byte[] data;
                while( (data = pBuffer.GetOrDefault(pSequence, null)) != null ) {
                    pBuffer.Remove(pSequence);
                    pStream.Append(data);
                    pSequence += (uint)data.Length;
                }
                if( pTCPPacket.SequenceNumber > pSequence ) pBuffer[(uint)pTCPPacket.SequenceNumber] = pTCPPacket.TCPData;
            }
            if( pTCPPacket.SequenceNumber < pSequence ) {
                int difference = (int)(pSequence - pTCPPacket.SequenceNumber);
                if( difference > 0 ) {
                    byte[] data = pTCPPacket.TCPData;
                    if( data.Length > difference ) {
                        pStream.Append(data, difference, data.Length - difference);
                        pSequence += (uint)(data.Length - difference);
                    }
                }
            } else if( pTCPPacket.SequenceNumber == pSequence ) {
                byte[] data = pTCPPacket.TCPData;
                pStream.Append(data);
                pSequence += (uint)data.Length;
            }

            FiestaPacket packet;
            bool refreshOpcodes = false;
            try {
                while( (packet = pStream.Read(DateTime.Now)) != null ) {
                    mPackets.Add(packet);
                    Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                    if( !mOpcodes.Exists(kv => kv.First == packet.Outbound && kv.Second == packet.Opcode) ) {
                        mOpcodes.Add(new Pair<bool, ushort>(packet.Outbound, packet.Opcode));
                        refreshOpcodes = true;
                    }
                    if( definition != null && definition.Ignore ) continue;
                    mPacketList.Items.Add(packet);
                    if( mPacketList.SelectedItems.Count == 0 ) packet.EnsureVisible();
                }
            } catch( Exception exc ) {
                OutputForm output = new OutputForm("Packet Error");
                output.Append(exc.ToString());
                output.Show(DockPanel, new Rectangle(MainForm.Location, new Size(400, 400)));
                mTerminated = true;
                Text += " (T.)";
            }

            if( DockPanel.ActiveDocument == this && refreshOpcodes ) MainForm.SearchForm.RefreshOpcodes(true);
        }

        public void OpenReadOnly(string pFilename) {
            mFileSaveMenu.Enabled = false;
            mTerminated = true;
            long sessionStart = 0;
            using( FileStream stream = new FileStream(pFilename, FileMode.Open, FileAccess.Read) ) {
                BinaryReader reader = new BinaryReader(stream);
                mLocalPort = reader.ReadUInt16();
                sessionStart = reader.ReadInt64();
                while( stream.Position < stream.Length ) {
                    long timestamp = reader.ReadInt64();
                    ushort size = reader.ReadUInt16();
                    ushort opcode = reader.ReadUInt16();
                    bool outbound = (size & 0x8000) != 0;
                    size = (ushort)(size & 0x7FFF);
                    byte[] buffer = reader.ReadBytes(size);
                    Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == outbound && d.Opcode == opcode);
                    FiestaPacket packet = new FiestaPacket(new DateTime(timestamp), outbound, opcode, definition == null ? "" : definition.Name, buffer, new DateTime(sessionStart));
                    mPackets.Add(packet);
                    if( !mOpcodes.Exists(kv => kv.First == packet.Outbound && kv.Second == packet.Opcode) ) mOpcodes.Add(new Pair<bool, ushort>(packet.Outbound, packet.Opcode));
                    if( packet.definition != null && packet.definition.Ignore ) continue;
                    mPacketList.Items.Add(packet);
                }
                if( mPacketList.Items.Count > 0 ) mPacketList.EnsureVisible(0);
            }
            Text = string.Format("Port {0} ({1})", mLocalPort, new DateTime(sessionStart).ToShortDateString());
        }

        public void RefreshPackets() {
            Pair<bool, ushort> search = (MainForm.SearchForm.ComboBox.SelectedIndex >= 0 ? mOpcodes[MainForm.SearchForm.ComboBox.SelectedIndex] : null);
            FiestaPacket previous = mPacketList.SelectedItems.Count > 0 ? mPacketList.SelectedItems[0] as FiestaPacket : null;
            mOpcodes.Clear();
            mPacketList.Items.Clear();
            MainForm.DataForm.HexBox.ByteProvider = null;
            MainForm.StructureForm.Tree.Nodes.Clear();
            MainForm.PropertyForm.Properties.SelectedObject = null;
            if( !mViewOutboundMenu.Checked && !mViewInboundMenu.Checked ) return;
            mPacketList.BeginUpdate();
            for( int index = 0; index < mPackets.Count; ++index ) {
                FiestaPacket packet = mPackets[index];
                if( packet.Outbound && !mViewOutboundMenu.Checked ) continue;
                if( !packet.Outbound && !mViewInboundMenu.Checked ) continue;
                Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
				packet.definition = definition;
                packet.DefinitionName = definition == null ? "" : definition.Name;
                if( !mOpcodes.Exists(kv => kv.First == packet.Outbound && kv.Second == packet.Opcode) ) mOpcodes.Add(new Pair<bool, ushort>(packet.Outbound, packet.Opcode));
                if( packet.definition != null && !mViewIgnoredMenu.Checked && packet.definition.Ignore ) continue;
                mPacketList.Items.Add(packet);
                if( packet == previous ) packet.Selected = true;
            }
            mPacketList.EndUpdate();
            MainForm.SearchForm.RefreshOpcodes(true);
        }


        private void mFileSaveMenu_Click(object pSender, EventArgs pArgs) {
            SaveMe();
        }

        public void SaveMe()
        {
            if (mFilename == null)
            {
                mSaveDialog.FileName = string.Format("Port {0}", mLocalPort);
                if (mSaveDialog.ShowDialog(this) == DialogResult.OK) mFilename = mSaveDialog.FileName;
                else return;
            }
            using (FileStream stream = new FileStream(mFilename, FileMode.Create, FileAccess.Write))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(mLocalPort);
                writer.Write(mOutboundStream.Created.Ticks);
                foreach (FiestaPacket packet in mPackets) writer.Write(packet.Dump());
                stream.Flush();
            }
            if (mTerminated)
            {
                mFileSaveMenu.Enabled = false;
                Text = string.Format("Port {0} (ReadOnly)", mLocalPort);
            }
        }

        private void mFileExportMenu_Click(object pSender, EventArgs pArgs) {
            mExportDialog.FileName = string.Format("Port {0}", mLocalPort);
            if( mExportDialog.ShowDialog(this) != DialogResult.OK ) return;
            using(var stream = File.Open(mExportDialog.FileName, FileMode.Create, FileAccess.Write) ) {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("=== Port: {0} ===", mLocalPort);
                    int outboundCount = 0;
                    int inboundCount = 0;
                    foreach (FiestaPacket packet in mPackets)
                    {
                        if (packet.Outbound) ++outboundCount;
                        else ++inboundCount;
                        writer.WriteLine("[{0}]\t({1})\t<{2}>\t==\t{3}\t{4}", packet.Timestamp, (packet.Outbound ? outboundCount : inboundCount), (packet.Outbound ? "Outbound" : "Inbound"), string.Format("{0:D3} | {1:D3}", packet.Header, packet.Type), BitConverter.ToString(packet.InnerBuffer).Replace('-', ' '));
                    }
                }
            }
        }

        private void mViewGlobalScriptMenu_Click(object pSender, EventArgs pArgs) {
            string scriptPath = "Scripts" + Path.DirectorySeparatorChar + "Global.txt";
            if( !Directory.Exists(Path.GetDirectoryName(scriptPath)) ) Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));
            ScriptForm script = new ScriptForm(scriptPath, null);
            script.FormClosed += GlobalScript_FormClosed;
            script.Show(DockPanel, new Rectangle(MainForm.Location, new Size(600, 300)));
        }
        private void GlobalScript_FormClosed(object pSender, FormClosedEventArgs pArgs) {
            if( mPacketList.SelectedIndices.Count == 0 ) return;
            FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
            MainForm.StructureForm.ParseFiestaPacket(packet);
            Activate();
        }

        private void mViewCommonScriptMenu_Click(object pSender, EventArgs pArgs) {
            string scriptPath = "Scripts" + Path.DirectorySeparatorChar + "Common.txt";
            if( !Directory.Exists(Path.GetDirectoryName(scriptPath)) ) Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));
            ScriptForm script = new ScriptForm(scriptPath, null);
            script.FormClosed += CommonScript_FormClosed;
            script.Show(DockPanel, new Rectangle(MainForm.Location, new Size(600, 300)));
        }
        private void CommonScript_FormClosed(object pSender, FormClosedEventArgs pArgs) {
            if( mPacketList.SelectedIndices.Count == 0 ) return;
            FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
            MainForm.StructureForm.ParseFiestaPacket(packet);
            Activate();
        }

        private void mViewRefreshMenu_Click(object pSender, EventArgs pArgs) { RefreshPackets(); }
        private void mViewOutboundMenu_CheckedChanged(object pSender, EventArgs pArgs) { RefreshPackets(); }
        private void mViewInboundMenu_CheckedChanged(object pSender, EventArgs pArgs) { RefreshPackets(); }
        private void mViewIgnoredMenu_CheckedChanged(object pSender, EventArgs pArgs) { RefreshPackets(); }

        private void mPacketList_SelectedIndexChanged(object pSender, EventArgs pArgs) {
            try {
                if( mPacketList.SelectedItems.Count == 0 ) { MainForm.DataForm.HexBox.ByteProvider = null; MainForm.StructureForm.Tree.Nodes.Clear(); MainForm.PropertyForm.Properties.SelectedObject = null; return; }
                MainForm.DataForm.HexBox.ByteProvider = new DynamicByteProvider((mPacketList.SelectedItems[0] as FiestaPacket).InnerBuffer);
                MainForm.StructureForm.ParseFiestaPacket(mPacketList.SelectedItems[0] as FiestaPacket);
            } catch {
                // When you move session window, while something is selected inside it, then we end here.
            }
        }

        private void mPacketList_ItemActivate(object pSender, EventArgs pArgs) {
            if( mPacketList.SelectedIndices.Count == 0 ) return;
            FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
            string scriptPath = "Scripts" + Path.DirectorySeparatorChar + (packet.Outbound ? "Outbound" : "Inbound") + Path.DirectorySeparatorChar + "0x" + packet.Opcode.ToString("X4") + ".txt";
            if( !Directory.Exists(Path.GetDirectoryName(scriptPath)) ) Directory.CreateDirectory(Path.GetDirectoryName(scriptPath));
            ScriptForm script = new ScriptForm(scriptPath, packet);
            script.FormClosed += Script_FormClosed;
            script.Show(DockPanel, new Rectangle(MainForm.Location, new Size(600, 300)));
        }

        private void Script_FormClosed(object pSender, FormClosedEventArgs pArgs) {
            ScriptForm script = pSender as ScriptForm;
            script.Packet.Selected = true;
            try {
                MainForm.StructureForm.ParseFiestaPacket(script.Packet);
                Activate();
            } catch {
                // If Session Tab is closed, while Script window was opened from it and then closed, then we arrive here
            }
        }

        private void mPacketContextMenu_Opening(object pSender, CancelEventArgs pArgs) {
            mPacketContextNameBox.Text = "";
            //mPacketContextIgnoreMenu.Checked = false;
            if( mPacketList.SelectedItems.Count == 0 ) pArgs.Cancel = true;
            else {
                FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
                Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                if( definition != null ) {
                    mPacketContextNameBox.Text = definition.Name;
                    if( definition.Ignore ) {
                        mPacketContextIgnoreMenu.Checked = true;
                    } else {
                        mPacketContextIgnoreMenu.Checked = false;
                    }
                }
            }
        }
        private void mPacketContextMenu_Opened(object pSender, EventArgs pArgs) {
            mPacketContextNameBox.Focus();
            mPacketContextNameBox.SelectAll();
        }
        private void mPacketContextNameBox_KeyDown(object pSender, KeyEventArgs pArgs) {
            if( pArgs.Modifiers == Keys.None && pArgs.KeyCode == Keys.Enter ) {
                FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
                Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
                if( definition == null ) {
                    definition = new Definition();
                    definition.Outbound = packet.Outbound;
                    definition.Opcode = packet.Opcode;
                    Config.Instance.Definitions.Add(definition);
                }
                definition.Name = mPacketContextNameBox.Text;
                Config.Instance.Save();
                pArgs.SuppressKeyPress = true;
                mPacketContextMenu.Close();
                RefreshPackets();
            }
        }
        private void mPacketContextIgnoreMenu_CheckedChanged(object pSender, EventArgs pArgs) {
            FiestaPacket packet = mPacketList.SelectedItems[0] as FiestaPacket;
            Definition definition = Config.Instance.Definitions.Find(d => d.Outbound == packet.Outbound && d.Opcode == packet.Opcode);
            if( definition == null ) {
                definition = new Definition();
                definition.Outbound = packet.Outbound;
                definition.Opcode = packet.Opcode;
                Config.Instance.Definitions.Add(definition);
            }
            definition.Ignore = mPacketContextIgnoreMenu.Checked;
            Config.Instance.Save();
            RefreshPackets();
        }
    }
}