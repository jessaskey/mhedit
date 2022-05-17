using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using mhedit;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer : UserControl, IUserInterface
    {
        private readonly IWindowManager _windowManager;

        private static readonly string ResourcePath =
            $"/{typeof( GameExplorer ).Assembly.GetName().Name};component/Resources/Images/";

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

            this.treeView.ImageList = new ImageList( this.components )
                                      {
                                          TransparentColor = Color.Fuchsia,
                                      };

            this.treeView.ImageList.Images.Add( this.LoadImage( "ThumbnailViewHS.bmp" ) );
            this.treeView.ImageList.Images.Add( this.LoadImage( "maze_a.bmp" ) );
            this.treeView.ImageList.Images.Add( this.LoadImage( "maze_b.bmp" ) );
            this.treeView.ImageList.Images.Add( this.LoadImage( "maze_c.bmp" ) );
            this.treeView.ImageList.Images.Add( this.LoadImage( "maze_d.bmp" ) );

            this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
            this.treeView.MouseDoubleClick += this.OnTreeViewMouseDoubleClick;

            menuManager?.Add(
                new MenuItem( "LoadFromROM" )
                {
                    Command = new MenuCommand( this.OnLoadFromRomCommand ),
                    Display = "Load From ROM",
                    Icon = PackUriHelper.Create( ResourceLoader.ApplicationUri,
                        new Uri( $"{ResourcePath}Buttons/rom_32.png", UriKind.Relative ) )
                } );
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

        private Image LoadImage( string imagePath )
        {
            return ResourceLoader.GetEmbeddedImage( this.GetImageUri( imagePath ) );
        }

        private Uri GetImageUri( string imagePath )
        {
            return PackUriHelper.Create( ResourceLoader.ApplicationUri,
                new Uri( $"{ResourcePath}{imagePath}", UriKind.Relative ) );
        }

        private void OnLoadFromRomCommand( object obj )
        {
            DialogLoadROM dlr = new DialogLoadROM( Path.GetFullPath( "../" ) );

            DialogResult dr = dlr.ShowDialog();

            if ( dr == DialogResult.OK )
            {
                dlr.MazeCollection.PropertyChanged += this.OnMazeCollectionPropertyChanged;

                this.treeView.SelectedNode = this.Add( dlr.MazeCollection );
            }
        }

        private void OnMazeCollectionPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
        }

        private TreeNode Add( MazeCollection mazeCollection )
        {
            TreeNode mazeCollectionNode;

            try
            {
                this.treeView.BeginUpdate();

                mazeCollectionNode = new KryptonTreeNode( mazeCollection.Name )
                                     {
                                         Tag = mazeCollection,
                                         ImageIndex = 0,
                                         SelectedImageIndex = 0
                                     };

                foreach ( Maze maze in mazeCollection.Mazes )
                {
                    this.AddSibling( mazeCollectionNode, maze );
                }

                this.treeView.Nodes.Insert(
                    this.treeView.Nodes.IndexOf(
                        this.treeView.SelectedNode?.Parent ??
                        this.treeView.SelectedNode ) + 1, mazeCollectionNode );

                mazeCollectionNode.Expand();

                if ( !mazeCollectionNode.IsVisible )
                {
                    // must end update before EnsureVisible 
                    this.treeView.EndUpdate();
                    mazeCollectionNode.EnsureVisible();
                }
            }
            finally
            {
                this.treeView.EndUpdate();
            }

            return mazeCollectionNode;
        }

        private void AddSibling( TreeNode parent, Maze maze )
        {
            TreeNode child = new KryptonTreeNode( maze.Name )
                             {
                                 Tag = maze,
                                 ForeColor = Color.Black,
                                 ImageIndex = (int) maze.MazeType + 1,
                                 SelectedImageIndex = (int) maze.MazeType + 1
                             };

            parent.Nodes.Add( child );
        }
    }

}
