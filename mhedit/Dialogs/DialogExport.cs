using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogExport : Form
    {
        public DialogExport()
        {
            InitializeComponent();
        }

        public string ExportDirectory { 
            get { return textBoxExportPath.Text; }
            set { textBoxExportPath.Text = value; }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = textBoxExportPath.Text;
            DialogResult dr = fd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                textBoxExportPath.Text = fd.SelectedPath;
            }
        }

    }
}
