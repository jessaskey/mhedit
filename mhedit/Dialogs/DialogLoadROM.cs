﻿using mhedit.Containers;
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
        private const string DefaultRomRootPath = @"C:\SVN\havoc\mame\roms\";
        private MazeCollection _mazeCollection = null;

        //public DialogLoadROM( string relativeTemplatePath )
        //{
        //    InitializeComponent();

        //    textBoxROMPath.Text = Directory.Exists( relativeTemplatePath ) ?
        //        relativeTemplatePath :
        //        string.Empty;

        //    //set defaults
        //    FindDefaultPaths();
        //}


        public DialogLoadROM()
        {
            InitializeComponent();

            //set defaults
            FindDefaultPaths();
        }

        public MazeCollection Mazes
        {
            get
            {
                return _mazeCollection;
            }
        }

        private void FindDefaultPaths()
        {
            /// Only auto-populate the path if it's not been previously set
            /// or isn't rooted in the default.
            if ( string.IsNullOrWhiteSpace( textBoxROMPath.Text ) ||
                 textBoxROMPath.Text.StartsWith( DefaultRomRootPath ) )
            {
                string romPath = DefaultRomRootPath +
                    ( radioButtonMH.Checked ? @"mhavoc\" : @"mhavocpe\" );

                if ( Directory.Exists( romPath ) )
                {
                    textBoxROMPath.Text = romPath;
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string templateFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "template");
            //Load ROM's here
            try
            {
                IGameController controller = radioButtonMH.Checked ?
                    (IGameController)new MajorHavoc(templateFolder ) :
                     new MajorHavocPromisedEnd(templateFolder );

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
            catch ( Exception ex )
            {
                MessageBox.Show( ex.Message, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "Take me to your production ROMs";
            DialogResult dr = fb.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                textBoxROMPath.Text = fb.SelectedPath + "\\";
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            FindDefaultPaths();
        }

        private void Textbox_TextChanged( object sender, EventArgs e )
        {
            buttonOK.Enabled = !string.IsNullOrWhiteSpace( textBoxROMPath.Text );
        }
    }
}
