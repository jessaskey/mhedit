using mhedit.Containers;
using mhedit.Controllers;
using mhedit.GameControllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogLoadROM : Form
    {
        private MazeCollection _mazeCollection = null;

        public DialogLoadROM()
            : this( string.Empty )
        {}

        public DialogLoadROM( string templatePath )
        {
            InitializeComponent();

            textBoxROMPath.Text = templatePath;
        }

        public MazeCollection Mazes
        {
            get
            {
                return _mazeCollection;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
#if DEBUG
#else
            try
#endif
            {
                //Load ROM's here
                IGameController controller = new MajorHavocPromisedEnd( textBoxROMPath.Text );

                List<string> loadMessages = new List<string>();
                _mazeCollection = controller.LoadMazes( textBoxROMPath.Text, loadMessages );

                if ( loadMessages.Count > 0 )
                {
                    DialogMessages dm = new DialogMessages();
                    dm.SetMessages( loadMessages );
                    dm.ShowDialog();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
#if DEBUG
#else
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
#endif
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
    }
}
