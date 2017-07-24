using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Sm4shCommand.GUI
{
    public partial class WorkspaceExplorer : DockContent
    {
        public WorkspaceExplorer()
        {
            InitializeComponent();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = treeView1.GetNodeAt(e.Location);
        }
    }
}
