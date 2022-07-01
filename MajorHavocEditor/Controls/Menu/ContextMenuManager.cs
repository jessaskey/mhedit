using System.Windows.Forms;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public class ContextMenuManager : MenuManager
    {
        public ContextMenuManager()
            //: base(new KryptonContextMenu())  // Requires movement to Krypton MenuItems
            : base(new ContextMenuStrip())
        { }

#region Overrides of MenuManager

        /// <inheritdoc />
        protected override ToolStripItem CreateMenuItem( IMenuItem menuItem )
        {
            if (menuItem is Separator)
            {
                return new ToolStripSeparator();
            }

            return menuItem.Create<ToolStripMenuItem>();
        }

#endregion
    }

}