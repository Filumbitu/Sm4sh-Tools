using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sm4shCommand.GUI
{
    public partial class NewProjectDialog : Form
    {
        public NewProjectDialog()
        {
            InitializeComponent();

        }
        private WorkspaceManager Manager { get; set; }
        private string WorkspacePath
        {
            get
            {
                if (txtWorkspace.Enabled)
                    return Path.Combine(txtLocation.Text.TrimEnd(Path.DirectorySeparatorChar), txtWorkspace.Text);
                else
                    return Manager.TargetWorkspace.WorkspaceRoot;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            Manager = MainForm.Instance.WorkspaceManager;
            txtName.Text = txtWorkspace.Text = "NewProject";

            if (Manager.TargetWorkspace != null)
            {
                txtWorkspace.Enabled = false;
                txtLocation.Text = Manager.TargetWorkspace.WorkspaceRoot;
            }
            else
            {
                if (!Directory.Exists(GLOBALS.DefaultProjectDirectory))
                {
                    Directory.CreateDirectory(GLOBALS.DefaultProjectDirectory);
                }
                txtLocation.Text = GLOBALS.DefaultProjectDirectory;
            }


            InitializeProjectTemplates();

            // Set default project template
            lstProjTemplate.Items[0].Selected = true;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(WorkspacePath, txtName.Text)))
            {
                Directory.CreateDirectory(Path.Combine(WorkspacePath, txtName.Text));
                switch (lstProjTemplate.SelectedIndices[0])
                {
                    case 0:
                        CreateEmptyProject(Path.Combine(WorkspacePath, txtName.Text, txtName.Text + ".fitproj"));
                        break;
                    case 1:
                        // DecompileNewProject();
                        break;
                    case 2:
                        // ProjectFromExistingFiles();
                        break;
                }
            }
            else
            {
                MessageBox.Show("An existing project with this name already exists at this location!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void InitializeProjectTemplates()
        {
            // init default project templates
            Font f = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular);
            ListViewItem lvi = new ListViewItem("New Empty Project");
            lvi.Font = f;
            lvi.ImageIndex = 1;
            lstProjTemplate.Items.Add(lvi);

            lvi = new ListViewItem("Decompile New Project");
            lvi.Font = f;
            lvi.ImageIndex = 2;
            lstProjTemplate.Items.Add(lvi);

            lvi = new ListViewItem("Project From Existing Files");
            lvi.Font = f;
            lvi.ImageIndex = 0;
            lstProjTemplate.Items.Add(lvi);
        }

        private void CreateEmptyProject(string path)
        {
            if (txtWorkspace.Enabled && Manager.TargetWorkspace == null)
            {
                Manager.CreateNewWorkspace(Path.Combine(WorkspacePath, txtWorkspace.Text + ".wrkspc"));
            }

            var p = new FitProj
            {
                ProjFilepath = path,
                ProjName = txtName.Text,
                Platform = ProjPlatform.WiiU,
                ToolVer = Program.Version,
                GameVer = "1.1.7",
                ProjectGuid = Guid.NewGuid()
            };

            Manager.AddProject(p);
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(Path.Combine($"{txtLocation.Text}", txtWorkspace.Text, txtName.Text)))
            {
                int i = 1;
                while (Directory.Exists(Path.Combine($"{txtLocation.Text}", txtWorkspace.Text, txtName.Text + i)))
                {
                    i++;
                }
                txtName.Text = txtName.Text + i;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using(var dlg = new FolderSelectDialog())
            {
                if(dlg.ShowDialog() == DialogResult.OK)
                {
                    txtLocation.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
