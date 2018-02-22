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
    public partial class DialogMHPLogin : Form
    {
        public DialogMHPLogin()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                Properties.Settings.Default.MHPUsername = textBoxUsername.Text;
                Properties.Settings.Default.MHPPassword = textBoxPassword.Text;
                Properties.Settings.Default.Save();
                Close();
            }
            else
            {
                MessageBox.Show("Username or Password is incorrect.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool Validate()
        {
            return MHPController.Login(textBoxUsername.Text, textBoxPassword.Text);
        }
    }
}
