using mhedit.Containers.MazeEnemies.IonCannon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    public partial class CannonProgramPreview : Form
    {
        private IonCannonProgram _program;
        private int _currentIndex = 0;
        private Point _currentPosition = new Point(32, 32);
        private Velocity _currentVelocity = new Velocity();

        public CannonProgramPreview( IonCannonProgram movements )
        {
            InitializeComponent();
            _program = movements;
            timerMain.Enabled = true;
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            _currentIndex++;
            toolStripLabelSequenceCounter.Text = "(" + _currentIndex.ToString() + "/" + _program.Count.ToString() + ")";
        
            if (_currentIndex > _program.Count)
            {
                _currentIndex = 0;

                if (!toolStripButtonRepeat.Checked)
                {
                    timerMain.Enabled = false;
                }
            }
      
        }
    }
}
