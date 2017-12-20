using Sm4shCommand.GUI.Editors;
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
        private static ContextMenuStrip _menu;
        public TreeNode RootNode
        {
            get
            {
                TreeNode n = this;
                if (this.Parent != null)
                {
                    while (n != null)
                        return n = n.Parent;
                }
                return n;
            }
        }
        static ProjectExplorerNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Delete", null, DeleteAction);
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
                    string path = ((DirectoryInfo)this.Tag).FullName;
                    ProjectNode.Project.RemoveFolder(Util.CanonicalizePath(this.FullPath.Replace(ProjectNode.FullPath, "").TrimStart(Path.DirectorySeparatorChar)));
                    if (Directory.Exists(path))
                        ((DirectoryInfo)this.Tag).Delete(true);
                }
                else if (this is ProjectNode)
                {
                    string path = ((FileInfo)this.Tag).FullName;
                    var n = this as ProjectNode;
                    MainForm.Instance.WorkspaceManager.RemoveProject(n.Project);
                    if (File.Exists(path))
                        ((FileInfo)this.Tag).Delete();
                }
                else if (this.Tag is FileInfo)
                {
                    string path = ((FileInfo)this.Tag).FullName;
                    ProjectNode.Project.RemoveFile(Util.CanonicalizePath(this.FullPath.Replace(ProjectNode.FullPath, "").TrimStart(Path.DirectorySeparatorChar)));
                    if (File.Exists(path))
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
        public virtual EditorBase GetEditor() { return null; }

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
    }

    public class ProjectFolderNode : ProjectExplorerNode
    {
        private static ContextMenuStrip _menu;
        static ProjectFolderNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Add", null,
                                                 new ToolStripMenuItem("Existing Item", null, ImportFileAction),
                                                 new ToolStripMenuItem("New Item", null, NewFileAction),
                                                 new ToolStripMenuItem("New Folder", null, AddFolderAction))

                           );
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Delete", null, DeleteAction);
        }
        public ProjectFolderNode()
        {
            this.ContextMenuStrip = _menu;
            this.ImageIndex = this.SelectedImageIndex = 0;
        }

        protected static void NewFileAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().NewFile();
        }
        protected static void ImportFileAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().ImportFile();
        }
        protected static void AddFolderAction(object sender, EventArgs e)
        {
            GetInstance<ProjectFolderNode>().AddFolder();
        }

        public override void EndRename(string newname)
        {
            DirectoryInfo info = (DirectoryInfo)this.Tag;

            // remove project node from path
            string search = this.FullPath.ReplaceFirstOccurance(ProjectNode.FullPath, "").TrimStart(Path.DirectorySeparatorChar);

            // Update text field for node.FullPath
            string oldname = this.Text;
            this.Text = newname;

            // split original path and update the occurance
            // of the node to be renamed
            var indexedPath = search.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();

            int count = search.Count(x => x == Path.DirectorySeparatorChar);
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
            using (var ofd = new OpenFileDialog() { Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.TreeView.BeginUpdate();

                    // Only query these properties once, iterating over 
                    // them is really slow due to getter functions
                    string[] safeNames = ofd.SafeFileNames;
                    string[] filenames = ofd.FileNames;

                    // Iterating over the copies has 
                    // minimal performance impact
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        string _sfname = safeNames[i];
                        string _fname = filenames[i];
                        foreach (TreeNode n in this.Nodes)
                        {
                            if (n.Text == _sfname)
                            {
                                MessageBox.Show("A file with this name already exists!");
                                return;
                            }
                        }

                        string path = "";
                        if (this is ProjectNode)
                            path = Path.Combine(Path.GetDirectoryName((((FileInfo)this.Tag).FullName)), _sfname);
                        else
                            path = Path.Combine((((DirectoryInfo)this.Tag).FullName), _sfname);

                        ProjectNode.Project.AddFile(path, false);
                        File.Copy(_fname, path);
                        var node = new ProjectFileNode()
                        {
                            Tag = new FileInfo(path)
                        };
                        node.Text = _sfname;
                        Nodes.Add(node);
                    }
                    this.TreeView.EndUpdate();
                    ProjectNode.Project.SaveProject();
                }
            }
        }
    }
    public class ProjectFileNode : ProjectExplorerNode
    {
        private static ContextMenuStrip _menu;
        static ProjectFileNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Delete", null, DeleteAction);
        }
        public ProjectFileNode()
        {
            this.ContextMenuStrip = _menu;
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
        private static ContextMenuStrip _menu;
        static ProjectNode()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Add", null,
                                                 new ToolStripMenuItem("Existing Item", null, ImportFileAction),
                                                 new ToolStripMenuItem("New Item", null, NewFileAction),
                                                 new ToolStripMenuItem("New Folder", null, AddFolderAction))

                           );
            _menu.Items.Add("Rename", null, RenameAction);
            _menu.Items.Add("Delete", null, DeleteAction);
        }
        public ProjectNode()
        {
            this.ContextMenuStrip = _menu;
            this.ImageIndex = this.SelectedImageIndex = 3;
        }
        public ProjectNode(Project p) : this()
        {
            this.Project = p;
            this.Text = p.ProjName;
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
            this.Text = newname;
        }
    }
}
