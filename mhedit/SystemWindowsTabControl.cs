using System;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers.Validation;

namespace mhedit
{
    /// <summary>
    /// Derived from:
    /// https://stackoverflow.com/a/36900582
    ///
    /// Removed the Add tab as "windows" are added via events in the system.
    /// </summary>
    public class SystemWindowsTabControl : TabControl, ISystemWindows
    {
        public SystemWindowsTabControl()
        {
            this.Padding = new Point( 12, 4 );
            this.DrawMode = TabDrawMode.OwnerDrawFixed;

            this.DrawItem += this.OnDrawItem;
            this.MouseDown += this.OnMouseDown;
            this.ControlAdded += this.OnControlAdded;
        }

#region Implementation of ISystemWindows

        public void Add( UserControl view )
        {
            TabPage viewTab = new TabPage( view.Text )
                              {
                                  Dock = DockStyle.Fill,
                                  UseVisualStyleBackColor = true
                              };

            view.Dock = DockStyle.Fill;

            viewTab.Controls.Add( view );

            this.TabPages.Add( viewTab );

            this.SelectedTab = viewTab;
        }

        #endregion

        private void OnControlAdded( object sender, ControlEventArgs e )
        {
            this.Visible = this.TabPages.Count > 0;
        }

        private void OnMouseDown( object sender, MouseEventArgs e )
        {
            for ( var i = 0; i < this.TabPages.Count; i++ )
            {
                var tabRect = this.GetTabRect( i );
                tabRect.Inflate( -2, -2 );
                var closeImage = Properties.Resources.Close;

                var imageRect = new Rectangle(
                    ( tabRect.Right - closeImage.Width ),
                    tabRect.Top + ( tabRect.Height - closeImage.Height ) / 2,
                    closeImage.Width,
                    closeImage.Height );

                if ( imageRect.Contains( e.Location ) )
                {
                    this.TabPages.RemoveAt( i );

                    this.Visible = this.TabPages.Count != 0;

                    break;
                }
            }
        }
         
        private void OnDrawItem( object sender, DrawItemEventArgs e )
        {
            var tabPage = this.TabPages[ e.Index ];
            var tabRect = this.GetTabRect( e.Index );
            tabRect.Inflate( -2, -2 );

            var closeImage = Properties.Resources.Close;

            e.Graphics.DrawImage( closeImage,
                ( tabRect.Right - closeImage.Width ),
                tabRect.Top + ( tabRect.Height - closeImage.Height ) / 2 );

            TextRenderer.DrawText( e.Graphics, tabPage.Text, tabPage.Font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left );
        }
    }
}
