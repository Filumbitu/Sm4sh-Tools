using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sm4shCommand.GUI.Nodes
{
    public class WorkspaceNode : ProjectFolderNode
    {
        private static ContextMenuStrip _menu;
        static WorkspaceNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Add", null,
                                                 new ToolStripMenuItem("New Project", null, AddProjectAction))

                           );
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Close Workspace", null, CloseWorkspaceAction);
        }
        public WorkspaceNode()
        {
            this.ContextMenuStrip = _menu;
            this.ImageIndex = this.SelectedImageIndex = 2;
        }
        protected static void AddProjectAction(object sender, EventArgs e)
        {
            GetInstance<WorkspaceNode>().AddProject();
        }
        protected static void CloseWorkspaceAction(object sender, EventArgs e)
        {
            GetInstance<WorkspaceNode>().CloseWorkspace();
        }
        public void CloseWorkspace()
        {
            MainForm.Instance.WorkspaceManager.CloseWorkspace();
        }
        public void AddProject()
        {
            using(var dlg = new NewProjectDialog())
            {
                if(dlg.ShowDialog() == DialogResult.OK)
                {

                }
            }
        }
    }
}
