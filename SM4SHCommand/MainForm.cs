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
    public partial class AcmdMain : Form
    {
        public static AcmdMain Instance
        {
            get { return _instance != null ? _instance : (_instance = new AcmdMain()); }
        }
        private static AcmdMain _instance;

        public AcmdMain()
        {
            InitializeComponent();
            this.Text = $"{Program.AssemblyTitle} {Program.Version} - ";
        }

        public const string FileFilter =
                              "All Supported Files (*.bin, *.mscsb)|*.bin;*.mscsb|" +
                              "ACMD Binary (*.bin)|*.bin|" +
                              "MotionScript Binary (*.mscsb)|*.mscsb|" +
                              "All Files (*.*)|*.*";

        private OpenFileDialog ofDlg = new OpenFileDialog() { Filter = FileFilter };
        private SaveFileDialog sfDlg = new SaveFileDialog() { Filter = FileFilter };
        private FolderSelectDialog fsDlg = new FolderSelectDialog();

        public SortedList<string, IScriptCollection> ScriptFiles { get; set; }
        public ParamFile ParamFile { get; set; }
        public MTable MotionTable { get; set; }
        public IDE_MODE IDEMode { get; private set; }

        private void aboutToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var abtBox = new AboutBox();
            abtBox.ShowDialog();
        }

        private unsafe void fOpen_Click(object sender, EventArgs e)
        {
            if(ofDlg.ShowDialog() == DialogResult.OK)
            {

                AddDockedControl(new CodeEditor() { ShowHint = DockState.Document,TabText = "editor" });
            }
        }
        private void fitOpen_Click(object sender, EventArgs e)
        {

        }
        private void projOpen_Click(object sender, EventArgs e)
        {

        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void FileTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

        }
        private void FileTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            FileTree.SelectedNode = FileTree.GetNodeAt(e.Location);
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {

        }
        private void parseAnimationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
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

        public enum IDE_MODE
        {
            File,
            Fighter,
            Project,
            NONE
        }
    }
}