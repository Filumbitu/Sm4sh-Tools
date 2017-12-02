namespace Sm4shCommand.GUI.Editors
{
    partial class CodeEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeEditor));
            this.ITS_EDITOR1 = new Sm4shCommand.ITS_EDITOR();
            ((System.ComponentModel.ISupportInitialize)(this.ITS_EDITOR1)).BeginInit();
            this.SuspendLayout();
            // 
            // ITS_EDITOR1
            // 
            this.ITS_EDITOR1.AutoCompleteBrackets = true;
            this.ITS_EDITOR1.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.ITS_EDITOR1.AutoScrollMinSize = new System.Drawing.Size(115, 14);
            this.ITS_EDITOR1.BackBrush = null;
            this.ITS_EDITOR1.CharHeight = 14;
            this.ITS_EDITOR1.CharWidth = 8;
            this.ITS_EDITOR1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ITS_EDITOR1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.ITS_EDITOR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ITS_EDITOR1.IsReplaceMode = false;
            this.ITS_EDITOR1.Location = new System.Drawing.Point(0, 0);
            this.ITS_EDITOR1.Name = "ITS_EDITOR1";
            this.ITS_EDITOR1.Paddings = new System.Windows.Forms.Padding(0);
            this.ITS_EDITOR1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.ITS_EDITOR1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("ITS_EDITOR1.ServiceColors")));
            this.ITS_EDITOR1.Size = new System.Drawing.Size(284, 262);
            this.ITS_EDITOR1.TabIndex = 0;
            this.ITS_EDITOR1.Zoom = 100;
            // 
            // CodeEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.ITS_EDITOR1);
            this.Name = "CodeEditor";
            ((System.ComponentModel.ISupportInitialize)(this.ITS_EDITOR1)).EndInit();
            this.ResumeLayout(false);

        }

        private ITS_EDITOR ITS_EDITOR1;
    }
}
