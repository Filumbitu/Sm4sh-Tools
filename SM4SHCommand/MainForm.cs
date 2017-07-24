using Sm4shCommand.GUI;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using SALT.PARAMS;
using SALT.Moveset;
using SALT.Moveset.AnimCMD;
using System.ComponentModel;
using SALT.Moveset.MSC;
using WeifenLuo.WinFormsUI.Docking;

namespace Sm4shCommand
{
    public partial class MainForm : Form
    {
        public static MainForm Instance
        {
            get { return _instance != null ? _instance : (_instance = new MainForm()); }
        }
        private static MainForm _instance;

        public MainForm()
        {
            InitializeComponent();
            this.Text = $"{Program.AssemblyTitle} {Program.Version} BETA - ";
        }

        public const string FileFilter =
                              "All Supported Files (*.acm, *.fitproj, *.wrkspc)|*.bin;*.fitproj;*.wrkspc|" +
                              "ACMD Script (*.acm)|*.acm|" +
                              "Fighter Project (*.fitproj)|*.fitproj|" +
                              "Project Workspace (*.wrkspc)|*.wrkspc|" +
                              "All Files (*.*)|*.*";

        private OpenFileDialog ofDlg = new OpenFileDialog() { Filter = FileFilter };
        private SaveFileDialog sfDlg = new SaveFileDialog() { Filter = FileFilter };
        private FolderSelectDialog fsDlg = new FolderSelectDialog();

        internal WorkspaceExplorer Explorer { get; set; }
        internal WorkspaceManager WorkspaceManager { get; set; }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var abtBox = new AboutBox();
            abtBox.ShowDialog();
        }

        public void AddDockedControl(DockContent content)
        {
            if (dockPanel1.DocumentStyle == DocumentStyle.SystemMdi)
            {
                content.MdiParent = this;
                content.Show();
            }
            else
                content.Show(dockPanel1);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Explorer = new WorkspaceExplorer() { ShowHint = DockState.DockRight };
            AddDockedControl(Explorer);
            AddDockedControl(new CodeEditor() { ShowHint = DockState.Document ,TabText = "Editor"});
            WorkspaceManager = new WorkspaceManager(Explorer);
        }

        private void projectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewProjectDialog dlg = new NewProjectDialog();
            dlg.ShowDialog();
        }
    }
}