using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajorHavocEditor.Controls.Menu.Items
{
    class About : MenuItem
    {
        public About()
            : base(AboutMenuName)
        {
            this.Display = "_About";

            // try to force to left.
            this.SortOrder = int.MaxValue;

            // make private group key.
            //this.GroupKey = new object();
        }
    }
}
