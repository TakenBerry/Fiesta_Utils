using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MapleShark
{
    public partial class ScriptForm : DockContent
    {
        private string mPath = "";
        private FiestaPacket mPacket = null;

        public ScriptForm(string pPath, FiestaPacket pPacket)
        {
            mPath = pPath;
            mPacket = pPacket;
            InitializeComponent();
            try {
                if( pPacket != null ) {
                    Text = "Script 0x" + pPacket.Opcode.ToString("X4") + ", " + (pPacket.Outbound ? "Outbound" : "Inbound");
                } else if( Path.GetFileName(pPath).Contains("Global") ) {
                    Text = "Global Script";
                } else if( Path.GetFileName(pPath).Contains("Common") ) {
                    Text = "Common Script";
                } else {
                    Text = "Script";
                }
            } catch (Exception e){
                // Shoudn't reach this
                MessageBox.Show("Exception in ScriptForm(). Make picture of it and post it in Forum." + Environment.NewLine + 
                    e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        internal FiestaPacket Packet { get { return mPacket; } }

        private void ScriptForm_Load(object pSender, EventArgs pArgs)
        {
            mScriptEditor.Document.SetSyntaxFromEmbeddedResource(Assembly.GetExecutingAssembly(), "MapleShark.ScriptSyntax.txt");
            if (File.Exists(mPath)) mScriptEditor.Open(mPath);
        }
        /*
        private void mScriptEditor_TextChanged(object pSender, EventArgs pArgs)
        {
            //mSaveButton.Enabled = true;
            //mSaveCloseButton.Enabled = true;
        }
        */
        private void mSaveButton_Click(object pSender, EventArgs pArgs)
        {
            if (mScriptEditor.Document.Text.Length == 0) File.Delete(mPath);
            else mScriptEditor.Save(mPath);
        }
        private void mSaveUpdateButton_Click(object pSender, EventArgs pArgs) {
            if( mScriptEditor.Document.Text.Length == 0 ) File.Delete(mPath);
            else mScriptEditor.Save(mPath);
            // Update packet
        }
        private void mSaveCloseButton_Click(object pSender, EventArgs pArgs) {
            if( mScriptEditor.Document.Text.Length == 0 ) File.Delete(mPath);
            else mScriptEditor.Save(mPath);
            Close();
        }

    }
}
