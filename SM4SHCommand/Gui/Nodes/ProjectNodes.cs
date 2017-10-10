using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sm4shCommand.GUI.Nodes
{
    public class ProjectExplorerNode : TreeNode
    {
        public static ContextMenuStrip _menu;
        static ProjectExplorerNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add("Delete", null, DeleteAction);
            _menu.Items.Add("Rename", null, RenameAction);
        }
        public ProjectExplorerNode()
        {
            this.ContextMenuStrip = _menu;
        }

        public virtual void DeleteFileOrFolder()
        {
            var result = MessageBox.Show($"Are you sure you want to delete {this.Text}? This cannot be undone!", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                if (this.Tag is DirectoryInfo)
                {
                    string name = ((DirectoryInfo)this.Tag).FullName;
                    ProjectNode.Project.RemoveFolder(this.FullPath.Replace(ProjectNode.Text, ""));
                    ((DirectoryInfo)this.Tag).Delete(true);
                }
                else if (this is ProjectNode)
                {
                    string name = ((FileInfo)this.Tag).FullName;
                    var n = this as ProjectNode;
                    MainForm.Instance.WorkspaceManager.RemoveProject(n.ProjectName);
                    ((FileInfo)this.Tag).Delete();
                }
                else if (this.Tag is FileInfo)
                {
                    string name = ((FileInfo)this.Tag).FullName;
                    ProjectNode.Project.RemoveFile(name);
                    ((FileInfo)this.Tag).Delete();
                }

                this.Remove();
            }
        }

        public virtual void BeginRename()
        {
            this.EnsureVisible();
            this.BeginEdit();
        }
        public virtual void EndRename(string newname) { }

        protected static void DeleteAction(object sender, EventArgs e)
        {
            GetInstance<ProjectExplorerNode>().DeleteFileOrFolder();
        }
        protected static void RenameAction(object sender, EventArgs e)
        {
            GetInstance<ProjectExplorerNode>().BeginRename();
        }
        protected static T GetInstance<T>() where T : TreeNode
        {
            return MainForm.Instance.Explorer.treeView1.SelectedNode as T;
        }

        public ProjectNode ProjectNode
        {
            get
            {
                TreeNode node = this;
                while (node != null)
                {
                    if (node is ProjectNode)
                        break;

                    node = node.Parent;
                }
                return (ProjectNode)node;
            }
        }
        public string ProjectRelativePath
        {
            get
            {
                return null;
            }
        }
    }

    public class ProjectFolderNode : ProjectExplorerNode
    {
        static ProjectFolderNode()
        {
            _menu.Items.Clear();
            _menu.Items.Add(new ToolStripMenuItem("Add", null,
                                                 new ToolStripMenuItem("New Item", null, NewFileAction),
                                                 new ToolStripMenuItem("New Folder", null, AddFolderAction),
                                                 new ToolStripMenuItem("Existing Item", null, ImportFileAction))

                           );
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Delete", null, DeleteAction);
        }
        private static void NewFileAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().NewFile();
        }
        private static void ImportFileAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().ImportFile();
        }
        private static void AddFolderAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().AddFolder();
        }

        public override void EndRename(string newname)
        {

            DirectoryInfo info = (DirectoryInfo)this.Tag;

            // remove project node from path
            string search = this.FullPath.ReplaceFirstOccurance(ProjectNode.Text, "");

            // Update text field for node.FullPath
            string oldname = this.Text;
            this.Text = newname;

            string replace = this.FullPath.ReplaceFirstOccurance(ProjectNode.Text, "");

            // move directory and update project file
            info.MoveTo(info.FullName.ReplaceFirstOccurance(search, replace));
            this.ProjectNode.Project.RenameDirectory(info.FullName, search, replace);
        }

        public void NewFile()
        {
            throw new NotImplementedException();
        }
        public void AddFolder()
        {
            int i = 0;
            foreach (TreeNode n in this.Nodes)
            {
                if (n.Text == $"NewFolder{i}")
                {
                    i++;
                }
                else break;
            }

            string path = "";
            if (this is ProjectNode)
                path = Path.Combine(Path.GetDirectoryName((((FileInfo)this.Tag).FullName)), $"NewFolder{i}");
            else
                path = Path.Combine((((DirectoryInfo)this.Tag).FullName), $"NewFolder{i}");

            ProjectNode.Project.AddFolder(path);
            Directory.CreateDirectory(path);
            var node = new ProjectFolderNode()
            {
                Tag = new DirectoryInfo(path)
            };
            node.Text = $"NewFolder{i}";
            Nodes.Add(node);
            node.EnsureVisible();
            node.BeginEdit();
        }

        public void ImportFile()
        {
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ProjectNode.Project.AddFile(ofd.FileName);
                    Nodes.Add(new ProjectFileNode() { Tag = new FileInfo(ofd.FileName) });
                }
            }
        }

        public ProjectFolderNode()
        {
            this.ImageIndex = this.SelectedImageIndex = 0;
        }
    }
    public class ProjectFileNode : ProjectExplorerNode
    {
        public ProjectFileNode()
        {
            this.ImageIndex = this.SelectedImageIndex = 1;
        }

        public override void EndRename(string newname)
        {
            FileInfo info = (FileInfo)this.Tag;
            string s = this.FullPath;
            s = s.Remove(0, s.IndexOf(Path.DirectorySeparatorChar));
            int indexOfRename = s.IndexOf(this.Text);
            this.ProjectNode.Project.RenameFile(info.FullName, this.Text, newname);
        }
    }

    // Inherit from folder as the proj file is treated as one
    public class ProjectNode : ProjectFolderNode
    {
        public ProjectNode(Project p)
        {
            this.Project = p;
            this.Text = p.ProjName;
            this.ContextMenuStrip = _menu;
            this.ImageIndex = this.SelectedImageIndex = 3;
        }

        public Project Project { get; private set; }
        public string ProjectName
        {
            get
            {
                return Project.ProjName;
            }
            set
            {
                Project.ProjName = value;
                this.Text = value;
            }
        }
    }
}
