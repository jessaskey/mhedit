using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MajorHavocEditor.Interfaces.Ui
{
    public interface IMenuItem
    {
        /// <summary>
        /// Used to uniquely identify this menuItem and provides a way for other
        /// menu items to identify it as a parent.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Provides an association to a parent, Effectively making this menu item
        /// a submenu of the parent. A null or empty string indicates a top level
        /// menu.
        /// </summary>
        string ParentName { get; }

        /// <summary>
        /// Object to be displayed in the menu. Typically this is a string
        /// but in WPF it can be any visual element.
        /// </summary>
        object Display { get; }

        /// <summary>
        /// Object that represents the Icon that is displayed with the menuItem.
        /// Typically these would be Uri or string objects that point to resources.
        /// <see cref="http://msdn.microsoft.com/en-us/library/aa970069.aspx"/>
        /// </summary>
        object Icon { get; }

        /// <summary>
        /// Sets the text describing an input gesture that will call the command
        /// tied to the specified item.
        /// </summary>
        //string InputGestureText { get; }

        /// <summary>
        /// True if the menuItem should be checked, false otherwise.
        /// </summary>
        bool IsChecked { get; }

        /// <summary>
        /// Used to group multiple MenuItems together within the menu structure. Typically
        /// this is the UI that created this item.
        /// </summary>
        object GroupKey { get; }

        /// <summary>
        /// Provides a sort order for items within a group.
        /// </summary>
        int SortOrder { get; }

        // Mimic the ICommandSource interface of WPF here because
        // its very useful. Can't use it directly because it wouldn't
        // support Windows Forms.
        #region Mimic ICommandSource

        /// <summary>
        /// The command that will be executed when the menu item is selected.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// The parameter passed to the ICommand.Exceute method when the command
        /// is executed.
        /// </summary>
        object CommandParameter { get; }

        /// <summary>
        /// The target object of the command. This has special meaning for WPF UI elements.
        /// Typically this isn't used for WinForms.
        /// </summary>
        //object CommandTarget { get; }

        #endregion
    }
}
