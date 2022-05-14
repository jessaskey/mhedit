using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajorHavocEditor.Interfaces.Ui
{
    public interface IMenuManager
    {
        /// <summary>
        /// Returns the Menu object for incorporation into the UI.
        /// </summary>
        object Menu { get; }

        /// <summary>
        /// Adds a menu item to the Menu.
        /// </summary>
        /// <param name="menuItem"></param>
        void Add(IMenuItem menuItem);

        /// <summary>
        /// Adds a collection of menu items and their heirarchy to the
        /// Menu.
        /// </summary>
        /// <param name="menuItems"></param>
        void Add(IEnumerable<IMenuItem> menuItems);

        /// <summary>
        /// Attempt to remove a menu item from the menu. As long as the item is at the
        /// bottom of a tree (has no children/isn't a submenu) it will be removed. If the item
        /// removed is the last child of a parent menu item the parent is removed as well.
        /// </summary>
        /// <param name="menuItem"></param>
        void Remove(IMenuItem menuItem);

        /// <summary>
        /// Removes a collection of menu items.
        /// </summary>
        /// <param name="menuItems"></param>
        void Remove(IEnumerable<IMenuItem> menuItems);
    }
}
