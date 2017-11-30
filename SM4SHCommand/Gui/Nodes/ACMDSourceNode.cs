using Sm4shCommand.GUI.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sm4shCommand.GUI.Nodes
{
    class ACMDSourceNode : ProjectFileNode
    {
        public override EditorBase GetEditor()
        {
            return new CodeEditor(this) { Text = this.Text };
        }
    }
}
