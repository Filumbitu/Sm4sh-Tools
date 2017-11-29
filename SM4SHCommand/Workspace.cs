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
    }
}
