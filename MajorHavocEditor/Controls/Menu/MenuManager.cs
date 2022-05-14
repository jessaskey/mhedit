using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Controls.Menu
{
    public partial class MenuManager : ToolStrip, IMenuManager
    {
        public MenuManager()
        {
            InitializeComponent();
        }

#region Implementation of IMenuManager

        public object Menu
        {
            get { return this; }
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

            ToolStripButton newItem =
                new ToolStripButton( display )
                {
                    Tag = menuItem,
                    Name = menuItem.Name,
                    Image = image,
                    DisplayStyle = image == null ? ToolStripItemDisplayStyle.Text :
                                       ToolStripItemDisplayStyle.Image
                };

            newItem.Click += ( s, e ) => menuItem.Command.Execute( menuItem.CommandParameter );

            if ( string.IsNullOrWhiteSpace( menuItem.ParentName ) )
            {
                this.Items.Add( newItem );
            }
            else
            {
                ToolStripDropDownButton parent;

                if ( this.Items.ContainsKey( menuItem.ParentName ) )
                {
                    parent = (ToolStripDropDownButton)this.Items[ menuItem.ParentName ];
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
    }

}
