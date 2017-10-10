using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SALT.Moveset;
using SALT.Moveset.AnimCMD;
using SALT.Moveset.MSC;
using System.Xml;
using SALT.PARAMS;
using Sm4shCommand.GUI;
using Sm4shCommand.GUI.Nodes;
using System.ComponentModel;

namespace Sm4shCommand
{
    public class WorkspaceManager
    {
        public WorkspaceManager(WorkspaceExplorer tree)
        {
            Projects = new SortedList<string, Project>();
            Tree = tree;
        }

        public XmlDocument WorkspaceFile { get; set; }

        public SortedList<string, Project> Projects { get; set; }

        private WorkspaceExplorer Tree { get; set; }
        public string WorkspaceRoot { get; set; }
        public string TargetProject { get; set; }
        public string WorkspaceName { get; set; }

        public void OpenWorkspace(string filepath)
        {
            WorkspaceFile = new XmlDocument();
            WorkspaceFile.Load(filepath);

            WorkspaceRoot = Path.GetDirectoryName(filepath);

            var rootNode = WorkspaceFile.SelectSingleNode("//Workspace");

            WorkspaceName = rootNode.Attributes["Name"].Value;
            var nodes = WorkspaceFile.SelectNodes("//Workspace//Project");
            foreach (XmlNode node in nodes)
            {
                var proj = ReadProjectFile(Path.Combine(WorkspaceRoot, node.Attributes["Path"].Value));
                proj.ProjName = node.Attributes["Name"].Value;
                Projects.Add(proj.ProjName, proj);
            }
            PopulateTreeView();
        }

        public void RemoveProject(Project p)
        {
            Projects.Remove(p.ProjName);
            var nodes = WorkspaceFile.SelectNodes("//Workspace//Project");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["Name"].Value == p.ProjName)
                {
                    WorkspaceFile.SelectSingleNode("//Workspace").RemoveChild(node);
                }
            }
        }
        public void RemoveProject(string name)
        {
            RemoveProject(Projects[name]);
        }

        public void OpenProject(string filename)
        {
            var p = ReadProjectFile(filename);
            Projects.Add(p.ProjName, p);
            PopulateTreeView();
        }
        private Project ReadProjectFile(string filepath)
        {
            var proj = new Project();
            if (filepath.EndsWith(".fitproj", StringComparison.InvariantCultureIgnoreCase))
            {
                proj = new FitProj();
            }
            proj.ReadProject(filepath);
            if (string.IsNullOrWhiteSpace(proj.ProjName))
                proj.ProjName = Path.GetFileNameWithoutExtension(filepath);

            return proj;
        }

        public void PopulateTreeView()
        {
            Tree.treeView1.BeginUpdate();
            TreeNode workspaceNode = null;

            // If we're actually opening a full workspace and
            // not just a single project, add all projects
            // as children to the workspace
            if (!string.IsNullOrEmpty(WorkspaceName))
            {
                workspaceNode = new TreeNode(WorkspaceName);
                workspaceNode.ImageIndex = workspaceNode.SelectedImageIndex = 2;
            }

            foreach (var pair in Projects)
            {
                FitProj p = (FitProj)pair.Value;

                FileInfo fileinfo = new FileInfo(p.ProjFilepath);
                var projNode = new ProjectNode(p);
                projNode.Tag = fileinfo;

                GetDirectories(new DirectoryInfo(p.ProjDirectory).GetDirectories(), projNode, p);
                GetFiles(new DirectoryInfo(p.ProjDirectory), projNode, p);

                if (workspaceNode != null)
                    workspaceNode.Nodes.Add(projNode);
                else
                    Tree.treeView1.Nodes.Add(projNode);
            }
            if (workspaceNode != null)
                Tree.treeView1.Nodes.Add(workspaceNode);

            Tree.treeView1.EndUpdate();
        }

        private void GetDirectories(DirectoryInfo[] subDirs, ProjectFolderNode nodeToAddTo, FitProj p)
        {
            ProjectFolderNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new ProjectFolderNode() { Text = subDir.Name };
                aNode.Tag = subDir;
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode, p);
                }
                GetFiles(subDir, aNode, p);
                nodeToAddTo.Nodes.Add(aNode);
            }
        }
        private void GetFiles(DirectoryInfo dir, ProjectFolderNode nodeToAddTo, FitProj p)
        {
            foreach (var fileinfo in dir.GetFiles())
            {
                if (fileinfo.Name.EndsWith(".fitproj", StringComparison.InvariantCultureIgnoreCase))
                    break;

                var child = new ProjectFileNode() { Text = fileinfo.Name };
                child.Tag = fileinfo;
                foreach (var f in p.IncludedFiles)
                {
                    if (fileinfo.FullName.Contains(f.RelativePath))
                    {
                        nodeToAddTo.Nodes.Add(child);
                        break;
                    }
                }
            }
        }
    }
}
