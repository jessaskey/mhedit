using System;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace MajorHavocEditor.Views.Dialogs
{
    public partial class DialogExport : KryptonForm
    {
        public DialogExport()
        {
            this.InitializeComponent();
        }

        public string ExportDirectory { 
            get { return this.textBoxExportPath.Text; }
            set { this.textBoxExportPath.Text = value; }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = this.textBoxExportPath.Text;
            DialogResult dr = fd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                this.textBoxExportPath.Text = fd.SelectedPath;
            }
        }

    }
}
