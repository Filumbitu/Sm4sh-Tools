using Sm4shCommand.GUI.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SALT.Moveset.AnimCMD;

namespace Sm4shCommand.GUI.Nodes
{
    class ACMDSourceNode : ProjectFileNode
    {
        public override EditorBase GetEditor()
        {
            var items = new List<SubstringAutocompleteItem>(ACMD_INFO.CMD_NAMES.Count);
            foreach(uint key in ACMD_INFO.CMD_NAMES.Keys.ToArray())
            {
                string name = ACMD_INFO.CMD_NAMES[key];
                string desc = ACMD_INFO.CMD_DESC[key];

                SubstringAutocompleteItem item = new SubstringAutocompleteItem(name, true);
                item.ToolTipTitle = name;
                item.ToolTipText = desc;
                items.Add(item);
            }
            var editor = new CodeEditor(this) { Text = this.Text };
            editor.SetAutocomplete(items.ToArray());
            return editor;
        }
    }
}
