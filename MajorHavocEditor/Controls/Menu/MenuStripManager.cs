using System.Windows.Forms;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public class MenuStripManager : MenuManager
    {
        public MenuStripManager( DockStyle dockStyle )
            : base( new ToolStrip { Dock = dockStyle } )
        { }

#region Overrides of MenuManager

        /// <inheritdoc />
        protected override ToolStripItem CreateMenuItem( IMenuItem menuItem )
        {
            if (menuItem is Separator)
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

#endregion
    }

}