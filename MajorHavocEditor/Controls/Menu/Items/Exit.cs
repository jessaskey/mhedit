﻿using System;

namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class Exit : MenuItem
    {
        public Exit()
            : base("Exit")
        {
            this.Display = "E_xit";

            this.ParentName = FileMenuName;

            this.SortOrder = int.MaxValue;

            this.Command = new MenuCommand( notUsed => {} );

            // make private group key.
            this.GroupKey = new object();
        }

        public event EventHandler ExitApplication;
    }
}