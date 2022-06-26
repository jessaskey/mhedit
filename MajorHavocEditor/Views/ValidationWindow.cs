using System.Drawing;
using System.Windows.Forms;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{

    public class ValidationWindow : TabControl, IUserInterface
    {
        private readonly Image _closeImage;

        public ValidationWindow()
        {
            this._closeImage = ResourceLoader.GetEmbeddedImage(
                @"Resources\Images\Menu\Close.png".CreateResourceUri() );

            this.Padding = new Point( 12, 4 );
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Dock = DockStyle.Fill;
            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        }

        #region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.DockBottom; }
        }

        public DockingPosition DockingPositions
        {
            get
            {
                return DockingPosition.Left |
                       DockingPosition.Right |
                       DockingPosition.Top |
                       DockingPosition.Bottom;
            }
        }

        public bool HideOnClose
        {
            get { return true; }
        }

        public string Caption
        {
            get { return "Validation"; }
        }

        public object Icon
        {
            get { return null; }
        }

#endregion

        public void Add( UserControl view )
        {
            TabPage viewTab = new TabPage( view.Text )
                              {
                                  Name = view.Text,
                                  Dock = DockStyle.Fill,
                                  UseVisualStyleBackColor = true
                              };

            view.Dock = DockStyle.Fill;

            viewTab.Controls.Add( view );

            //if (this._validationWindow.TryFindTab(subject, out ValidationResultTab tab))
            //{

            //}

            /// replace the existing.
            if ( this.TabPages.ContainsKey( view.Text ) )
            {
                int insertIndex = this.TabPages.IndexOfKey( view.Text );

                viewTab.Controls.RemoveByKey( view.Text );

                this.TabPages.RemoveByKey( view.Text );

                this.TabPages.Insert( insertIndex, viewTab );
            }
            else
            {
                this.TabPages.Add( viewTab );
            }

            this.SelectedTab = viewTab;
        }

#region Overrides of Control

        /// <inheritdoc />
        //protected override void OnControlAdded( ControlEventArgs e )
        //{
        //    this.Visible = this.TabPages.Count > 0;
        //}

        /// <inheritdoc />
        protected override void OnMouseDown( MouseEventArgs e )
        {
            for ( var i = 0; i < this.TabPages.Count; i++ )
            {
                var tabRect = this.GetTabRect( i );
                tabRect.Inflate( -2, -2 );

                var imageRect = new Rectangle(
                    ( tabRect.Right - this._closeImage.Width ),
                    tabRect.Top + ( tabRect.Height - this._closeImage.Height ) / 2,
                    this._closeImage.Width,
                    this._closeImage.Height );

                if ( imageRect.Contains( e.Location ) )
                {
                    this.TabPages.RemoveAt( i );

                    this.Visible = this.TabPages.Count != 0;

                    break;
                }
            }
        }

#endregion

#region Overrides of TabControl

        /// <inheritdoc />
        protected override void OnDrawItem( DrawItemEventArgs e )
        {
            /// This system keeps supplying an index that's bigger than the
            /// collection!?
            if ( e.Index >= this.TabPages.Count )
            {
                return;
            }

            var tabPage = this.TabPages[ e.Index ];
            var tabRect = this.GetTabRect( e.Index );
            tabRect.Inflate( -2, -2 );

            e.Graphics.DrawImage( this._closeImage,
                ( tabRect.Right - this._closeImage.Width ),
                tabRect.Top + ( tabRect.Height - this._closeImage.Height ) / 2 );

            TextRenderer.DrawText( e.Graphics, tabPage.Text, tabPage.Font,
                tabRect, tabPage.ForeColor, TextFormatFlags.Left );
        }

#endregion
    }

}
