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
        private string _templatePath = null;
        public DialogLoadROM()
        {
            InitializeComponent();

            //set defaults
            FindDefaultPaths();
        }

        public string ROMPath
        {
            get
            {
                return textBoxROMPath.Text;
            }
            set
            {
                textBoxROMPath.Text = value;
            }
        }

        public string TemplatePath
        {
            get
            {
                return _templatePath;
            }
            set
            {
                _templatePath = value;
            }
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
            string romPath = "";
            if (radioButtonMH.Checked)
            {
                romPath = @"C:\SVN\havoc\mame\roms\mhavoc\";
            }
            if (radioButtonMHPE.Checked)
            {
                romPath = @"C:\SVN\havoc\mame\roms\mhavocpe\";
            }
            if (Directory.Exists(romPath))
            {
                textBoxROMPath.Text = romPath;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //Load ROM's here
            //try
            //{
                IGameController controller = null;

                if (radioButtonMH.Checked)
                {
                    controller = new MajorHavoc(_templatePath);
                }
                if (radioButtonMHPE.Checked)
                {
                    controller = new MajorHavocPromisedEnd(_templatePath);
                }

                List<string> loadMessages = new List<string>();
                _mazeCollection = controller.LoadMazes(textBoxROMPath.Text, loadMessages);

            if (loadMessages.Count > 0)
            {
                DialogMessages dm = new DialogMessages();
                dm.SetMessages(loadMessages);
                dm.ShowDialog();
            }

                DialogResult = DialogResult.OK;
                Close();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "ROM Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //}

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
    }
}
