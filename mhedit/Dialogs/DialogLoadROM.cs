using mhedit.Containers;
using mhedit.GameControllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace mhedit
{

    public partial class DialogLoadROM : Form
    {
        private ToolTip tt;
        private MazeCollection _mazeCollection = null;

        public DialogLoadROM()
            : this( string.Empty )
        {
            comboBoxGameDriver.SelectedIndex = 0;
        }

        public DialogLoadROM( string templatePath )
        {
            InitializeComponent();
            comboBoxGameDriver.SelectedIndex = 0;
            textBoxROMPath.Text = templatePath;
        }

        public MazeCollection MazeCollection
        {
            get
            {
                return _mazeCollection;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                IGameController controller = null;

                switch (comboBoxGameDriver.SelectedIndex)
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

                if (controller.LoadTemplate(textBoxROMPath.Text))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Application.DoEvents();

                    List<string> loadMessages = new List<string>();
                    _mazeCollection = controller.LoadMazes(loadMessages);

                    // Change the Created info to show that we loaded the maze From ROMs.
                    foreach (Maze maze in _mazeCollection.Mazes)
                    {
                        maze.Created = new Containers.EditInfo(maze.Created.TimeStamp,
                                       Containers.VersionInformation.ApplicationVersion)
                        {
                            Rom = new RomInfo(controller.Name,
                                               VersionInformation.RomVersion)
                        };
                    }

                    _mazeCollection.AcceptChanges();
                    if (loadMessages.Count > 0)
                    {
                        DialogMessages dm = new DialogMessages();
                        dm.SetMessages(loadMessages);
                        dm.ShowDialog();
                    }
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("There was an issue loading the maze objects: " + controller.LastError, "ROM Load Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    DialogResult = DialogResult.Abort;
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
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            using ( var fbd = new FolderBrowserDialog() )
            {
                fbd.Description = "Take me to your production ROMs";

                fbd.SelectedPath = Path.GetFullPath( textBoxROMPath.Text );

                DialogResult result = fbd.ShowDialog();

                if ( result == DialogResult.OK && !string.IsNullOrWhiteSpace( fbd.SelectedPath ) )
                {
                    textBoxROMPath.Text = fbd.SelectedPath + "\\";
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void Textbox_TextChanged( object sender, EventArgs e )
        {
            buttonOK.Enabled = !string.IsNullOrWhiteSpace( textBoxROMPath.Text );
        }

        private void textBoxTT_MouseHover( object sender, EventArgs e )
        {
            TextBox textBox = (TextBox)sender;

            tt = new ToolTip();
            tt.InitialDelay = 0;

            if ( !string.IsNullOrWhiteSpace( textBox.Text ) )
            {
                tt.Show( textBox.Text, textBox );
            }
        }

        private void textBoxTT_MouseLeave( object sender, EventArgs e )
        {
            tt?.Dispose();

            tt = null;
        }
    }
}
