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
        private ToolTip tt;
        private MazeCollection _mazeCollection = null;

        public DialogLoadROM()
            : this( string.Empty )
        {}

        public DialogLoadROM( string templatePath )
        {
            InitializeComponent();

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
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();

                List<string> loadMessages = new List<string>();
                IGameController controller = new MajorHavocPromisedEnd(textBoxROMPath.Text);
                _mazeCollection = controller.LoadMazes(textBoxROMPath.Text, loadMessages);
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
#if DEBUG
#else
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
#endif
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
