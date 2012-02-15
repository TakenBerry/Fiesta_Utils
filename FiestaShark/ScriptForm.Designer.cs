namespace MapleShark
{
    partial class ScriptForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptForm));
            this.mSaveButton = new System.Windows.Forms.Button();
            this.mScriptEditor = new Alsing.Windows.Forms.SyntaxBoxControl();
            this.mScriptSyntax = new Alsing.SourceCode.SyntaxDocument(this.components);
            this.mSaveCloseButton = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mSaveButton
            // 
            this.mSaveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSaveButton.Location = new System.Drawing.Point(0, 0);
            this.mSaveButton.Name = "mSaveButton";
            this.mSaveButton.Size = new System.Drawing.Size(520, 25);
            this.mSaveButton.TabIndex = 5;
            this.mSaveButton.Text = "Save";
            this.mSaveButton.UseVisualStyleBackColor = true;
            this.mSaveButton.Click += new System.EventHandler(this.mSaveButton_Click);
            // 
            // mScriptEditor
            // 
            this.mScriptEditor.ActiveView = Alsing.Windows.Forms.ActiveView.BottomRight;
            this.mScriptEditor.AutoListPosition = null;
            this.mScriptEditor.AutoListSelectedText = "a123";
            this.mScriptEditor.AutoListVisible = false;
            this.mScriptEditor.BackColor = System.Drawing.Color.White;
            this.mScriptEditor.BorderStyle = Alsing.Windows.Forms.BorderStyle.None;
            this.mScriptEditor.CopyAsRTF = false;
            this.mScriptEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mScriptEditor.Document = this.mScriptSyntax;
            this.mScriptEditor.FontName = "Courier new";
            this.mScriptEditor.HighLightActiveLine = true;
            this.mScriptEditor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mScriptEditor.Indent = Alsing.Windows.Forms.SyntaxBox.IndentStyle.Scope;
            this.mScriptEditor.InfoTipCount = 1;
            this.mScriptEditor.InfoTipPosition = null;
            this.mScriptEditor.InfoTipSelectedIndex = 1;
            this.mScriptEditor.InfoTipVisible = false;
            this.mScriptEditor.Location = new System.Drawing.Point(0, 0);
            this.mScriptEditor.LockCursorUpdate = false;
            this.mScriptEditor.Name = "mScriptEditor";
            this.mScriptEditor.ParseOnPaste = true;
            this.mScriptEditor.ShowScopeIndicator = false;
            this.mScriptEditor.Size = new System.Drawing.Size(694, 258);
            this.mScriptEditor.SmoothScroll = false;
            this.mScriptEditor.SplitviewH = -4;
            this.mScriptEditor.SplitviewV = -4;
            this.mScriptEditor.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(219)))), ((int)(((byte)(214)))));
            this.mScriptEditor.TabIndex = 0;
            this.mScriptEditor.TabsToSpaces = true;
            this.mScriptEditor.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            // 
            // mScriptSyntax
            // 
            this.mScriptSyntax.Lines = new string[] {
        ""};
            this.mScriptSyntax.MaxUndoBufferSize = 1000;
            this.mScriptSyntax.Modified = false;
            this.mScriptSyntax.UndoStep = 0;
            // 
            // mSaveCloseButton
            // 
            this.mSaveCloseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSaveCloseButton.Location = new System.Drawing.Point(0, 0);
            this.mSaveCloseButton.Name = "mSaveCloseButton";
            this.mSaveCloseButton.Size = new System.Drawing.Size(173, 25);
            this.mSaveCloseButton.TabIndex = 6;
            this.mSaveCloseButton.Text = "Save && Close";
            this.mSaveCloseButton.UseVisualStyleBackColor = true;
            this.mSaveCloseButton.Click += new System.EventHandler(this.mSaveCloseButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.mScriptEditor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(694, 284);
            this.splitContainer1.SplitterDistance = 258;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 7;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.mSaveButton);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.mSaveCloseButton);
            this.splitContainer2.Size = new System.Drawing.Size(694, 25);
            this.splitContainer2.SplitterDistance = 520;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 7;
            this.splitContainer2.TabStop = false;
            // 
            // ScriptForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 284);
            this.Controls.Add(this.splitContainer1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptForm";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Float;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Script";
            this.Load += new System.EventHandler(this.ScriptForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Alsing.Windows.Forms.SyntaxBoxControl mScriptEditor;
        private Alsing.SourceCode.SyntaxDocument mScriptSyntax;
        private System.Windows.Forms.Button mSaveButton;
        private System.Windows.Forms.Button mSaveCloseButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;

    }
}