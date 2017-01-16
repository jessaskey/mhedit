using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mhedit.MazeEnemies;

namespace mhedit
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
            CannonMovementAngle movement = new CannonMovementAngle();
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
    }
}
