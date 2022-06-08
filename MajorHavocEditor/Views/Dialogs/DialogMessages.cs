using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MajorHavocEditor.Views.Dialogs
{
    public partial class DialogMessages : Form
    {
        public DialogMessages()
        {
            this.InitializeComponent();
        }

        public void SetMessages(List<string> messages)
        {
            this.textBoxMessages.Text = String.Join("\r\n", messages.ToArray());
        }
    }
}
