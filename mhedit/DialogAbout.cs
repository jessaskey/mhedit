using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogAbout : Form
    {
        /// <summary>
        /// Our default constructor for the About dialog.
        /// </summary>
        public DialogAbout()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DialogAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Application.ProductVersion;
            labelCopyright.Text = "Copyright © 2006-" + DateTime.Now.Year.ToString() + "  Jess M. Askey";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


    }
}