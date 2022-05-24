using System;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public static class MenuManagerExtensions
    {
        public static ToolStripItem Create<T>( this IMenuItem menuItem )
            where T : ToolStripItem, new()
        {
            Image image = menuItem.GetImage();

            return new T
                   {
                       Tag = menuItem,
                       Text = menuItem.Display as string ?? menuItem.Name,
                       Name = menuItem.Name,
                       ToolTipText = menuItem.ToolTip as string,
                       Image = image,
                       ImageTransparentColor = Color.Fuchsia,
                       DisplayStyle = image == null ?
                                          ToolStripItemDisplayStyle.Text :
                                          typeof(T) == typeof(ToolStripMenuItem) ?
                                              ToolStripItemDisplayStyle.ImageAndText :
                                              ToolStripItemDisplayStyle.Image
                   };
        }

        public static Image GetImage( this IMenuItem menuItem )
        {
            return menuItem.Icon switch
            {
                string str => ResourceFactory.GetResourceImage(str),
                Uri uri => ResourceLoader.GetEmbeddedImage(uri),
                null => null,
                _ => throw new NotImplementedException()
            };
        }

    }

}