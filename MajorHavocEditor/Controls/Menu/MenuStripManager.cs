using System.Windows.Forms;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public class MenuStripManager : MenuManager
    {
        public MenuStripManager( DockStyle dockStyle )
            : base( new ToolStrip { Dock = dockStyle } )
        { }

#region Overrides of AltMenuManager

        /// <inheritdoc />
        protected override ToolStripItem CreateMenuItem( IMenuItem menuItem )
        {
            if (menuItem is Separator separator)
            {
                return new ToolStripSeparator();
            }

            if ( menuItem.Command == null )
            {
                return menuItem.Create<ToolStripDropDownButton>();
            }

            if ( string.IsNullOrWhiteSpace( menuItem.ParentName ) )
            {
                return menuItem.Create<ToolStripButton>();
            }

            return menuItem.Create<ToolStripMenuItem>();
        }


        ///// <inheritdoc />
        //protected override void MoveChildren( ToolStripItem source, ToolStripItem destination )
        //{
        //    if (source is ToolStripDropDownItem hasChildren)
        //    {
        //        ToolStripDropDownItem receivesChildren = (ToolStripDropDownItem)destination;

        //        ToolStripItem[] children = new ToolStripItem[hasChildren.DropDownItems.Count];

        //        hasChildren.DropDownItems.CopyTo(children, 0);

        //        hasChildren.DropDownItems.Clear();

        //        receivesChildren.DropDownItems.AddRange(children);
        //    }

        //}

#endregion
    }

}