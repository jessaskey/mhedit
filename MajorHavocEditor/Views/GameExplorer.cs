using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using mhedit.Containers;
using mhedit.Extensions;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;
using MajorHavocEditor.Services;
using MajorHavocEditor.Views.Dialogs;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer : UserControl, IUserInterface
    {
        private static readonly ImageList IconList;

        private readonly IWindowManager _windowManager;
        private readonly IMenuManager _contextMenuManager = new ContextMenuManager();
        private readonly IMameManager _mameManager;

        private readonly ObservableCollection<IFileProperties> _mazes =
            new ObservableCollection<IFileProperties>();
        private readonly ObservableCollection<IFileProperties> _selectedMazes =
            new ObservableCollection<IFileProperties>();

        private ICommand _validateCommand;

        static GameExplorer()
        {
            IconList =
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
        }

        //public GameExplorer()
        //    : this( null, null )
        //{}

        public GameExplorer( IMenuManager menuManager, IWindowManager windowManager,
            IMameManager mameManager )
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            this._windowManager = windowManager;

            this._mameManager = mameManager;

            this.treeView.ItemsSource = this._mazes;
            this.treeView.ItemsDelegate = new ItemsSourceDelegate();
            this.treeView.SelectedItems = this._selectedMazes;

            this.treeView.ContextMenuStrip = (ContextMenuStrip) this._contextMenuManager.Menu;

            this.treeView.ImageList = IconList;

            this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
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
                    Command = new MenuCommand(this.AddMazeCommand),
                    Display = "Maze",
                    Icon = @"Resources\Images\Menu\NewMaze.bmp".CreateResourceUri()
                };

            MenuItem addMazeCollectionMenuItem =
                new MenuItem("GameExplorer_AddMazeCollection")
                {
                    ParentName = addMenuItem.Name,
                    Command = new MenuCommand(this.AddMazeCollectionCommand),
                    Display = "Maze Collection",
                    Icon = @"Resources\Images\Menu\NewCollection.bmp".CreateResourceUri()
                };

            this._contextMenuManager.Add(addMenuItem);
            this._contextMenuManager.Add(addMazeMenuItem);
            this._contextMenuManager.Add(addMazeCollectionMenuItem);

            IMenuItem open =
                new MenuItem("GameExplorer_Open")
                {
                    Command = new MenuCommand(this.OpenFileCommand),
                    Display = "Open",
                    ShortcutKey = Keys.Control | Keys.O,
                    ToolTip = "Open a Maze or Collection from file.",
                    Icon = @"Resources\Images\Menu\OpenFolder_16x_24.bmp".CreateResourceUri()
                };

            IMenuItem save =
                new MenuItem( "GameExplorer_Save" )
                {
                    Command = new MenuCommand( this.SaveCommand ),
                    Display = "Save",
                    ShortcutKey = Keys.Control | Keys.S,
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Menu\Save_16x_24.bmp".CreateResourceUri()
                };

            IMenuItem saveAll =
                new MenuItem( "GameExplorer_SaveAll" )
                {
                    Command = new MenuCommand( this.SaveAllCommand ),
                    Display = "Save All",
                    ShortcutKey = Keys.Control | Keys.Shift | Keys.S,
                    ToolTip = "Save All to file.",
                    Icon = @"Resources\Images\Menu\SaveAll_16x_24.bmp".CreateResourceUri()
                };

            menuManager.Add(addMenuItem);
            menuManager.Add(addMazeMenuItem);
            menuManager.Add(addMazeCollectionMenuItem);
            menuManager.Add(open);
            menuManager.Add(save);
            menuManager.Add(saveAll);

            menuManager?.Add(
                new MenuItem("GameExplorer_LoadFromROM")
                {
                    Command = new MenuCommand(this.LoadFromRomCommand),
                    Display = "Load From ROM",
                    ToolTip = "Load Maze Collection From ROM",
                    Icon = @"Resources\Images\Menu\rom_32.png".CreateResourceUri()
                });

            this._contextMenuManager.Add( new Separator(addMenuItem, addMenuItem.GroupKey) );

            this._contextMenuManager.Add( open );

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Close")
                {
                    Command = new MenuCommand( this.CloseCommand ),
                    Display = "Close",
                    ToolTip = "Close the active Maze or Collection.",
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add( save );

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_SaveAs")
                {
                    Command = new MenuCommand(this.SaveAsCommand),
                    Display = "Save As...",
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Menu\SaveAs_16x_24.bmp".CreateResourceUri()
                });

            this._contextMenuManager.Add( saveAll );

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Delete")
                {
                    Command = new MenuCommand(this.DeleteCommand),
                    Display = "Delete",
                    ShortcutKey = Keys.Delete,
                    ToolTip = "Delete a Maze or Collection.",
                    Icon = @"Resources\Images\Menu\delete.ico".CreateResourceUri()
                });

            this._contextMenuManager.Add(
                new MenuItem("GameExplorer_Rename")
                {
                    Command = new MenuCommand( this.RenameCommand, this.IsOneItemSelected ),
                    Display = "Rename",
                    ToolTip = "Rename the Maze or MazeCollection",
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem( "GameExplorer_Preview" )
                {
                    Command = new MenuCommand( this.PreviewInHbMame, this.SingleMazeSelected ),
                    Display = "Preview in HBMAME",
                    ShortcutKey = Keys.Control | Keys.P,
                    ToolTip = "Preview a maze in HBMAME",
                    Icon = @"Resources\Images\Menu\hbmame_32.png".CreateResourceUri()
                } );
        }

        public ICommand ValidateCommand
        {
            get { return this._validateCommand; }
            set
            {
                this._validateCommand = value;

                IMenuItem validate =
                    new MenuItem( "GameExplorer_Validate" )
                    {
                        Command = new MenuCommand( o => this.ValidateCommand.Execute( this.SelectedItems ),
                            o => this.ValidateCommand.CanExecute( this.SelectedItems ) ),
                        Display = "Validate",
                        GroupKey = new Guid(),
                        SortOrder = 10,
                        ShortcutKey = Keys.Control | Keys.V,
                        ToolTip = "Validate a Maze or Collection.",
                        Icon = @"Resources\Images\Menu\ValidateDocument_315.png"
                            .CreateResourceUri()
                    };

                this._contextMenuManager.Add(
                    new Separator( validate, validate.GroupKey ) { SortOrder = 9 } );

                this._contextMenuManager.Add(validate);
            }
        }

        public IList SelectedItems
        {
            get { return this.treeView.SelectedItems; }
        } 

        private void AddMazeCollectionCommand( object obj )
        {
            if ( this.treeView.SelectedItems.Count < 2 )
            {
                int index = this.treeView.ItemsSource.IndexOf( this.treeView.SelectedItem );

                if ( index < 1 )
                {
                    this.treeView.ItemsSource.Add( new MazeCollection() );
                }
                else
                {
                    this.treeView.ItemsSource.Insert( index + 1, new MazeCollection() );
                }
            }
        }

        private void AddMazeCommand( object obj )
        {
            if ( this.treeView.SelectedItems.Count < 2 )
            {
                if ( this.treeView.SelectedItem == null )
                {
                    this.treeView.ItemsSource.Add( new Maze() );
                }
                else if ( this.treeView.SelectedItem is MazeCollection selectedMazeCollection )
                {
                    // Will generate name using the MazeCollection.
                    selectedMazeCollection.Mazes.Add( new Maze( string.Empty ) );
                }
                else
                {
                    // Find where the selected Maze is...
                    if ( this.TryFindLocation( this.treeView.SelectedItem, out int index, out IList list ) )
                    {
                        // Will generate name using the MazeCollection.
                        list.Insert( index + 1,
                            ReferenceEquals( list, this.treeView.ItemsSource ) ?
                                new Maze() :
                                new Maze( string.Empty ) );
                    }
                    else
                    {
                        this.treeView.ItemsSource.Add( new Maze() );
                    }
                }
            }
        }

        private bool TryFindLocation( object item, out int index, out IList list )
        {
            list = this.treeView.ItemsSource;

            index = this.treeView.ItemsSource.IndexOf(item);

            if ( index < 0 )
            {
                int loopIndex = -1;

                list = this.treeView.ItemsSource.OfType<MazeCollection>()
                    .FirstOrDefault(
                        c => (loopIndex = ( (IList) c.Mazes ).IndexOf( item ) ) > -1 )?.Mazes;

                index = loopIndex;
            }

            return index > -1;
        }

        private bool SingleMazeSelected(object notUsed)
        {
            return this._selectedMazes.Count == 1 &&
                   this._selectedMazes[0] is Maze;
        }

        private void PreviewInHbMame( object obj )
        {
            if ( this._selectedMazes.Count == 1 &&
                 this._selectedMazes[0] is Maze maze )
            {
                MazeCollection collection =
                this._mazes.OfType<MazeCollection>()
                    .FirstOrDefault( mc => mc.Mazes.Contains( maze ) );

                if ( collection != null )
                {
                    this._mameManager.Preview( collection, maze );
                }
            }
        }

        private void SaveAsCommand( object obj )
        {
            if (this._selectedMazes.Count == 1 )
            {
                this.Save( this._selectedMazes.First(), true );
            }
        }

        private void SaveCommand( object obj )
        {
            if (this._selectedMazes.Count == 1)
            {
                this.Save( this._selectedMazes.First() );
            }
        }

        private void OpenFileCommand(object obj)
        {
            OpenFileDialog ofd =
                new OpenFileDialog
                {
                    Title = "Open Maze or Maze Collection",
                    InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                    Filter = "Editor Files (*.mhz;*.mhc)|*.mhz;*.mhc|Mazes (*.mhz)|*.mhz|Maze Collections (*.mhc)|*.mhc",
                    CheckFileExists = true,
                };

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                object opened = Path.GetExtension(ofd.FileName).ToLowerInvariant() switch
                {
                    ".mhz" => this.Open<Maze>(ofd.FileName),
                    ".mhc" => this.Open<MazeCollection>(ofd.FileName),
                    _ => throw new ArgumentOutOfRangeException(
                             $"{Path.GetExtension(ofd.FileName)} is not a supported extension.")
                };

                //TODO: Insert after Parent.
                if ( opened != null )
                {
                    this.treeView.ItemsSource.Add( opened );
                }
            }
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
                    Debug.WriteLine("OnTreeViewAfterSelect");
                    this._windowManager.Show( maze );
                }
            }
        }

        private void OnTreeViewMouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (this.treeView.SelectedNode?.Tag is Maze maze &&
            this.treeView.SelectedNode.Bounds.Contains( e.X, e.Y ) )
            {
                Debug.WriteLine("OnTreeViewMouseDoubleClick");
                this._windowManager.Show( maze, true );
            }
        }

        private void LoadFromRomCommand( object obj )
        {
            DialogLoadROM dlr = new DialogLoadROM();

            DialogResult dr = dlr.ShowDialog();

            if ( dr == DialogResult.OK )
            {
                this._mazes.Add( dlr.MazeCollection );
            }
        }

        private void SaveAllCommand( object o )
        {
            // On save all, write MazeCollections, and Loose Mazes that
            // that exist at top level of treeview. No need to go further
            // down as collections save child mazes.
            foreach ( object topLevelItem in this.treeView.ItemsSource )
            {
                if (topLevelItem is IChangeTracking changeTracking &&
                    changeTracking.IsChanged)
                {
                }
            }
        }

        /// <summary>
        /// TODO: Decide if should force conditions.. like all from sam parent, or same level.
        /// </summary>
        private void DeleteCommand(object obj)
        {
            DialogResult result = MessageBox.Show(
                this.treeView.SelectedItems.Count == 1 ?
                    $"{this.treeView.SelectedItems.Cast<IName>().First().Name} will be deleted permanently!" :
                    $"All Selected nodes will be deleted permanently!",
                "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);

            if (result == DialogResult.OK)
            {
                // TODO: If Mazes had a "Parent" property this would be really fast...
                // Sort into groups with MazeCollections first.
                List<IGrouping<Type, object>> deleteList =
                    this.treeView.SelectedItems
                        .Cast<object>()
                        .ToLookup(o => o.GetType())
                        .OrderBy(o => o.Key == typeof(MazeCollection))
                        .ToList();

                /// BUG: Should warn of unsaved changes.

                foreach (IGrouping<Type, object> grouping in deleteList)
                {
                    if (grouping.Key == typeof(MazeCollection))
                    {
                        foreach (MazeCollection collectionToDelete in grouping)
                        {
                            foreach ( Maze maze in collectionToDelete.Mazes )
                            {
                                this._windowManager.Remove( maze );
                            }

                            // MazeCollections are always on top level.
                            this.treeView.ItemsSource.Remove(collectionToDelete);
                        }

                        // move on to 2nd grouping (Mazes)
                        continue;
                    }

                    foreach (Maze mazeToDelete in grouping)
                    {
                        IList copy = this.treeView.ItemsSource.Cast<object>().ToList();

                        foreach (object itemInTree in copy)
                        {
                            this._windowManager.Remove(mazeToDelete);

                            if (itemInTree.Equals(mazeToDelete))
                            {
                                // Top level maze
                                this.treeView.ItemsSource.Remove(mazeToDelete);
                            }
                            else if (itemInTree is MazeCollection mazeCollection &&
                                     mazeCollection.Mazes.Contains(mazeToDelete))
                            {
                                mazeCollection.Mazes.Remove(mazeToDelete);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This closes a singular... Not sure about logic here
        /// </summary>
        private void CloseCommand( object o )
        {
            if ( this._selectedMazes.Count != 1 )
            {
                return;
            }

            IFileProperties itemToClose = this._selectedMazes.First();

            bool safeToRemove = itemToClose is IChangeTracking changeTracking ?
                                    !changeTracking.IsChanged :
                                    true;

            if ( !safeToRemove )
            {
                // prompt user to save...
                DialogResult result = MessageBox.Show(
                    $"Save changes to {itemToClose.Name}?",
                    "Close", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if ( result == DialogResult.Yes )
                {
                    safeToRemove = this.Save( itemToClose );
                }
            }

            if ( !safeToRemove )
            {
                this.TryFindLocation( itemToClose, out int _, out IList list );

                list.Remove( itemToClose );
            }
        }

        private void RenameCommand(object unused)
        {
            this.treeView.SelectedNode?.BeginEdit();
        }

        private bool Save( IFileProperties file, bool saveAs = false )
        {
            DialogResult result = DialogResult.OK;

            // if there isn't a file associated with this MazeCollection then ask
            // for the fileName. 
            if (string.IsNullOrWhiteSpace(file.Path) || saveAs )
            {
                SaveFileDialog sfd =
                    new SaveFileDialog
                    {
                        InitialDirectory = file.Path ??
                                           Environment.GetFolderPath(
                                               Environment.SpecialFolder.MyDocuments ),
                        FileName = file.Name,
                        Filter = $"{file.GetType().Name} Files (*{file.Extension})|*{file.Extension}|All files (*.*)|*.*",
                        AddExtension = true,
                        OverwritePrompt = true
                    };

                // capture user choice for save operation below.
                result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    file.Name = Path.GetFileName(sfd.FileName);
                    file.Path = Path.GetDirectoryName( sfd.FileName );
                }
            }

            try
            {
                if (result == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Application.DoEvents();

                    file.SerializeAndCompress();

                    if ( file is IChangeTracking ict )
                    {
                        ict.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = DialogResult.Cancel;

                MessageBox.Show(
                    $@"An error has occurred while trying to save: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "An Error Occurred",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result == DialogResult.OK;
        }

        private T Open<T>( string fileName ) where T : class
        {
            Cursor.Current = Cursors.WaitCursor;
            
            Application.DoEvents();

            object result = null;

            try
            {
                result = fileName.ExpandAndDeserialize<T>( HandleNotifications );

                void HandleNotifications( string message )
                {
                    MessageBox.Show(message, "Maze Load Issues",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                };

                if ( result is IFileProperties fileProperties )
                {
                    fileProperties.Name = Path.GetFileName( fileName );

                    fileProperties.Path = Path.GetFullPath( fileName );
                }

                if ( result is IChangeTracking ict )
                {
                    ict.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show($@"Maze could not be opened: {(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}",
                    "File Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                //Bryan, I put this here as an example of how to report Exceptions that are caught, but you still
                //may want to log them. All un-handled exceptions will still log.
                //Program.SendException("MazeOpen", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

            return result as T;
        }
    }

}
