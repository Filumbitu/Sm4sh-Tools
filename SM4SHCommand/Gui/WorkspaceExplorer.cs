using Sm4shCommand.GUI.Nodes;
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

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Text == e.Label || e.Label == null)
            {
                e.CancelEdit = true;
            }
            else
            {
                ((ProjectExplorerNode)e.Node).EndRename(e.Label);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var editor = ((ProjectExplorerNode)e.Node).GetEditor();
            if (editor != null)
            {
                MainForm.Instance.AddDockedControl(editor, DockState.Document);
            }
            
        }
    }
}
