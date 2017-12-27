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
        private IProjectTemplate SelectedTemplate
        {
            get
            {
                if(lstProjTemplate.SelectedIndices.Count > 0)
                {
                    return (IProjectTemplate)lstProjTemplate.Items[lstProjTemplate.SelectedIndices[0]].Tag;
                }
                else
                {
                    return null;
                }
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
                if (txtWorkspace.Enabled && Manager.TargetWorkspace == null)
                {
                    Manager.CreateNewWorkspace(Path.Combine(WorkspacePath, txtWorkspace.Text + ".wrkspc"));
                }

                if (SelectedTemplate != null)
                {
                    Manager.AddProject(SelectedTemplate.CreateProject(Path.Combine(WorkspacePath, txtName.Text, txtName.Text + ".fitproj"), txtName.Text, Manager));
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
            imageList1.Images.Clear();

            // init default project templates
            Font f = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular);
            for (int i = 0; i < GLOBALS.ProjectTemplates.Length; i++)
            {
                IProjectTemplate template = GLOBALS.ProjectTemplates[i];

                if (template.TemplateIcon != null)
                {
                    imageList1.Images.Add(template.TemplateIcon);
                }

                ListViewItem lvi = new ListViewItem(template.DisplayText);
                lvi.Tag = template;
                lvi.Font = f;

                lstProjTemplate.Items.Add(lvi);
            }
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
            using (var dlg = new FolderSelectDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtLocation.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
