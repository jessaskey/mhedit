using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Controls.Commands;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;
using MajorHavocEditor.Services;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer : UserControl, IUserInterface
    {
        private static readonly ImageList IconList;

        static GameExplorer()
        {
            IconList =
                new ImageList { TransparentColor = Color.Fuchsia }
                    .AddImages(new[]
                               {
                                   "ThumbnailViewHS.bmp",
                                   "maze_a.bmp",
                                   "maze_b.bmp",
                                   "maze_c.bmp",
                                   "maze_d.bmp"
                               })
                    .WithResourcePath("Resources/Images")
                    .Load();
        }

        private readonly IWindowManager _windowManager;
        private readonly IMenuManager _contextMenuManager = new ContextMenuManager();
        private IMenuItem Open;

        public GameExplorer( IMenuManager menuManager, IWindowManager windowManager,
            GameManager gameManager )
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            this._windowManager = windowManager;

            this.treeView.ItemsSource = gameManager.GameObjects;
            this.treeView.ItemsDelegate = new GameExplorerItemsSourceDelegate();
            this.treeView.SelectedItems = gameManager.SelectedObjects;
            
            this.treeView.ContextMenuStrip = (ContextMenuStrip) this._contextMenuManager.Menu;

            this.treeView.ImageList = IconList;

            this.treeView.MouseDoubleClick += this.OnTreeViewMouseDoubleClick;

            MenuItem addMenuItem =
                new MenuItem("GameExplorer_Add")
                {
                    Display = "Add",
                    GroupKey = new Guid(),
                    Icon = @"Resources\Images\Menu\NewFileCollection_16x_24.bmp".CreateResourceUri()
                };

            MenuItem addMazeMenuItem =
                new MenuItem("GameExplorer_AddMaze")
                {
                    ParentName = addMenuItem.Name,
                    Command = gameManager.AddMazeCommand,
                    Display = "Maze",
                    Icon = @"Resources\Images\Menu\NewMaze.bmp".CreateResourceUri()
                };

            MenuItem addMazeCollectionMenuItem =
                new MenuItem("GameExplorer_AddMazeCollection")
                {
                    ParentName = addMenuItem.Name,
                    Command = gameManager.AddMazeCollectionCommand,
                    Display = "Maze Collection",
                    Icon = @"Resources\Images\Menu\NewCollection.bmp".CreateResourceUri()
                };

            this._contextMenuManager.Add(addMenuItem);
            this._contextMenuManager.Add(addMazeMenuItem);
            this._contextMenuManager.Add(addMazeCollectionMenuItem);

            this.Open =
                new MenuItem("GameExplorer_OpenMaze")
                {
                    Command = gameManager.OpenMazeCommand,
                    Display = "Open",
                    ShortcutKey = Keys.Control | Keys.O,
                    ToolTip = "Open Maze for editing.",
                    Icon = @"Resources\Images\Menu\Open_16x.png".CreateResourceUri()
                };

            IMenuItem load =
                new MenuItem("GameExplorer_LoadFromFile")
                {
                    Command = gameManager.LoadFromFileCommand,
                    Display = "Load from File",
                    ShortcutKey = Keys.Control | Keys.O,
                    ToolTip = "Load a Maze or Collection from file.",
                    Icon = @"Resources\Images\Menu\OpenFolder_16x_24.bmp".CreateResourceUri()
                };

            IMenuItem save =
                new MenuItem( "GameExplorer_Save" )
                {
                    Command = gameManager.SaveCommand,
                    Display = "Save",
                    ShortcutKey = Keys.Control | Keys.S,
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Menu\Save_16x_24.bmp".CreateResourceUri()
                };

            IMenuItem saveAll =
                new MenuItem( "GameExplorer_SaveAll" )
                {
                    Command = gameManager.SaveAllCommand,
                    Display = "Save All",
                    ShortcutKey = Keys.Control | Keys.Shift | Keys.S,
                    ToolTip = "Save All to file.",
                    Icon = @"Resources\Images\Menu\SaveAll_16x_24.bmp".CreateResourceUri()
                };

            menuManager.Add(addMenuItem);
            menuManager.Add(addMazeMenuItem);
            menuManager.Add(addMazeCollectionMenuItem);
            menuManager.Add(load);
            menuManager.Add(save);
            menuManager.Add(saveAll);

            menuManager?.Add(
                new MenuItem("GameExplorer_LoadFromROM")
                {
                    Command = gameManager.LoadFromRomCommand,
                    Display = "Load From ROM",
                    ToolTip = "Load Maze Collection From ROM",
                    Icon = @"Resources\Images\Menu\rom_32.png".CreateResourceUri()
                });

            this._contextMenuManager.Add(this.Open);

            this._contextMenuManager.Add( new Separator(addMenuItem, addMenuItem.GroupKey) );

            this._contextMenuManager.Add( load );

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Close")
                {
                    Command = gameManager.CloseCommand,
                    Display = "Close",
                    ToolTip = "Close the active Maze or Collection.",
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add( save );

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_SaveAs")
                {
                    Command = gameManager.SaveAsCommand,
                    Display = "Save As...",
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Menu\SaveAs_16x_24.bmp".CreateResourceUri()
                });

            this._contextMenuManager.Add( saveAll );

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Delete")
                {
                    Command = gameManager.DeleteCommand,
                    Display = "Delete",
                    ShortcutKey = Keys.Delete,
                    ToolTip = "Delete a Maze or Collection.",
                    Icon = @"Resources\Images\Menu\delete.ico".CreateResourceUri()
                });

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Rename")
                {
                    Command = new DelegateCommand( this.RenameCommand, this.IsOneItemSelected ),
                    Display = "Rename",
                    ToolTip = "Rename the Maze or MazeCollection",
                });

            IMenuItem validate =
                new MenuItem("GameExplorer_Validate")
                {
                    Command = gameManager.ValidateCommand,
                    Display = "Validate",
                    GroupKey = new Guid(),
                    SortOrder = 10,
                    ShortcutKey = Keys.Control | Keys.V,
                    ToolTip = "Validate a Maze or Collection.",
                    Icon = @"Resources\Images\Menu\ValidateDocument_315.png"
                        .CreateResourceUri()
                };

            this._contextMenuManager.Add(
                new Separator(validate, validate.GroupKey) { SortOrder = 9 });

            this._contextMenuManager.Add(validate);

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem( "GameExplorer_Preview" )
                {
                    Command = gameManager.PreviewInHbMameCommand,
                    Display = "Preview in HBMAME",
                    ShortcutKey = Keys.Control | Keys.P,
                    ToolTip = "Preview a maze in HBMAME",
                    Icon = @"Resources\Images\Menu\hbmame_32.png".CreateResourceUri()
                } );
        }

        private bool IsOneItemSelected()
        {
            return this.treeView.SelectedItems.Count == 1;
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

        private void OnTreeViewMouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (this.treeView.SelectedNode?.Tag is Maze maze &&
            this.treeView.SelectedNode.Bounds.Contains( e.X, e.Y ) )
            {
                Debug.WriteLine("OnTreeViewMouseDoubleClick");
                this._windowManager.Show(maze, true);
            }
        }

        private void RenameCommand()
        {
            this.treeView.SelectedNode?.BeginEdit();
        }
    }

}
