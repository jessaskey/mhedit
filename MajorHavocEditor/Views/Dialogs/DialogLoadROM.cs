using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using mhedit.Containers;
using mhedit.GameControllers;
using VersionInformation = mhedit.VersionInformation;

namespace MajorHavocEditor.Views.Dialogs
{

    public partial class DialogLoadROM : Form
    {
        private ToolTip tt;
        private MazeCollection _mazeCollection = null;

        public DialogLoadROM()
            : this( string.Empty )
        { }

        public DialogLoadROM( string templatePath )
        {
            this.InitializeComponent();

            this.comboBoxGameDriver.SelectedIndex = 0;

            this.textBoxROMPath.Text = string.IsNullOrWhiteSpace( templatePath ) ?
                                           Properties.Settings.Default.TemplatesLocation :
                                           templatePath;
        }

        public MazeCollection MazeCollection
        {
            get
            {
                return this._mazeCollection;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            /// The Ok button was selected... but lets assume that it wasn't until
            /// we successfully load the ROMs.
            this.DialogResult = DialogResult.None;

            try
            {
                IGameController controller = null;

                switch (this.comboBoxGameDriver.SelectedIndex)
                {
                    case 0:
                        //Major Havoc - The Promised End
                        controller = new MajorHavocPromisedEnd();
                        break;
                    case 1:
                        //Major Havoc v3
                        controller = new MajorHavoc(false);
                        break;
                    case 2:
                        //Return to Vaxx
                        controller = new MajorHavoc(true);
                        break;
                }

                if (controller == null)
                {
                    MessageBox.Show("The selected Game Driver is unknown.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (controller.LoadTemplate(this.textBoxROMPath.Text))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Application.DoEvents();

                    List<string> loadMessages = new List<string>();
                    this._mazeCollection = controller.LoadMazes(loadMessages);

                    // Change the Created info to show that we loaded the maze From ROMs.
                    foreach (Maze maze in this._mazeCollection.Mazes)
                    {
                        maze.Created = new mhedit.Containers.EditInfo(maze.Created.TimeStamp,
                                       mhedit.Containers.VersionInformation.ApplicationVersion)
                        {
                            Rom = new RomInfo(controller.Name,
                                               VersionInformation.RomVersion)
                        };
                    }

                    this._mazeCollection.AcceptChanges();
                    if (loadMessages.Count > 0)
                    {
                        //DialogMessages dm = new DialogMessages();
                        //dm.SetMessages(loadMessages);
                        //dm.ShowDialog();
                    }
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("There was an issue loading the maze objects: " + controller.LastError, "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.DialogResult = DialogResult.Abort;
                }
            }
//#if DEBUG
//#else
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
//#endif
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            using ( var fbd = new FolderBrowserDialog() )
            {
                fbd.Description = "Take me to your production ROMs";

                fbd.SelectedPath = Path.GetFullPath( this.textBoxROMPath.Text );

                DialogResult result = fbd.ShowDialog();

                if ( result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
                {
                    this.textBoxROMPath.Text = fbd.SelectedPath + "\\";
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void Textbox_TextChanged( object sender, EventArgs e )
        {
            this.buttonOK.Enabled = !string.IsNullOrWhiteSpace( this.textBoxROMPath.Text );
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
