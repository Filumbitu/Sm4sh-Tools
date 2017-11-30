using WeifenLuo.WinFormsUI.Docking;
using System.Windows.Forms;
using Sm4shCommand.GUI.Nodes;
using System.Security.Cryptography;
using System.IO;

namespace Sm4shCommand.GUI.Editors
{
    public partial class CodeEditor : EditorBase
    {
        public CodeEditor()
        {
            InitializeComponent();
        }
        public CodeEditor(ProjectExplorerNode node) : this()
        {
            LinkedNode = node;
            if(node.Tag is FileInfo f)
            {
                using(StreamReader reader = File.OpenText(f.FullName))
                {
                    this.ITS_EDITOR1.Text = reader.ReadToEnd();
                }
            }
        }

        public override ProjectExplorerNode LinkedNode { get; set; }

        public override bool Save()
        {
            throw new System.NotImplementedException();
        }
        public override bool Save(string filename)
        {
            throw new System.NotImplementedException();
        }


    }
}
