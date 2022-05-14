using System.Drawing;
using System.Windows.Forms;
using mhedit;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{
    public partial class MazeView : UserControl, IUserInterface
    {
        private object _icon;
        private Maze _maze;

        public MazeView( Maze maze )
        {
            InitializeComponent();

            this.Controls.Add( new MazeController( maze )
                               {
                                   AutoSize = true,
                                   Anchor = AnchorStyles.Top | AnchorStyles.Left,
                                   Dock = DockStyle.None,
                               } );

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this._maze = maze;

            this.BackColor = Color.Black;

            this.Dock = DockStyle.Fill;

            AutoSize = true;

            this.AutoScroll = true;

            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        public Maze Maze
        {
            get { return this._maze; }
        }

#region Overrides of ContainerControl

        /// <summary>
        /// Fixes that annoying scroll when you first click in a maze.
        /// <see cref="https://stackoverflow.com/a/9317532"/>
        /// </summary>
        protected override Point ScrollToControl( Control activeControl )
        {
            // Returning the current location prevents the panel from
            // scrolling to the active control when the panel loses and regains focus
            return this.DisplayRectangle.Location;
        }

#endregion

#region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.Document; }
        }

        public DockingPosition DockingPositions
        {
            get { return DockingPosition.Document; }
        }

        public bool HideOnClose
        {
            get { return false; }
        }

        public string Caption
        {
            get { return this.Maze.Name; }
        }

        public object Icon
        {
            get { return this.Controls[ 0 ].ClientRectangle; }
        }

#endregion
    }
}
