using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
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
            //Show the ClickOnce assembly version if possible, if this is not a ClickOnce deployment, then show the Assembly version still 
            try
            {
                labelVersion.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() + " (ClickOnce)"; 
            }
            catch
            {
                labelVersion.Text = (Application.ProductVersion + " (Assembly)");
            }
            
            labelCopyright.Text = "Copyright © 2006-" + DateTime.Now.Year.ToString() + "  Jess M. Askey/Bryan L. Roth";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }


    }
}