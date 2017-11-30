using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sm4shCommand.GUI.Nodes
{
    public static class TreeNodeFactory
    {
        public static Dictionary<string, Func<ProjectExplorerNode>> SupportedFiles = new Dictionary<string, Func<ProjectExplorerNode>>()
        {
            {".acm",        () => new ACMDSourceNode()  },
            {".txt",        () => new ProjectFileNode() },
            {".png",        () => new ProjectFileNode() },
            {".fitproj",    () => new ProjectNode()     },
            {".wrkspc",     () => new WorkspaceNode()   }
        };
        public static ProjectExplorerNode NodeFromExtension(string extension)
        {
            if (SupportedFiles.ContainsKey(extension))
                return SupportedFiles[extension].Invoke();

            return new ProjectFileNode();
        }
    }
}
