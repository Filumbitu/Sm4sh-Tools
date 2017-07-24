namespace Sm4shCommand.GUI
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
            this.itS_EDITOR1 = new Sm4shCommand.ITS_EDITOR();
            ((System.ComponentModel.ISupportInitialize)(this.itS_EDITOR1)).BeginInit();
            this.SuspendLayout();
            // 
            // itS_EDITOR1
            // 
            this.itS_EDITOR1.AutoCompleteBrackets = true;
            this.itS_EDITOR1.AutoCompleteBracketsList = new char[] {
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
            this.itS_EDITOR1.AutocompleteMenu = null;
            this.itS_EDITOR1.AutoScrollMinSize = new System.Drawing.Size(115, 14);
            this.itS_EDITOR1.BackBrush = null;
            this.itS_EDITOR1.CharHeight = 14;
            this.itS_EDITOR1.CharWidth = 8;
            this.itS_EDITOR1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.itS_EDITOR1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.itS_EDITOR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itS_EDITOR1.IsReplaceMode = false;
            this.itS_EDITOR1.Location = new System.Drawing.Point(0, 0);
            this.itS_EDITOR1.Name = "itS_EDITOR1";
            this.itS_EDITOR1.Paddings = new System.Windows.Forms.Padding(0);
            this.itS_EDITOR1.Script = null;
            this.itS_EDITOR1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.itS_EDITOR1.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("itS_EDITOR1.ServiceColors")));
            this.itS_EDITOR1.Size = new System.Drawing.Size(284, 262);
            this.itS_EDITOR1.TabIndex = 0;
            this.itS_EDITOR1.Text = "ITS_EDITOR1";
            this.itS_EDITOR1.Zoom = 100;
            // 
            // CodeEditor
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.itS_EDITOR1);
            this.Name = "CodeEditor";
            ((System.ComponentModel.ISupportInitialize)(this.itS_EDITOR1)).EndInit();
            this.ResumeLayout(false);

        }

        private ITS_EDITOR itS_EDITOR1;
    }
}
