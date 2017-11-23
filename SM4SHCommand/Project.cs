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
using System.Linq;

namespace Sm4shCommand
{
    public class Project
    {
        public Project()
        {
            Includes = new List<string>();
            IncludedFiles = new List<ProjectItem>();
            IncludedFolders = new List<ProjectItem>();
            ProjectFolders = new List<string>();
        }

        // Project Properties
        public XmlDocument ProjFile { get; set; }
        public string ProjFilepath { get; set; }
        public string ProjDirectory { get { return Path.GetDirectoryName(ProjFilepath); } }
        public string ProjName { get; set; }
        public string ToolVer { get; set; }
        public string GameVer { get; set; }
        public ProjPlatform Platform { get; set; }

        public List<string> Includes { get; set; }
        public List<string> ProjectFolders { get; set; }

        public List<ProjectItem> IncludedFiles { get; set; }

        // Folders are only included here if empty.
        public List<ProjectItem> IncludedFolders { get; set; }

        public ProjectItem GetFile(string path)
        {
            return this[path];
        }
        public bool RemoveFile(ProjectItem item)
        {
            bool result = IncludedFiles.Remove(item);
            SaveProject();
            return result;
        }
        public bool RemoveFile(string path)
        {
            if (this[path] != null)
                return RemoveFile(this[path]);
            else
                return false;
        }

        public void AddFile(string filepath)
        {
            var item = new ProjectItem();
            item.RealPath = filepath;
            item.RelativePath = filepath.Replace(ProjDirectory, "");
            IncludedFiles.Add(item);
            SaveProject();
        }
        public void RemoveFolder(string path)
        {
            // enumerate over COPIES of the lists so we don't
            // change the enumaration target when removing
            foreach (var item in IncludedFolders.ToList())
            {
                if (item.RelativePath.StartsWith(path))
                    IncludedFolders.Remove(item);
            }
            foreach (var file in IncludedFiles.ToList())
            {
                if (file.RelativePath.StartsWith(path))
                    IncludedFiles.Remove(file);
            }
            SaveProject();
        }
        public void AddFolder(string path)
        {
            var item = new ProjectItem();
            item.RealPath = path;
            item.RelativePath = path.Replace(ProjDirectory, "");
            IncludedFolders.Add(item);
            SaveProject();
        }

        public void RenameFile(string filepath, string oldname, string newname)
        {
            var entry = IncludedFiles.FirstOrDefault(x => x.RealPath == filepath || x.RelativePath == filepath);
            entry.RelativePath = entry.RelativePath.Replace(oldname, newname);
            entry.RealPath = entry.RealPath.Replace(oldname, newname);
            if (entry.RealPath.EndsWith(".fitproj"))
            {
                File.Move(entry.RealPath, entry.RealPath.Remove(Util.CanonicalizePath(entry.RealPath).LastIndexOf(Path.DirectorySeparatorChar)) + newname + ".fitproj");
            }
            else
            {
                File.Move(filepath, entry.RealPath);
            }
            SaveProject();
        }
        public void RenameDirectory(string directory, int depth, string oldname, string newname)
        {
            foreach (var entry in IncludedFiles)
            {
                // split the entry's path so we can index
                // into the correct node and rename it
                string[] indexedPath = entry.RelativePath.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();
                if (depth >= indexedPath.Length)
                    continue;

                if (indexedPath[depth] == oldname)
                {
                    indexedPath[depth] = newname;
                    entry.RelativePath = Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath);
                    entry.RealPath = Path.Combine(this.ProjDirectory, string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath));
                }
            }
            foreach (var entry in IncludedFolders)
            {
                // split the entry's path so we can index
                // into the correct node and rename it
                string[] indexedPath = entry.RelativePath.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();
                if (depth >= indexedPath.Length)
                    continue;

                if (indexedPath[depth] == oldname)
                {
                    indexedPath[depth] = newname;
                    entry.RelativePath = Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath);
                    entry.RealPath = Path.Combine(this.ProjDirectory, string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath));
                }
            }
            SaveProject();
        }
        public ProjectItem this[string key]
        {
            get
            {
                return IncludedFiles.FirstOrDefault(x => x.RelativePath == key);
            }
        }

        public virtual void ReadProject(string filepath) { }
        public virtual void SaveProject(string filepath) { }
        public virtual void SaveProject()
        {
            SaveProject(ProjFilepath);
        }
    }
    public class FitProj : Project
    {
        public FitProj()
        {

        }
        public FitProj(string name)
        {
            ProjName = name;
        }
        public FitProj(string name, string filepath) : this(name)
        {
            ReadProject(filepath);
        }

        public override void ReadProject(string filepath)
        {
            ProjFilepath = filepath;
            var proj = new XmlDocument();
            proj.Load(filepath);

            var node = proj.SelectSingleNode("//Project");
            this.ToolVer = node.Attributes["ToolVer"].Value;
            this.GameVer = node.Attributes["GameVer"].Value;
            this.ProjName = node.Attributes["Name"].Value;

            if (node.Attributes["Platform"].Value == "WiiU")
                this.Platform = ProjPlatform.WiiU;
            else if (node.Attributes["Platform"].Value == "3DS")
                this.Platform = ProjPlatform.ThreeDS;

            var nodes = proj.SelectNodes("//Project/FileGroup");
            foreach (XmlNode n in nodes)
            {
                foreach (XmlNode child in n.ChildNodes)
                {
                    var item = new ProjectItem();
                    item.RelativePath = Util.CanonicalizePath(child.Attributes["Include"].Value);

                    item.RealPath = Util.CanonicalizePath(Path.Combine(ProjDirectory, item.RelativePath.Trim(Path.DirectorySeparatorChar)));
                    if (child.HasChildNodes)
                    {
                        // foreach (XmlNode child2 in child.ChildNodes)
                        // {
                        //     if (child2.LocalName == "DependsUpon")
                        //     {
                        //         var path = Util.CanonicalizePath(Path.Combine(Path.GetDirectoryName(item.RelativePath), child2.InnerText));
                        //         item.Depends.Add(IncludedFiles.Find(x => x.RelativePath == path));
                        //     }
                        // }
                    }
                    if (child.Name == "Folder")
                    {
                        ProjectFolders.Add(item.RelativePath);
                    }
                    else if (child.Name == "Content")
                    {
                        Includes.Add(item.RelativePath);
                    }
                }
            }
            ProjFile = proj;
        }
        public override void SaveProject(string filepath)
        {
            var writer = XmlWriter.Create(filepath, new XmlWriterSettings() { Indent = true, IndentChars = "\t" });
            writer.WriteStartDocument();
            writer.WriteStartElement("Project");
            writer.WriteAttributeString("Name", ProjName);
            writer.WriteAttributeString("ToolVer", ToolVer);
            writer.WriteAttributeString("GameVer", GameVer);
            writer.WriteAttributeString("Platform", Enum.GetName(typeof(ProjPlatform), Platform));

            writer.WriteStartElement("FileGroup");
            foreach (var item in IncludedFolders)
            {
                writer.WriteStartElement("Folder");
                writer.WriteAttributeString("Include", item.RelativePath);
                writer.WriteEndElement();
            }
            // Dont need to write new start and 
            // end element unless we structurally
            // seperate included files from folders
            //writer.WriteEndElement();

            //writer.WriteStartElement("FileGroup");
            foreach (var item in IncludedFiles)
            {
                writer.WriteStartElement("Content");
                writer.WriteAttributeString("Include", item.RelativePath);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();
            var doc = new XmlDocument();
            doc.Load(filepath);
            ProjFile = doc;
        }
    }
    public class ProjectItem
    {
        public ProjectItem()
        {
            Depends = new List<ProjectItem>();
        }
        public string RelativePath { get; set; }
        public string RealPath { get; set; }
        public bool IsDirectory
        {
            get
            {
                return RelativePath.EndsWith("/") || RelativePath.EndsWith("\\");
            }
        }
        public List<ProjectItem> Depends { get; set; }
        public override string ToString()
        {
            return RelativePath;
        }
    }

    public enum ProjPlatform
    {
        [Description("Wii U")]
        WiiU = 0,
        [Description("3DS")]
        ThreeDS = 1
    }
}
