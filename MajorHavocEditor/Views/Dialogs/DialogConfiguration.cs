using System;
using System.IO;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace MajorHavocEditor.Views.Dialogs
{
    public partial class DialogConfiguration : KryptonForm
    {
        private ToolTip tt;

        public DialogConfiguration()
        {
            this.InitializeComponent();
            this.textBoxMameDriver.Text = Properties.Settings.Default.MameDriver;
            this.textBoxMameExecutable.Text = Properties.Settings.Default.MameExecutable;
            //textBoxUsername.Text = Properties.Settings.Default.MHPUsername;
            //textBoxPassword.Text = Properties.Settings.Default.MHPPassword;
            this.checkBoxDebug.Checked = Properties.Settings.Default.MameDebug;
            this.checkBoxShowGridCoordinateReferences.Checked = Properties.Settings.Default.ShowGridReferences;
            this.checkBoxMAMEWindow.Checked = Properties.Settings.Default.MameWindow;
            this.textBoxMameCommandLineOptions.Text = Properties.Settings.Default.MameCommandLineOptions;
            this.checkBoxCompressOnSave.Checked = Properties.Settings.Default.CompressOnSave;

            //locations
            this.textBoxTemplatesLocation.Text = Properties.Settings.Default.TemplatesLocation;
        }

        //private void linkLabelMHP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    System.Diagnostics.Process.Start(Globals.MHPHomepage);
        //}

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //save values back
            Properties.Settings.Default.MameDriver = this.textBoxMameDriver.Text;
            Properties.Settings.Default.MameExecutable = this.textBoxMameExecutable.Text;
            //Properties.Settings.Default.MHPUsername = textBoxUsername.Text;
            //Properties.Settings.Default.MHPPassword = textBoxPassword.Text;
            Properties.Settings.Default.MameDebug = this.checkBoxDebug.Checked;
            Properties.Settings.Default.ShowGridReferences = this.checkBoxShowGridCoordinateReferences.Checked;
            Properties.Settings.Default.MameWindow = this.checkBoxMAMEWindow.Checked;
            Properties.Settings.Default.MameCommandLineOptions = this.textBoxMameCommandLineOptions.Text;
            Properties.Settings.Default.CompressOnSave = this.checkBoxCompressOnSave.Checked;

            //locations
            Properties.Settings.Default.TemplatesLocation = this.textBoxTemplatesLocation.Text;

            Properties.Settings.Default.Save();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonBrowseMameExecutable_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select the path to the HBMAME executable",
                InitialDirectory = Path.GetDirectoryName( this.textBoxMameExecutable.Text ),
                FileName = Path.GetFileNameWithoutExtension( this.textBoxMameExecutable.Text ),
                CheckFileExists = true,
                Filter = "HBMame Executable (*.exe)|*.exe"
            };

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                this.textBoxMameExecutable.Text = ofd.FileName;
            }
        }

        private void buttonBrowseTemplatesFolder_Click( object sender, EventArgs e )
        {
            using ( var fbd = new FolderBrowserDialog() )
            {
                fbd.SelectedPath = Path.GetFullPath( this.textBoxTemplatesLocation.Text );

                DialogResult result = fbd.ShowDialog();

                if ( result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
                {
                    this.textBoxTemplatesLocation.Text = fbd.SelectedPath;
                }
            }
        }

        private void textBoxTT_MouseHover( object sender, EventArgs e )
        {
            TextBox textBox = (TextBox)sender;

            this.tt = new ToolTip();
            this.tt.InitialDelay = 0;

            if ( !string.IsNullOrWhiteSpace( textBox.Text ) )
            {
                this.tt.Show( textBox.Text, textBox );
            }
        }

        private void textBoxTT_MouseLeave( object sender, EventArgs e )
        {
            this.tt?.Dispose();

            this.tt = null;
        }
    }
}
