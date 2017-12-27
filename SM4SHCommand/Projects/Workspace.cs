using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sm4shCommand
{
    public class Workspace
    {
        public Workspace()
        {
            Projects = new SortedList<Guid, Project>();
        }

        public XmlDocument WorkspaceFile { get; set; }

        public SortedList<Guid, Project> Projects { get; set; }

        public string WorkspaceRoot { get; set; }
        public string TargetProject { get; set; }
        public string WorkspaceName { get; set; }
        public string WorkspaceFilePath { get; set; }

        public void SaveWorkspace()
        {
            SaveWorkspace(WorkspaceFilePath);
        }
        public void SaveWorkspace(string filepath)
        {
            var writer = XmlWriter.Create(filepath, new XmlWriterSettings() { Indent = true, IndentChars = "\t" });
            writer.WriteStartDocument();
            writer.WriteStartElement("Workspace");
            writer.WriteAttributeString("Name", WorkspaceName);
            foreach(var pair in Projects)
            {
                pair.Value.SaveProject();

                writer.WriteStartElement("Project");
                writer.WriteAttributeString("GUID", pair.Key.ToString());
                writer.WriteAttributeString("Path", Util.CanonicalizePath(pair.Value.ProjFilepath.ReplaceFirstOccurance(WorkspaceRoot, "")));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.Close();
            var doc = new XmlDocument();
            doc.Load(filepath);
            WorkspaceFile = doc;
        }
    }
}
