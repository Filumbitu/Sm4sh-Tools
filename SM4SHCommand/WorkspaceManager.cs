using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SALT.Moveset;
using SALT.Moveset.AnimCMD;
using SALT.Moveset.MSC;
using System.Xml;
using System.Linq;
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
            Util.LogMessage($"Opening project {filename}..");
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
                FitProj proj = (FitProj)pair.Value;
                FileInfo fileinfo = new FileInfo(proj.ProjFilepath);
                var projNode = new ProjectNode(proj);
                projNode.Tag = fileinfo;

                ProjectExplorerNode nodeToAddTo = projNode;
                foreach (var projItem in proj.Includes)
                {
                    string pathAggregate = string.Empty;
                    string[] pathParts = projItem.RelativePath.Split(Path.DirectorySeparatorChar).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        string part = pathParts[i];
                        pathAggregate = Path.Combine(pathAggregate, pathParts[i]);
                        string treePath = Path.Combine(projNode.Text, pathAggregate);
                        string itmRelativePath = Path.Combine(proj.ProjDirectory, pathAggregate);

                        if (i == pathParts.Length - 1)
                        {
                            var node = new ProjectFileNode() { Text = part };
                            node.Name = treePath;
                            node.Tag = new FileInfo(itmRelativePath);
                            if (!File.Exists(itmRelativePath))
                            {
                                Util.LogMessage($"Couldn't find part of the path: \"{itmRelativePath}\"", ConsoleColor.Red);
                                node.ForeColor = System.Drawing.Color.Red;
                            }
                            nodeToAddTo.Nodes.Add(node);
                        }
                        else if (nodeToAddTo.Nodes.Find(treePath, true).Length > 0)
                        {
                            nodeToAddTo = (ProjectExplorerNode)nodeToAddTo.Nodes.Find(treePath, true)[0];
                        }
                        else
                        {
                            var node = new ProjectFolderNode() { Text = part };
                            node.Tag = new DirectoryInfo(itmRelativePath);
                            node.Name = treePath;
                            if (!Directory.Exists(itmRelativePath))
                            {
                                Util.LogMessage($"Couldn't find part of the path: \"{itmRelativePath}\"", ConsoleColor.Red);
                                node.ForeColor = System.Drawing.Color.Red;
                            }
                            nodeToAddTo.Nodes.Add(node);
                            nodeToAddTo = node;
                        }

                    }
                    nodeToAddTo = projNode;
                }

                foreach (var projItem in proj.ProjectFolders)
                {
                    string pathAggregate = string.Empty;
                    string[] pathParts = projItem.RelativePath.Split(Path.DirectorySeparatorChar).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        string part = pathParts[i];
                        pathAggregate = Path.Combine(pathAggregate, pathParts[i]);
                        string treePath = Path.Combine(projNode.Text, pathAggregate);
                        string itmRelativePath = Path.Combine(proj.ProjDirectory, pathAggregate);

                        if (i == pathParts.Length - 1)
                        {

                            var node = new ProjectFolderNode() { Text = part };
                            node.Name = treePath;
                            node.Tag = new DirectoryInfo(itmRelativePath);
                            if (!Directory.Exists(itmRelativePath))
                            {
                                Util.LogMessage($"Directory not found:\"{itmRelativePath}\"", ConsoleColor.Red);
                                node.ForeColor = System.Drawing.Color.Red;
                            }
                            nodeToAddTo.Nodes.Add(node);
                        }
                        else if (nodeToAddTo.Nodes.Find(treePath, true).Length > 0)
                        {
                            nodeToAddTo = (ProjectExplorerNode)nodeToAddTo.Nodes.Find(treePath, true)[0];
                        }
                    }
                    nodeToAddTo = projNode;
                }

                if (workspaceNode != null)
                    workspaceNode.Nodes.Add(projNode);
                else
                    Tree.treeView1.Nodes.Add(projNode);
            }
            if (workspaceNode != null)
                Tree.treeView1.Nodes.Add(workspaceNode);

            Tree.treeView1.EndUpdate();
        }
    }
}