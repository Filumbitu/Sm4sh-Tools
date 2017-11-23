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
                    ProjectNode.Project.RemoveFolder(this.FullPath.Replace(ProjectNode.Text, "").TrimStart(Path.DirectorySeparatorChar));
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
        public ProjectFolderNode()
        {
            this.ImageIndex = this.SelectedImageIndex = 0;
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

            // split original path and update the occurance
            // of the node to be renamed
            var indexedPath = search.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();

            int count = this.FullPath.Count(x => x == Path.DirectorySeparatorChar) - 1;
            indexedPath[count] = newname;

            // use new path to update project references and
            // move the file or directory
            info.MoveTo(Path.Combine(ProjectNode.Project.ProjDirectory, string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath)));
            this.ProjectNode.Project.RenameDirectory(info.FullName, count, oldname, newname);
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
                    foreach(TreeNode n in this.Nodes)
                    {
                        if (n.Text == ofd.SafeFileName)
                        {
                            MessageBox.Show("A file with this name already exists!");
                            return;
                        }
                    }

                    string path = "";
                    if (this is ProjectNode)
                        path = Path.Combine(Path.GetDirectoryName((((FileInfo)this.Tag).FullName)), ofd.SafeFileName);
                    else
                        path = Path.Combine((((DirectoryInfo)this.Tag).FullName), ofd.SafeFileName);

                    ProjectNode.Project.AddFile(path);
                    File.Copy(ofd.FileName, path);
                    var node = new ProjectFileNode()
                    {
                        Tag = new FileInfo(path)
                    };
                    node.Text = ofd.SafeFileName;
                    Nodes.Add(node);
                    node.EnsureVisible();
                }
            }
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
        public override void EndRename(string newname)
        {
            Project.RenameProject(newname);
        }
    }
}
