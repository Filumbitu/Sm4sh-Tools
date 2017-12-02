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
            Includes = new List<ProjectItem>();
        }

        // Project Properties
        public Guid ProjectGuid { get; set; }
        public XmlDocument ProjFile { get; set; }
        public string ProjFilepath { get; set; }
        public string ProjDirectory { get { return Path.GetDirectoryName(ProjFilepath); } }
        public string ProjName { get; set; }
        public string ToolVer { get; set; }
        public string GameVer { get; set; }
        public ProjPlatform Platform { get; set; }

        public List<ProjectItem> Includes { get; set; }

        public ProjectItem GetFile(string path)
        {
            return this[path];
        }
        public bool RemoveFile(ProjectItem item)
        {
            bool result = Includes.Remove(item);
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

        public void AddFile(string filepath, bool saveProject)
        {
            var item = new ProjectItem
            {
                RealPath = filepath,
                RelativePath = filepath.Replace(ProjDirectory, "").TrimStart(Path.DirectorySeparatorChar)
            };
            Includes.Add(item);

            if (saveProject)
                SaveProject();
        }
        public void AddFile(string filepath)
        {
            AddFile(filepath, true);
        }
        public void RemoveFolder(string path)
        {
            // enumerate over COPIES of the list so we don't
            // change the enumaration target when removing
            foreach (var file in Includes.ToList())
            {
                if (file.RelativePath.StartsWith(path))
                    Includes.Remove(file);
            }
            SaveProject();
        }
        public void AddFolder(string path)
        {
            var item = new ProjectItem
            {
                RealPath = path,
                RelativePath = path.Replace(ProjDirectory, "").TrimStart(Path.DirectorySeparatorChar),
                IsDirectory = true
            };
            Includes.Add(item);
            SaveProject();
        }

        public void RenameFile(string filepath, string oldname, string newname)
        {
            var entry = Includes.FirstOrDefault(x => x.RealPath == filepath || x.RelativePath == filepath);
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
            foreach (var entry in Includes)
            {
                // split the entry's path so we can index
                // into the correct node and rename it
                string[] indexedPath = entry.RelativePath.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();
                if (depth >= indexedPath.Length)
                    continue;

                if (indexedPath[depth] == oldname)
                {
                    indexedPath[depth] = newname;
                    entry.RelativePath = string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath);
                    entry.RealPath = Path.Combine(this.ProjDirectory, string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath));
                }
            }
            SaveProject();
        }
        public ProjectItem this[string key]
        {
            get
            {
                return Includes.FirstOrDefault(x => x.RelativePath == key);
            }
        }

        public virtual void RenameProject(string newname) { }
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
            try
            {
                Util.LogMessage($"Loading project {filepath}..");
                ProjFilepath = filepath;
                var proj = new XmlDocument();
                proj.Load(filepath);

                var node = proj.SelectSingleNode("//Project");
                this.ToolVer = node.Attributes["ToolVersion"].Value;
                this.GameVer = node.Attributes["GameVersion"].Value;
                this.ProjName = node.Attributes["Name"].Value;
                this.ProjectGuid = Guid.Parse(proj.SelectSingleNode("//Project/ProjectGUID").InnerText);

                if (node.Attributes["Platform"].Value == "WiiU")
                    this.Platform = ProjPlatform.WiiU;
                else if (node.Attributes["Platform"].Value == "3DS")
                    this.Platform = ProjPlatform.ThreeDS;

                var nodes = proj.SelectNodes("//Project/FileGroup");
                foreach (XmlNode n in nodes)
                {
                    foreach (XmlNode child in n.ChildNodes)
                    {
                        var item = new ProjectItem
                        {
                            RelativePath = Util.CanonicalizePath(child.Attributes["Include"].Value)
                        };
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
                            item.IsDirectory = true;
                            Includes.Add(item);
                        }
                        else if (child.Name == "Content")
                        {
                            Includes.Add(item);
                        }
                    }
                }
                ProjFile = proj;
            }
            catch (Exception e)
            {
                Util.LogMessage($"Error loading project: {e.Message}", ConsoleColor.Red);
            }
        }
        public override void SaveProject(string filepath)
        {
            var writer = XmlWriter.Create(filepath, new XmlWriterSettings() { Indent = true, IndentChars = "\t" });
            writer.WriteStartDocument();
            writer.WriteStartElement("Project");
            writer.WriteAttributeString("Name", ProjName);
            writer.WriteAttributeString("ToolVersion", ToolVer);
            writer.WriteAttributeString("GameVersion", GameVer);
            writer.WriteAttributeString("Platform", Enum.GetName(typeof(ProjPlatform), Platform));
            writer.WriteStartElement("ProjectGUID");
            writer.WriteString(ProjectGuid.ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("FileGroup");
            foreach (var item in Includes.Where(x => x.IsDirectory))
            {
                writer.WriteStartElement("Folder");
                writer.WriteAttributeString("Include", item.RelativePath);
                writer.WriteEndElement();
            }
            // Dont need to write new start and 
            // end element unless we structurally
            // seperate included files from folders
            writer.WriteEndElement();

            writer.WriteStartElement("FileGroup");
            foreach (var item in Includes.Where(x => !x.IsDirectory))
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
        public override void RenameProject(string newname)
        {
            ProjName = newname;
            string oldPath = ProjFilepath;

            string[] indexedPath = ProjFilepath.Split(Path.DirectorySeparatorChar).SkipWhile(x => string.IsNullOrEmpty(x)).ToArray();
            indexedPath[indexedPath.Length - 1] = $"{newname}.fitproj";
            ProjFilepath = string.Join(Path.DirectorySeparatorChar.ToString(), indexedPath);
            File.Move(oldPath, ProjFilepath);
            SaveProject();
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
        public bool IsDirectory { get; set; }
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
