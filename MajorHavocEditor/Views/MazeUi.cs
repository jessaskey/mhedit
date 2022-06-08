using System;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;
using MHavocEditor;

namespace MajorHavocEditor.Views
{

    public partial class MazeUi : UserControl, IUserInterface
    {
        private GameToolbox _gameToolbox = new GameToolbox();
        private MazeExplorer _mazeExplorer;
        private WindowManager _windowManager;
        private PropertyBrowser _propertyBrowser;
        private Maze _maze;

        public MazeUi( Maze maze )
        {
            this._maze = maze;

            InitializeComponent();

            // I need to wrap the maze controller to get scrollbars and fix a focus bug.
            CustomPanel wrapper = new CustomPanel()
                                  {
                                      Anchor = AnchorStyles.Top | AnchorStyles.Left,
                                      Dock = DockStyle.Fill,
                                      BackColor = Color.Black,
                                      AutoSize = true,
                                      AutoScroll = true,
                                      AutoSizeMode = AutoSizeMode.GrowAndShrink,
                                  };

            MazeController mc =
                new MazeController( maze )
                {
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Dock = DockStyle.None,
                };

            wrapper.Controls.Add( mc );

            this.kryptonPanel.Controls.Add( wrapper );

            this._mazeExplorer = new MazeExplorer( maze, mc.SelectedMazeObjects );

            this._windowManager = new WindowManager( this.kryptonDockingManager );

            this._propertyBrowser = new PropertyBrowser( this._mazeExplorer.SelectedObjects );

            this.Dock = DockStyle.Fill;
        }

        public Maze Maze
        {
            get { return this._maze; }
        }

#region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.Document; }
        }

        public DockingPosition DockingPositions
        {
            get { return DockingPosition.Document | DockingPosition.Float; }
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
            get { return null; }
        }

#endregion

#region Overrides of UserControl

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            // Setup docking functionality. No floating!
            this.kryptonDockingManager.ManageControl( this.kryptonPanel );
            this.kryptonDockingManager.ManageFloating( Application.OpenForms[ 0 ] );

            this._windowManager.Hide( this._gameToolbox );
            this._windowManager.Show( this._mazeExplorer );
            this._windowManager.Show( this._propertyBrowser );
        }

#endregion

    }

}
