using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Sm4shCommand
{
    public interface IProjectTemplate
    {
        string DisplayText { get; set; }
        string ProjDescription { get; set; }
        Image TemplateIcon { get; set; }

        Project CreateProject(string filepath, string name, WorkspaceManager manager);
    }

    public class EmptyProjectTemplate : IProjectTemplate
    {
        public EmptyProjectTemplate()
        {
            DisplayText = "New Empty Project";
            ProjDescription = "Creates a new Empty Project";
        }

        public string DisplayText { get; set; }
        public string ProjDescription { get; set; }
        public Image TemplateIcon { get; set; }

        public Project CreateProject(string filepath, string name, WorkspaceManager manager)
        {
            var p = new FitProj
            {
                ProjFilepath = filepath,
                ProjName = name,
                Platform = ProjPlatform.WiiU,
                ToolVer = Program.Version,
                GameVer = "1.1.7",
                ProjectGuid = Guid.NewGuid()
            };

            return p;
        }
    }
}
