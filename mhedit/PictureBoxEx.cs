using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mhedit
{
    class PictureBoxEx : PictureBox
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Down:
                        OnKeyDown(this, new KeyEventArgs(Keys.Down));
                        break;

                    case Keys.Up:
                        OnKeyDown(this, new KeyEventArgs(Keys.Up));
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
	
    }
}
