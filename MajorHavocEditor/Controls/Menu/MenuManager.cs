using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{

    public class MenuStripManager : MenuManager
    {
        public MenuStripManager( DockStyle dockStyle )
            : base( new ToolStrip { Dock = dockStyle } )
        {}
    }

    public class ContextMenuManager : MenuManager
    {
        public ContextMenuManager()
            : base(new ContextMenuStrip())
        { }
    }

    public abstract class MenuManager : IMenuManager
    {
        private readonly ToolStrip _toolStrip;

        protected MenuManager( ToolStrip toolStrip )
        {
            this._toolStrip = toolStrip;
        }

#region Implementation of IMenuManager

        public object Menu
        {
            get { return this._toolStrip; }
        }

        public void Add( IMenuItem menuItem )
        {
            string display = menuItem.Display as string ?? menuItem.Name;

            Image image = menuItem.Icon switch
            {
                string str => ResourceFactory.GetResourceImage(str),
                Uri uri => ResourceLoader.GetEmbeddedImage(uri),
                _ => throw new NotImplementedException()
            };

            ToolStripItem newItem = this._toolStrip switch
            {
                ContextMenuStrip => CreateMenuItem(menuItem, display, image),
                ToolStrip => this.CreateButton( menuItem, display, image ),
                _ => throw new NotImplementedException()
            };


            newItem.Click += ( s, e ) => menuItem.Command.Execute( menuItem.CommandParameter );

            if ( string.IsNullOrWhiteSpace( menuItem.ParentName ) )
            {
                this._toolStrip.Items.Add( newItem );
            }
            else
            {
                ToolStripDropDownButton parent;

                if ( this._toolStrip.Items.ContainsKey( menuItem.ParentName ) )
                {
                    parent = (ToolStripDropDownButton) this._toolStrip.Items[ menuItem.ParentName ];
                }
                else
                {
                    parent = new ToolStripDropDownButton( menuItem.ParentName )
                             {
                                 DisplayStyle = ToolStripItemDisplayStyle.Image,
                                 Text = menuItem.ParentName
                             };
                }

                parent.DropDownItems.Add( newItem );
            }
        }

        public void Add( IEnumerable<IMenuItem> menuItems )
        {
            foreach ( IMenuItem menuItem in menuItems )
            {
                this.Add( menuItem );
            }
        }

        public void Remove( IMenuItem menuItem )
        {
            throw new NotImplementedException();
        }

        public void Remove( IEnumerable<IMenuItem> menuItems )
        {
            throw new NotImplementedException();
        }

#endregion

        private ToolStripItem CreateMenuItem( IMenuItem menuItem, string display, Image image )
        {
            return new ToolStripMenuItem(display)
                   {
                       Tag = menuItem,
                       ToolTipText = menuItem.ToolTipText,
                       Name = menuItem.Name,
                       Image = image,
                       DisplayStyle = image == null ? ToolStripItemDisplayStyle.Text :
                                          ToolStripItemDisplayStyle.ImageAndText
            };
        }

        private ToolStripItem CreateButton( IMenuItem menuItem, string display, Image image )
        {
            return new ToolStripButton( display )
                   {
                       Tag = menuItem,
                       Name = menuItem.Name,
                       Image = image,
                       DisplayStyle = image == null ? ToolStripItemDisplayStyle.Text :
                                          ToolStripItemDisplayStyle.Image
                   };
        }
    }

}
