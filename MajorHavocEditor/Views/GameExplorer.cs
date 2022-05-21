using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private readonly IFileManager _fileManager;
        private readonly IMenuManager _menuManager = new ContextMenuManager();
        private readonly ObservableCollection<IName> _mazes = new ObservableCollection<IName>();
        private readonly ObservableCollection<IName> _selectedMazes = new ObservableCollection<IName>();

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

            this.treeView.ItemsSource = this._mazes;
            this.treeView.ItemsDelegate = new ItemsSourceDelegate();
            this.treeView.SelectedItems = this._selectedMazes;

            this.treeView.ContextMenuStrip = (ContextMenuStrip) this._menuManager.Menu;

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
                    Command = new MenuCommand( this.LoadFromRomCommand ),
                    Display = "Load From ROM",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                } );

            MenuItem renameMenuItem = 
                new MenuItem( "Rename" )
                {
                    Command = new MenuCommand( this.RenameCommand, this.IsOneItemSelected ),
                    Display = "Rename",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                };

            menuManager?.Add( renameMenuItem );
            this._menuManager.Add(renameMenuItem);
        }

        private bool IsOneItemSelected( object unused )
        {
            return this._selectedMazes.Count == 1;
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

        private void LoadFromRomCommand( object obj )
        {
            DialogLoadROM dlr = new DialogLoadROM( Path.GetFullPath( "../" ) );

            DialogResult dr = dlr.ShowDialog();

            if ( dr == DialogResult.OK )
            {
                this._mazes.Add( dlr.MazeCollection );
            }
        }

        private void SaveAllCommand()
        {
            // On save all, write MazeCollections, and Loose Mazes that
            // that exist at top level of treeview. No need to go further
            // down as collections save child mazes.
            foreach ( object topLevelItem in this.treeView.ItemsSource )
            {
                if (topLevelItem is IChangeTracking changeTracking &&
                    changeTracking.IsChanged)
                {
                    this._fileManager.Save(topLevelItem);
                }
                //if ( topLevelItem is Maze maze && maze.IsChanged )
                //{
                //    this._fileManager.Save( maze );
                //}
                //else if ( topLevelItem is MazeCollection mazeCollection &&
                //          mazeCollection.IsChanged )
                //{
                //    this._fileManager.Save( mazeCollection );
                //}
            }
        }
        
        /// <summary>
        /// TODO: Decide if should force conditions.. like all from same parent, or same level.
        /// </summary>
        private void DeleteSelectedCommand()
        {
            // TODO: If Mazes had a "Parent" property this would be really fast...
            // Sort into groups with MazeCollections first.
            List<IGrouping<Type, object>> deleteList =
                this.treeView.SelectedItems
                    .Cast<object>()
                    .ToLookup( o => o.GetType() )
                    .OrderBy(o => o.Key == typeof(MazeCollection))
                    .ToList();

            foreach ( IGrouping<Type, object> grouping in deleteList )
            {
                if ( grouping.Key == typeof( MazeCollection ) )
                {
                    foreach ( object collectionToDelete in grouping )
                    {
                        // MazeCollections are always on top level.
                        this.treeView.ItemsSource.Remove(collectionToDelete);
                    }

                    // move on to 2nd grouping (Mazes)
                    continue;
                }

                foreach ( Maze mazeToDelete in grouping )
                {
                    foreach ( object itemInTree in this.treeView.ItemsSource )
                    {
                        if ( itemInTree.Equals( mazeToDelete ) )
                        {
                            // Top level maze
                            this.treeView.ItemsSource.Remove(mazeToDelete);
                        }
                        else if ( itemInTree is MazeCollection mazeCollection &&
                                  mazeCollection.Mazes.Contains( mazeToDelete ))
                        {
                            mazeCollection.Mazes.Remove( mazeToDelete );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This closes a singular... Not sure about logic here
        /// </summary>
        private void CloseCommand()
        {
            bool safeToRemove = this.treeView.SelectedItem is IChangeTracking changeTracking ?
                !(changeTracking.IsChanged && this._fileManager.Save(this.treeView.SelectedItem)):
                true;

            // Find maze to close.. this consistently seems messy now.
            // Too much searching for the parent..

            if ( !safeToRemove )
            {
                if ( this.treeView.SelectedItem is MazeCollection ||
                     this.treeView.ItemsSource.Contains( this.treeView.SelectedItem ))
                {
                    this.treeView.ItemsSource.Remove( this.treeView.SelectedItem);
                }
                else
                {
                    Maze maze = (Maze) this.treeView.SelectedItem;

                    foreach ( object item in this.treeView.ItemsSource )
                    {
                        
                    }
                }
            }
        }

        private void RenameCommand(object unused)
        {
            this.treeView.SelectedNode?.BeginEdit();
        }
    }

    public interface IFileProperties
    {
        //bool PromptToSave { get; } Just use changetracking

        /// <summary>
        /// Description of file type
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The Filename for this object, or null if it doesn't have one. 
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// The file extension for this object.
        /// </summary>
        string Extension { get; set; }

        /// <summary>
        /// The path on disk for this object, or null if it doesn't have one. If the
        /// <see cref="Filename"/> is null then this represents the default storage location.
        /// </summary>
        string Path { get; set; }
    }

    internal interface IFileManager
    {
        /// <summary>
        /// Attempt to save the object to file.
        /// </summary>
        /// <param name="toBeSaved"></param>
        /// <param name="path"></param>
        /// <returns>True if save succeeded. False otherwise</returns>
        bool Save( object toBeSaved, string path = null );
        //bool Save(IFileProperties toBeSaved, string path = null);

        T Load<T>( IFileProperties toBeLoaded, Func<FileStream, T> postProcessing );
    }

}
