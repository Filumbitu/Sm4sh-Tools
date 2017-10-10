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

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NewProjectDialog_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(GLOBALS.DefaultProjectDirectory))
            {
                Directory.CreateDirectory(GLOBALS.DefaultProjectDirectory);
            }
            txtLocation.Text = GLOBALS.DefaultProjectDirectory;

            int i = 1;
            while (Directory.Exists($"NewProject{i}"))
            {
                i++;
            }
            txtName.Text = $"NewProject{i}";

            InitializeProjectTemplates();

            // Set default project template
            lstProjTemplate.Items[0].Selected = true;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (!chkCurWorkspace.Checked)
            {
                txtWorkspace.Text = ((TextBox)sender).Text;
            }
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Path.Combine(txtLocation.Text, txtName.Text)))
            {
                var path = Path.Combine(txtLocation.Text, txtName.Text);
                Directory.CreateDirectory(path);
                switch (lstProjTemplate.SelectedIndices[0])
                {
                    case 0:
                        CreateEmptyProject(Path.Combine(path, txtName.Text + ".fitproj"));
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
            WorkspaceManager manager = MainForm.Instance.WorkspaceManager;
            // manager.NewWorkspace(txtWorkspace.Text);

            var p = new FitProj();
            p.ProjName = txtName.Text;
            p.Platform = ProjPlatform.WiiU;
            p.ToolVer = Program.Version;
            p.GameVer = "1.1.7";

            p.SaveProject(path);
        }
    }
}
