using Sm4shCommand.GUI.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;

namespace Sm4shCommand.GUI.Editors
{
    public abstract class EditorBase : DockContent
    {
        public bool HasChanges { get; set; }

        public abstract bool Save();
        public abstract bool Save(string filename);

        public ProjectExplorerNode LinkedNode { get; set; }
    }
}
