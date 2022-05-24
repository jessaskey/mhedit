using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajorHavocEditor.Controls.Menu.Items
{
    internal class Paste : MenuItem
    {
        public Paste()
            : base("Paste")
        {
            this.Display = "_Paste";

            this.ParentName = EditMenuName;

            // try to force to left.
            this.SortOrder = int.MinValue + 1;

            // make private group key.
            this.GroupKey = Cut.CutCopyPasteGroupKey;
        }
    }
}
