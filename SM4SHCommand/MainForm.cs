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
using Sm4shCommand.GUI.Editors;
using System.Reflection;

namespace Sm4shCommand
{
    public partial class MainForm : Form
    {
        public static MainForm Instance
        {
            get { return _instance ?? new MainForm(); }
        }
        private static MainForm _instance;
        RecentFileHandler RecentFileHandler;
        internal string OpenTarget { get; set; }

        public MainForm()
        {
            InitializeComponent();
            this.Text = $"{Program.AssemblyTitle} {Program.Version} BETA - ";

            // prevents crashing if we dont have any components on mainform
            if (this.components == null)
                this.components = new Container();

            _instance = this;

            RecentFileHandler = new RecentFileHandler(this.components)
            {
                RecentFileToolStripItem = this.recentFilesStripMenuItem
            };
            recentFilesStripMenuItem.DropDownItemClicked += RecentFilesStripMenuItem_DropDownItemClicked;
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

        private void AboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var abtBox = new AboutBox();
            abtBox.ShowDialog();
        }

        public void AddDockedControl(DockContent content, DockState dock)
        {
            content.ShowHint = dock;
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

            Explorer = new WorkspaceExplorer();
            AddDockedControl(Explorer, DockState.DockRight);
            AddDockedControl(new CodeEditor() { TabText = "Editor" }, DockState.Document);
            WorkspaceManager = new WorkspaceManager(Explorer);

            if (!string.IsNullOrEmpty(OpenTarget))
                OpenFile(OpenTarget);
        }

        private void ProjectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NewProjectDialog dlg = new NewProjectDialog();
            dlg.ShowDialog();
        }

        /// <summary>
        /// Opens a file for reading by the application
        /// </summary>
        /// <param name="filename">Path to the file.</param>
        /// <returns>Returns true if successful</returns>
        private bool OpenFile(string filename)
        {
            try
            {
                switch (filename.Substring(filename.IndexOf('.')))
                {
                    case ".wrkspc":
                        WorkspaceManager.OpenWorkspace(filename);
                        break;
                    case ".fitproj":
                        WorkspaceManager.OpenProject(filename);
                        break;
                }
                RecentFileHandler.AddFile(ofDlg.FileName);
                return true;
            }
            catch (Exception x)
            {
                Util.LogMessage($"Error opening file {filename}: {x.Message}");
            }
            return false;
        }
        private void FOpen_Click(object sender, EventArgs e)
        {
            if (ofDlg.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofDlg.FileName);
            }
        }

        private void CloseWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkspaceManager.CloseWorkspace();
        }
        private void RecentFilesStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
           OpenFile(((RecentFileHandler.FileMenuItem)e.ClickedItem).FileName);
        }
    }
}