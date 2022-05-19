using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using mhedit;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer : UserControl, IUserInterface
    {
        private readonly IWindowManager _windowManager;

        public GameExplorer()
            : this( null, null )
        {}

        public GameExplorer( IMenuManager menuManager, IWindowManager windowManager )
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            this._windowManager = windowManager;

            this.treeView.ItemsSource.ItemsDelegate = new ItemsSourceDelegate();

            this.treeView.ImageList =
                new ImageList { TransparentColor = Color.Fuchsia }
                    .AddImages( new[]
                                 {
                                     "ThumbnailViewHS.bmp",
                                     "maze_a.bmp",
                                     "maze_b.bmp",
                                     "maze_c.bmp",
                                     "maze_d.bmp"
                                 } )
                    .WithResourcePath( "Resources/Images" )
                    .Load();

            this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
            this.treeView.MouseDoubleClick += this.OnTreeViewMouseDoubleClick;

            menuManager?.Add(
                new MenuItem( "LoadFromROM" )
                {
                    Command = new MenuCommand( this.OnLoadFromRomCommand ),
                    Display = "Load From ROM",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                } );

            menuManager?.Add(
                new MenuItem("SelectMaze")
                {
                    Command = new MenuCommand(
                        _ => ((IList)this.treeView.SelectedItems).Add(
                            this.treeView.Nodes[0].Nodes[new Random().Next(0, 27)].Tag)),
                    Display = "zap",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                });

            menuManager?.Add(
                new MenuItem("DeleteMaze")
                {
                    Command = new MenuCommand(_ => this.RemoveNode(new Random().Next(0, 27))),
                    Display = "delete",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                });

        }

        private void RemoveNode( int next )
        {
            TreeNode node = this.treeView.Nodes[ 0 ].Nodes[ next ];

            ( (MazeCollection) this.treeView.ItemsSource[ 0 ] ).Mazes.Remove( (Maze) node.Tag );
        }

#region Implementation of IUserInterface

        public DockingState DockingState
        {
            get { return DockingState.DockLeft; }
        }

        public DockingPosition DockingPositions
        {
            get
            {
                return DockingPosition.Float |
                       DockingPosition.Left |
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
            get { return "Game Explorer"; }
        }

        public object Icon
        {
            get { return null; }
        }

#endregion

        private void OnTreeViewAfterSelect( object sender, TreeViewEventArgs e )
        {
            if ( this.treeView.SelectedNode?.Tag is Maze maze )
            {
                if ( !ModifierKeys.HasFlag( Keys.Control ) )
                {
                    MazeUi mazeUi = (MazeUi)
                        this._windowManager.Interfaces.FirstOrDefault(
                            ui => ui is MazeUi mui && mui.Maze.Equals( maze ) );

                    if ( mazeUi != null )
                    {
                        this._windowManager.Show( mazeUi );
                    }
                }
            }
        }

        private void OnTreeViewMouseDoubleClick( object sender, MouseEventArgs e )
        {
            if ( this.treeView.SelectedNode.Bounds.Contains( e.X, e.Y ) &&
                 this.treeView.SelectedNode?.Tag is Maze maze )
            {
                MazeUi mazeUi = (MazeUi)
                                this._windowManager.Interfaces.FirstOrDefault(
                                    ui => ui is MazeUi mui && mui.Maze.Equals( maze ) ) ??
                                new MazeUi( maze );

                this._windowManager.Show( mazeUi );
            }
        }

        private void OnLoadFromRomCommand( object obj )
        {
            DialogLoadROM dlr = new DialogLoadROM( Path.GetFullPath( "../" ) );

            DialogResult dr = dlr.ShowDialog();

            if ( dr == DialogResult.OK )
            {
                this.treeView.ItemsSource.Add( dlr.MazeCollection );
            }
        }
    }

}
