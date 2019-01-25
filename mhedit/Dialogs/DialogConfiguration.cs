﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogConfiguration : Form
    {
        public DialogConfiguration()
        {
            InitializeComponent();
            textBoxMameDriver.Text = Properties.Settings.Default.MameDriver;
            textBoxMameExecutable.Text = Properties.Settings.Default.MameExecutable;
            //textBoxUsername.Text = Properties.Settings.Default.MHPUsername;
            //textBoxPassword.Text = Properties.Settings.Default.MHPPassword;
            checkBoxDebug.Checked = Properties.Settings.Default.MameDebug;
            checkBoxShowGridCoordinateReferences.Checked = Properties.Settings.Default.ShowGridReferences;
            checkBoxMAMEWindow.Checked = Properties.Settings.Default.MameWindow;

            //locations
            textBoxTemplatesLocation.Text = Properties.Settings.Default.TemplatesLocation;
        }

        private void linkLabelMHP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Globals.MHPHomepage);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //save values back
            Properties.Settings.Default.MameDriver = textBoxMameDriver.Text;
            Properties.Settings.Default.MameExecutable = textBoxMameExecutable.Text;
            //Properties.Settings.Default.MHPUsername = textBoxUsername.Text;
            //Properties.Settings.Default.MHPPassword = textBoxPassword.Text;
            Properties.Settings.Default.MameDebug = checkBoxDebug.Checked;
            Properties.Settings.Default.ShowGridReferences = checkBoxShowGridCoordinateReferences.Checked;
            Properties.Settings.Default.MameWindow = checkBoxMAMEWindow.Checked;

            //locations
            Properties.Settings.Default.TemplatesLocation = textBoxTemplatesLocation.Text;

            Properties.Settings.Default.Save();
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonBrowseMameExecutable_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                textBoxMameExecutable.Text = openFileDialog1.FileName;
            }
        }

        private void buttonBrowseTemplatesFolder_Click( object sender, EventArgs e )
        {
            using ( var fbd = new FolderBrowserDialog() )
            {
                fbd.SelectedPath = Path.GetFullPath( textBoxTemplatesLocation.Text );

                DialogResult result = fbd.ShowDialog();

                if ( result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
                {
                    textBoxTemplatesLocation.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
