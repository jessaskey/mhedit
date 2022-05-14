using System;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{
    /// <summary>
    /// Separator used in the construction of menus.
    /// </summary>
    public class Separator : MenuItem
    {
        /// <summary>
        /// Inserts as a separator object within a menu relative to the menuItem supplied
        /// and grouped against the provided key.
        /// </summary>
        /// <param name="relativeItem"></param>
        /// <param name="groupKey"></param>
        public Separator(IMenuItem relativeItem, object groupKey)
            : base(Guid.NewGuid().ToString())
        {
            this.ParentName = relativeItem.ParentName;

            this.GroupKey = groupKey;

            // put junk in the display as this is a don't care for a separator since its visual
            // overloaded by an item template.
            this.Display = groupKey;

            // Separator should insert above the relative IMenuItem.
            this.SortOrder = relativeItem.SortOrder - 1;
        }
    }
}
