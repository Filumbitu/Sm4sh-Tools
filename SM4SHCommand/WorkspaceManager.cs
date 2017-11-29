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
            Tree = tree;
        }

        public Workspace TargetWorkspace { get; set; }
        private WorkspaceExplorer Tree { get; set; }

        public void OpenWorkspace(string filepath)
        {
            TargetWorkspace = new Workspace();
            TargetWorkspace.WorkspaceFile = new XmlDocument();
            TargetWorkspace.WorkspaceFile.Load(filepath);

            TargetWorkspace.WorkspaceRoot = Path.GetDirectoryName(filepath);

            var rootNode = TargetWorkspace.WorkspaceFile.SelectSingleNode("//Workspace");

            TargetWorkspace.WorkspaceName = rootNode.Attributes["Name"].Value;
            var nodes = TargetWorkspace.WorkspaceFile.SelectNodes("//Workspace//Project");
            foreach (XmlNode node in nodes)
            {
                string projectPath = Util.CanonicalizePath(Path.Combine(TargetWorkspace.WorkspaceRoot, node.Attributes["Path"].Value));
                var proj = ReadProjectFile(projectPath);
                proj.ProjectGuid = Guid.Parse(node.Attributes["GUID"].Value);
                TargetWorkspace.Projects.Add(proj.ProjectGuid, proj);
            }
            PopulateTreeView();
        }
        public void CloseWorkspace()
        {
            TargetWorkspace.WorkspaceFile = null;
            TargetWorkspace.TargetProject = null;
            TargetWorkspace.WorkspaceName = string.Empty;
            TargetWorkspace.WorkspaceRoot = string.Empty;
            TargetWorkspace.Projects.Clear();
            Tree.treeView1.Nodes.Clear();
        }

        public void RemoveProject(Project p)
        {
            TargetWorkspace.Projects.Remove(p.ProjectGuid);
            var nodes = TargetWorkspace.WorkspaceFile.SelectNodes("//Workspace//Project");
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["Name"].Value == p.ProjName)
                {
                    TargetWorkspace.WorkspaceFile.SelectSingleNode("//Workspace").RemoveChild(node);
                }
            }
        }

        public void OpenProject(string filename)
        {
            Util.LogMessage($"Opening project {filename}..");
            var p = ReadProjectFile(filename);
            TargetWorkspace.Projects.Add(p.ProjectGuid, p);
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
            if (!string.IsNullOrEmpty(TargetWorkspace.WorkspaceName))
            {
                workspaceNode = new TreeNode(TargetWorkspace.WorkspaceName);
                workspaceNode.ImageIndex = workspaceNode.SelectedImageIndex = 2;
            }

            foreach (var pair in TargetWorkspace.Projects)
            {
                Project proj = pair.Value;
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
                            ProjectExplorerNode node = null;
                            if (projItem.IsDirectory)
                            {
                                node = new ProjectFolderNode() { Text = part };
                                node.Tag = new DirectoryInfo(itmRelativePath);
                                if (!Directory.Exists(itmRelativePath))
                                {
                                    Util.LogMessage($"Couldn't find part of the path: \"{itmRelativePath}\"", ConsoleColor.Red);
                                    node.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                            else
                            {
                                node = new ProjectFileNode() { Text = part };
                                node.Tag = new FileInfo(itmRelativePath);
                                if (!File.Exists(itmRelativePath))
                                {
                                    Util.LogMessage($"Couldn't find part of the path: \"{itmRelativePath}\"", ConsoleColor.Red);
                                    node.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                            node.Name = treePath;
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