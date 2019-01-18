using ICSharpCode.SharpZipLib.BZip2;
using mhedit.Containers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogMHPLogin : Form
    {
        private string _savedPasswordKey = "";
        private MazeController _maze = null;

        public DialogMHPLogin()
        {
            InitializeComponent();
        }

        public string Username
        {
            get { return textBoxUsername.Text; }
            set { textBoxUsername.Text = value; }
        }

        public string Password
        {
            get { return textBoxPassword.Text; }
            set { textBoxPassword.Text = value; }
        }

        public string PasswordKey
        {
            get { return _savedPasswordKey; }
            set { _savedPasswordKey = value; }
        }

        public bool SavePassword
        {
            get { return checkBoxSavePassword.Checked; }
            set { checkBoxSavePassword.Checked = value; }
        }
        public Image MazePreview
        {
            set
            {
                pictureBoxMaze.Image = value;
                //pictureBoxMaze.Width = value.Width;
            }
        }

        public MazeController MazeController {
            get
            {
                return _maze;
            }
            set
            {
                _maze = value;
                textBoxDescription.Text = _maze.Description;
                textBoxMazeName.Text = _maze.Name;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //make sure inputs are valid
            if (CredentialsNotEmpty())
            {
                if (MazeNameIsOkay())
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        MazeController.Maze.Description = textBoxDescription.Text;

                        MHEditServiceReference.SecurityToken token = null;
                        if (String.IsNullOrEmpty(_savedPasswordKey))
                        {
                            token = MHPController.Login(textBoxUsername.Text, textBoxPassword.Text);
                        }
                        else
                        {
                            token = new MHEditServiceReference.SecurityToken();
                            token.Username = Username;
                            token.EncryptedPassword = Password;
                            token.EncryptionKey = _savedPasswordKey;
                        }

                        if (token != null)
                        {
                            Properties.Settings.Default.MHPUsername = token.Username; 
                            if (checkBoxSavePassword.Checked)
                            {
                                Properties.Settings.Default.MHPPassword = token.EncryptedPassword;
                                Properties.Settings.Default.MHPKey = token.EncryptionKey;
                            }
                            Properties.Settings.Default.MHPSavePassword = checkBoxSavePassword.Checked;
                            Properties.Settings.Default.Save();

                            try
                            {
                                byte[] mazeBytes = MazeController.SerializeToByteArray(MazeController.Maze);
                                byte[] imageBytes = null;
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    pictureBoxMaze.Image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg); //you could ave in BPM, PNG  etc format.
                                    imageBytes = new byte[memoryStream.Length];
                                    memoryStream.Position = 0;
                                    memoryStream.Read(imageBytes, 0, imageBytes.Length);
                                    memoryStream.Close();
                                }
                                if (mazeBytes != null && imageBytes != null && token != null)
                                {
                                    if (MHPController.UploadMazeDefinition(token, mazeBytes, imageBytes))
                                    {
                                        MessageBox.Show("Maze has been uploaded sucessfully!", "Major Havoc Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        MessageBox.Show("There was an error uploading the maze. Contact Jess.", "Major Havoc Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                Cursor.Current = Cursors.Default;
                                Close();
                            }
                            catch (Exception ex)
                            {
                                Cursor.Current = Cursors.Default;
                                MessageBox.Show("There was a problem uploading the maze info, check your internet connection or try again later.", "Upload Issue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            Cursor.Current = Cursors.Default;
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                            MessageBox.Show("Username or Password is incorrect.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        Cursor.Current = Cursors.Default;
                        MessageBox.Show("There was an error communicating to the website, please check your internet connection.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private bool CredentialsNotEmpty()
        {
            if (String.IsNullOrEmpty(textBoxUsername.Text) || String.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Username and password cannont be blank.", "Empty Username or Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private bool MazeNameIsOkay()
        {
            string badRegex = @"(\s)*level(\s)*(\d)*";
            Regex regex = new Regex(badRegex, RegexOptions.IgnoreCase);
            Match match = regex.Match(textBoxMazeName.Text);
            if (match.Success)
            {
                //not good, they have to give it a better name.
                MessageBox.Show("Your maze name of '" + textBoxMazeName.Text + "' seems to be pretty generic. You have to go edit the maze name to something more descriptive as this will be how we label the maze on the gallery.", "Crappy Maze Name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (String.IsNullOrEmpty(textBoxDescription.Text))
            {
                MessageBox.Show("Maze description is required. Describe the features and play of the maze.", "Missing Description", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private void linkLabelWebLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://mhp.askey.org");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = MHPController.Ping();
            MessageBox.Show(result);

            MHPController.GetMazes();
        }
    }
}
