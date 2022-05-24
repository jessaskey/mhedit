using System.Drawing;
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

#region Overrides of AltMenuManager

        /// <inheritdoc />
        protected override ToolStripItem CreateMenuItem( IMenuItem menuItem )
        {
            Image image = menuItem.GetImage();

            if (menuItem is Separator separator)
            {
                return new ToolStripSeparator();
            }

            return menuItem.Create<ToolStripMenuItem>();

            //return new ToolStripMenuItem()
            //       {
            //           Tag = menuItem,
            //           Text = menuItem.Display as string ?? menuItem.Name,
            //           Name = menuItem.Name,
            //           ToolTipText = menuItem.ToolTip as string,
            //           Image = image,
            //           ImageTransparentColor = Color.Fuchsia,
            //           DisplayStyle = image == null ? ToolStripItemDisplayStyle.Text :
            //                              ToolStripItemDisplayStyle.ImageAndText
            //       };
        }

#endregion
    }

}