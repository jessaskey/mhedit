using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mhedit
{
    public partial class DialogMessages : Form
    {
        public DialogMessages()
        {
            InitializeComponent();
        }

        public void SetMessages(List<string> messages)
        {
            textBoxMessages.Text = String.Join("\r\n", messages.ToArray());
        }
    }
}
