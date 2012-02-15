using ScriptNET;
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
    public partial class StructureForm: DockContent {
        private FiestaPacket mParsing = null;
        private Stack<StructureNode> mSubNodes = new Stack<StructureNode>();
        private bool showTypes = false;
        private bool showData = false;

        private string typeFormat = "[{0}] ";
        private string nameFormat = "{1}";
        private string dataFormat = " - {2}";
        private string tndFormat;
        private string dateFormat = "{0}/{1:00}/{2:00} {3:00}:{4:00}";

        public StructureForm() {
            InitializeComponent();
            UpdateFormatString();
        }
        private void UpdateFormatString() {
            tndFormat = (showTypes ? typeFormat : "") + nameFormat + (showData ? dataFormat : "");
        }

        public MainForm MainForm { get { return ParentForm as MainForm; } }
        public TreeView Tree { get { return mTree; } }

        public void ParseFiestaPacket(FiestaPacket pPacket) {
            if( pPacket == null ) return;
            mTree.Nodes.Clear();
            mSubNodes.Clear();
            pPacket.Rewind();

            string scriptPath = "Scripts" + Path.DirectorySeparatorChar + (pPacket.Outbound ? "Outbound" : "Inbound") + Path.DirectorySeparatorChar + "0x" + pPacket.Opcode.ToString("X4") + ".txt";
            string beforePath = "Scripts" + Path.DirectorySeparatorChar + "Global.txt";
            string afterPath = "Scripts" + Path.DirectorySeparatorChar + "Common.txt";
            if( File.Exists(scriptPath) ) {
                mParsing = pPacket;

                try {
                    StringBuilder scriptCode = new StringBuilder();
                    this.showTypes = false; this.showData = false;
                    if( File.Exists(beforePath) ) scriptCode.Append(File.ReadAllText(beforePath));
                    scriptCode.Append(Environment.NewLine + File.ReadAllText(scriptPath) + Environment.NewLine);
                    if( File.Exists(afterPath) ) scriptCode.Append(File.ReadAllText(afterPath));
                    Script script = Script.Compile(scriptCode.ToString());
                    script.Context.SetItem("ScriptAPI", new ScriptAPI(this));
                    script.Execute();
                } catch( Exception exc ) {
                    OutputForm output = new OutputForm("Script Error");
                    output.Append(exc.ToString());
                    output.Show(DockPanel, new Rectangle(MainForm.Location, new Size(400, 400)));
                }

                mParsing = null;
            }
            if( pPacket.Remaining > 0 ) mTree.Nodes.Add(new StructureNode("Undefined", pPacket.InnerBuffer, pPacket.Cursor, pPacket.Remaining));
        }

        private TreeNodeCollection CurrentNodes { get { return mSubNodes.Count > 0 ? mSubNodes.Peek().Nodes : mTree.Nodes; } }
        internal byte APIAddByte(string pName) {
            byte value;
            if( !mParsing.ReadByte(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Byte", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 1, 1));
            return value;
        }
        internal sbyte APIAddSByte(string pName) {
            sbyte value;
            if( !mParsing.ReadSByte(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "SByte", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 1, 1));
            return value;
        }
        internal ushort APIAddUShort(string pName) {
            ushort value;
            if( !mParsing.ReadUShort(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "UShort", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 2, 2));
            return value;
        }
        internal short APIAddShort(string pName) {
            short value;
            if( !mParsing.ReadShort(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Short", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 2, 2));
            return value;
        }
        internal uint APIAddUInt(string pName) {
            uint value;
            if( !mParsing.ReadUInt(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "UInt", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 4, 4));
            return value;
        }
        internal int APIAddInt(string pName) {
            int value;
            if( !mParsing.ReadInt(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Int", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 4, 4));
            return value;
        }
        internal ulong APIAddULong(string pName) {
            ulong value;
            if( !mParsing.ReadULong(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "ULong", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 8, 8));
            return value;
        }
        internal long APIAddLong(string pName) {
            long value;
            if( !mParsing.ReadLong(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Long", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 8, 8));
            return value;
        }

        internal float APIAddFloat(string pName) {
            float value;
            if( !mParsing.ReadFloat(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Float", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 4, 4));
            return value;
        }
        internal double APIAddDouble(string pName) {
            double value;
            if( !mParsing.ReadDouble(out value) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Double", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 8, 8));
            return value;
        }

        internal string APIAddString(string pName) {
            pName = String.Format(tndFormat, "String", pName, "");
            APIStartNode(pName);
            short size = APIAddShort("Size");
            string value = APIAddPaddedString("String", size);
            APIEndNode(false);
            return value;
        }
        internal string APIAddPaddedString(string pName, int pLength) {
            string value;
            if( !mParsing.ReadPaddedString(out value, pLength) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Char(" + pLength + ")", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - pLength, pLength));
            return value;
        }
        internal void APIAddField(string pName, int pLength) {
            byte[] buffer = new byte[pLength];
            if( !mParsing.ReadBytes(buffer) ) throw new Exception("Insufficient packet data");
            pName = String.Format(tndFormat, "Char(" + pLength + ")", pName, buffer);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - pLength, pLength));
        }

        internal string APIAddDate(string pName) {
            uint v;
            if( !mParsing.ReadUInt(out v) ) throw new Exception("Insufficient packet data");
            string value = String.Format(dateFormat, 2000 + (v & 0xFF), (v >> 8) & 0x1F, (v >> 13) & 0x3F, (v >> 19) & 0x3F, (v >> 25) & 0x7F);
            pName = String.Format(tndFormat, "Date", pName, value);
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor - 4, 4));
            return value;
        }

        internal void APIAddComment(string pComment) {
            pComment = (showTypes ? "// " : "") + pComment;
            CurrentNodes.Add(new StructureNode(pComment, mParsing.InnerBuffer, mParsing.Cursor, 0));
        }
        internal void APIStartNode(string pName) {
            StructureNode node = new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor, 0);
            if( mSubNodes.Count > 0 ) mSubNodes.Peek().Nodes.Add(node);
            else mTree.Nodes.Add(node);
            mSubNodes.Push(node);
        }
        internal void APIEndNode(bool pExpand) {
            if( mSubNodes.Count > 0 ) {
                StructureNode node = mSubNodes.Pop();
                node.Length = mParsing.Cursor - node.Cursor;
                if( pExpand ) node.Expand();
            }
        }
        internal int APIRemaining() { return mParsing.Remaining; }

        internal void APIShowTypes(bool pShow) { this.showTypes = pShow; UpdateFormatString(); }
        internal void APIShowData(bool pShow) { this.showData = pShow; UpdateFormatString(); }
        internal void APISetTypeFormat(string pFormat) { this.typeFormat = pFormat; UpdateFormatString(); }
        internal void APISetNameFormat(string pFormat) { this.nameFormat = pFormat; UpdateFormatString(); }
        internal void APISetDataFormat(string pFormat) { this.dataFormat = pFormat; UpdateFormatString(); }
        internal void APISetDateFormat(string pFormat) { this.dateFormat = pFormat; }

        // These functions return value, but won't increase Cursor postition
        internal byte APIReadByte() {
            byte value;
            if( !mParsing.ReadByte(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(1);
            return value;
        }
        internal sbyte APIReadSByte() {
            sbyte value;
            if( !mParsing.ReadSByte(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(1);
            return value;
        }
        internal ushort APIReadUShort() {
            ushort value;
            if( !mParsing.ReadUShort(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(2);
            return value;
        }
        internal short APIReadShort() {
            short value;
            if( !mParsing.ReadShort(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(2);
            return value;
        }
        internal uint APIReadUInt() {
            uint value;
            if( !mParsing.ReadUInt(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(4);
            return value;
        }
        internal int APIReadInt() {
            int value;
            if( !mParsing.ReadInt(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(4);
            return value;
        }
        internal ulong APIReadULong() {
            ulong value;
            if( !mParsing.ReadULong(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(8);
            return value;
        }
        internal long APIReadLong() {
            long value;
            if( !mParsing.ReadLong(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(8);
            return value;
        }

        internal float APIReadFloat() {
            float value;
            if( !mParsing.ReadFloat(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(4);
            return value;
        }
        internal double APIReadDouble() {
            double value;
            if( !mParsing.ReadDouble(out value) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(8);
            return value;
        }

        internal string APIReadPaddedString(int pLength) {
            string value;
            if( !mParsing.ReadPaddedString(out value, pLength) ) throw new Exception("Insufficient packet data");
            mParsing.ReverseCursor(pLength);
            return value;
        }

        // These functions add value to the list
        internal void APIAddVariable(string pName, ulong pValue) {
            string typeName = "ULong";
            if( (mParsing.Cursor + 8) > mParsing.InnerBuffer.Length ) { throw new Exception("Out of packet bounds (AddVariable)."); }
            pName = (showTypes ? typeName + " " : "") + pName + (showData ? " - " + pValue : "");
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor, 8));
            mParsing.ReverseCursor(-8);
        }
        internal void APIAddVariable(string pName, long pValue, int pType) {
            string typeName = "";
            switch( pType ) {
                case -1: typeName = "[SByte]"; break;
                case 1: typeName = "[Byte]"; break;
                case -2: typeName = "[Short]"; break;
                case 2: typeName = "[UShort]"; break;
                case -4: typeName = "[Int]"; break;
                case 4: typeName = "[UInt]"; break;
                case -8: typeName = "[Long]"; break;
                default: return;
            }
            int length = Math.Abs(pType);
            if( (mParsing.Cursor + length) > mParsing.InnerBuffer.Length ) { throw new Exception("Out of packet bounds (AddVariable)."); }
            pName = (showTypes ? typeName + " " : "") + pName + (showData ? " - " + pValue : "");
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor, length));
            mParsing.ReverseCursor(-length);
        }
        internal void APIAddVariable(string pName, double pValue, int pType) {
            string typeName = "";
            switch( pType ) {
                case 4: typeName = "[Float]"; break;
                case 8: typeName = "[Double]"; break;
                default: return;
            }
            int length = Math.Abs(pType);
            if( (mParsing.Cursor + length) > mParsing.InnerBuffer.Length ) { throw new Exception("Out of packet bounds (AddVariable)."); }
            pName = (showTypes ? typeName + " " : "") + pName + (showData ? " - " + pValue : "");
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor, length));
            mParsing.ReverseCursor(-length);
        }
        internal void APIAddVariable(string pName, string pValue, int pLength) {
            if( (mParsing.Cursor + pLength) > mParsing.InnerBuffer.Length ) { throw new Exception("Out of packet bounds (AddVariable)."); }
            pName = (showTypes ? "[Char(" + pLength + ")] " : "") + pName + (showData ? " - " + pValue : "");
            CurrentNodes.Add(new StructureNode(pName, mParsing.InnerBuffer, mParsing.Cursor, pLength));
            mParsing.ReverseCursor(-pLength);
        }



        private void mTree_AfterSelect(object pSender, TreeViewEventArgs pArgs) {
            StructureNode node = pArgs.Node as StructureNode;
            if( node == null ) { MainForm.DataForm.HexBox.SelectionLength = 0; MainForm.PropertyForm.Properties.SelectedObject = null; return; }
            MainForm.DataForm.HexBox.SelectionStart = node.Cursor;
            MainForm.DataForm.HexBox.SelectionLength = node.Length;
            MainForm.PropertyForm.Properties.SelectedObject = new StructureSegment(node.Buffer, node.Cursor, node.Length);
        }

        private void mTree_KeyDown(object pSender, KeyEventArgs pArgs) {
            MainForm.CopyPacketHex(pArgs);
        }
    }
}
