﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mhedit.MazeEnemies
{
    public partial class CannonMovementPreview : Form
    {
        private List<iCannonMovement> _movements;
        private int _currentIndex = 0;
        private Point _currentPosition = new Point(32, 32);
        private Velocity _currentVelocity = new Velocity();

        public CannonMovementPreview(List<iCannonMovement> movements)
        {
            InitializeComponent();
            _movements = movements;
            timerMain.Enabled = true;
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            _currentIndex++;
            
        }
    }
}
