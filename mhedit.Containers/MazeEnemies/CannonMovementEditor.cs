using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using mhedit.Containers;
using mhedit.Containers.MazeEnemies;
using System.IO;
using Microsoft.VisualBasic;
using System.Runtime.Serialization.Formatters.Binary;

namespace mhedit.Containers
{
    public partial class CannonMovementEditor : Form
    {
        List<iCannonMovement> _movements = new List<iCannonMovement>();

        public List<iCannonMovement> Movements
        {
            get
            {
                return _movements; ;
            }
            set
            {
                _movements = value;
                BindListBox();
            }
        }

        public CannonMovementEditor()
        {
            InitializeComponent();
            LoadPresets();
        }

        public void LoadPresets()
        {
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonMovementsPath = Path.Combine(applicationPath, "cannonMovements");

            toolStripComboBoxLoadPreset.Items.Clear();

            if (Directory.Exists(cannonMovementsPath))
            {
                foreach(string filename in Directory.GetFiles(cannonMovementsPath))
                {
                    string name = Path.GetFileNameWithoutExtension(filename);
                    toolStripComboBoxLoadPreset.Items.Add(name);
                }
            }
        }

        private void listBoxMovements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxMovements.SelectedItem != null)
            {
                propertyGridMovement.SelectedObject = listBoxMovements.SelectedItem;
            }
            else
            {
                propertyGridMovement.SelectedObject = null;
            }
        }

        private void toolStripButtonAddMove_Click(object sender, EventArgs e)
        {
            CannonMovementMove movement = new CannonMovementMove();
            _movements.Add((iCannonMovement)movement);
            BindListBox();
            listBoxMovements.SelectedIndex = _movements.Count - 1;
        }

        private void toolStripButtonAddAngle_Click(object sender, EventArgs e)
        {
            CannonMovementPosition movement = new CannonMovementPosition();
            _movements.Add((iCannonMovement)movement);
            BindListBox();
            listBoxMovements.SelectedIndex = _movements.Count - 1;
        }

        private void toolStripButtonAddPause_Click(object sender, EventArgs e)
        {
            CannonMovementPause movement = new CannonMovementPause();
            _movements.Add((iCannonMovement)movement);
            BindListBox();
            listBoxMovements.SelectedIndex = _movements.Count - 1;
        }

        private void toolStripButtonAddRepeat_Click(object sender, EventArgs e)
        {
            CannonMovementReturn movement = new CannonMovementReturn();
            _movements.Add((iCannonMovement)movement);
            BindListBox();
            listBoxMovements.SelectedIndex = _movements.Count - 1;
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxMovements.SelectedIndex > -1)
            {
                _movements.RemoveAt(listBoxMovements.SelectedIndex);
            }
            BindListBox();
        }

        private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
        {
            if (listBoxMovements.SelectedIndex > 0)
            {
                int index = listBoxMovements.SelectedIndex;
                iCannonMovement o = _movements[index];
                _movements.RemoveAt(index);
                _movements.Insert(index - 1, o);
                BindListBox();
                listBoxMovements.SelectedIndex = index - 1;
            }
        }

        private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
        {
            if (listBoxMovements.SelectedIndex < (listBoxMovements.Items.Count - 2))
            {
                int index = listBoxMovements.SelectedIndex;
                iCannonMovement o = _movements[index];
                _movements.RemoveAt(index);
                _movements.Insert(index + 1, o);
                BindListBox();
                listBoxMovements.SelectedIndex = index + 1;
            }
        }

        private void propertyGridMovement_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            BindListBox();
        }

        private void BindListBox()
        {
            listBoxMovements.Items.Clear();
            foreach (iCannonMovement movement in _movements)
            {
                listBoxMovements.Items.Add(movement);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonPreview_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Preview no workie. Preview in MAME please!");
            //CannonMovementPreview pd = new CannonMovementPreview(_movements);
            //pd.ShowDialog();
        }

        private void toolStripButtonSaveMovements_Click(object sender, EventArgs e)
        {
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonMovementsPath = Path.Combine(applicationPath, "cannonMovements");

            if (!Directory.Exists(cannonMovementsPath))
            {
                Directory.CreateDirectory(cannonMovementsPath);
            }

            string name = Microsoft.VisualBasic.Interaction.InputBox("Give a unique name to the Cannon movement sequence...", "Save Cannon Movement", " ", 0, 0);

            if (!String.IsNullOrEmpty(name))
            {
                string pathFielName = Path.Combine(cannonMovementsPath, name + ".can");
                if (File.Exists(pathFielName))
                {
                    MessageBox.Show("This movement name already exists, you can't overwrite existing cannon movement sequences.", "Name Error");
                }
                else
                {
                    if (MovementsValid(_movements))
                    {
                        using (FileStream fStream = new FileStream(pathFielName, FileMode.Create))
                        {
                            BinaryFormatter b = new BinaryFormatter();
                            b.Serialize(fStream, _movements);
                        }
                        LoadPresets();
                    }
                }
            }
        }

        private bool MovementsValid(List<iCannonMovement> movements)
        {
            bool movementsValid = false;

            if (movements.Count > 0)
            {
                if (movements[movements.Count - 1] is CannonMovementReturn)
                {
                    movementsValid = true;
                }
                else
                {
                    MessageBox.Show("Movement is missing a Return/Loop step at the end. This is required.");
                }
            }
            return movementsValid;
        }

        private void toolStripComboBoxLoadPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            //load a preset
            string applicationPath = Path.GetDirectoryName(Application.ExecutablePath);
            string cannonMovementsPath = Path.Combine(applicationPath, "cannonMovements");

            string movementFile = Path.Combine(cannonMovementsPath, toolStripComboBoxLoadPreset.Text + ".can");
            if (File.Exists(movementFile))
            {
                using (FileStream fStream = new FileStream(movementFile, FileMode.Open))
                {
                    BinaryFormatter b = new BinaryFormatter();
                    _movements= (List<iCannonMovement>)b.Deserialize(fStream);
                    BindListBox();
                }
            }
        }
    }
}
