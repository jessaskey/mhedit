using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public static class MenuManagerExtensions
    {
        public static T Create<T>( this IMenuItem menuItem )
            where T : ToolStripItem, new()
        {
            Image image = menuItem.GetImage();

            return new T
                   {
                       Text = menuItem.Display as string ?? menuItem.Name,
                       Name = menuItem.Name,
                       ToolTipText = menuItem.ToolTip as string,
                       Image = image,
                       ImageTransparentColor = Color.Fuchsia,
                       Alignment = menuItem.Options.GetAlignment(),
                       DisplayStyle = image == null ?
                                          ToolStripItemDisplayStyle.Text :
                                          typeof( T ).IsAssignableTo(
                                              typeof( ToolStripMenuItem ) ) ?
                                              ToolStripItemDisplayStyle.ImageAndText :
                                              ToolStripItemDisplayStyle.Image
                   }
                .TrySetShortcutKeys( menuItem.ShortcutKey );
        }

        private static T TrySetShortcutKeys<T>( this T toolStripItem, object shortcutKeys )
            where T : ToolStripItem
        {
            if ( toolStripItem is ToolStripMenuItem tsmi )
            {
                tsmi.ShortcutKeys = (Keys?) shortcutKeys ?? Keys.None;
            }

            return toolStripItem;
        }

        private static Image GetImage( this IMenuItem menuItem )
        {
            return menuItem.Icon switch
            {
                string str => ResourceFactory.GetResourceImage(str),
                Uri uri => ResourceLoader.GetEmbeddedImage(uri),
                null => null,
                _ => throw new NotImplementedException()
            };
        }

        private static ToolStripItemAlignment GetAlignment( this IDictionary<string, object> options )
        {
            return options != null &&
                   options.TryGetValue( nameof( ToolStripItem.Alignment ), out object value ) ?
                       (ToolStripItemAlignment) value :
                       ToolStripItemAlignment.Left;
        }
    }
}
