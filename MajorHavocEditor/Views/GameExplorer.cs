﻿using System;
using System.Collections;
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
using MajorHavocEditor.Services;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer : UserControl, IUserInterface
    {
        private readonly IWindowManager _windowManager;
        private readonly IFileManager _fileManager;
        private readonly IMenuManager _contextMenuManager = new ContextMenuManager();//ContextMenuManager();
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

            this.treeView.ContextMenuStrip = (ContextMenuStrip) this._contextMenuManager.Menu;

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

            MenuItem addMenuItem =
                new MenuItem("Add")
                {
                    Display = "Add",
                    GroupKey = new Guid(),
                    Icon = @"Resources\Images\Buttons\NewFileCollection_16x_24.bmp".CreateResourceUri()
                };

            MenuItem addMazeMenuItem =
                new MenuItem("Add Maze")
                {
                    ParentName = addMenuItem.Name,
                    Command = new MenuCommand(this.AddMazeCommand),
                    Display = "Maze",
                    Icon = @"Resources\Images\Buttons\NewMaze.bmp".CreateResourceUri()
                };

            MenuItem addMazeCollectionMenuItem =
                new MenuItem("Add MazeCollection")
                {
                    ParentName = addMenuItem.Name,
                    Command = new MenuCommand(this.AddMazeCollectionCommand),
                    Display = "Maze Collection",
                    Icon = @"Resources\Images\Buttons\NewCollection.bmp".CreateResourceUri()
                };

            this._contextMenuManager.Add(addMenuItem);
            this._contextMenuManager.Add(addMazeMenuItem);
            this._contextMenuManager.Add(addMazeCollectionMenuItem);

            menuManager.Add(addMenuItem);
            menuManager.Add(addMazeMenuItem);
            menuManager.Add(addMazeCollectionMenuItem);

            menuManager?.Add(
                new MenuItem("LoadFromROM")
                {
                    Command = new MenuCommand(this.LoadFromRomCommand),
                    Display = "Load From ROM",
                    ToolTip = "Load Maze Collection From ROM",
                    Icon = @"Resources\Images\Buttons\rom_32.png".CreateResourceUri()
                });

            this._contextMenuManager.Add( new Separator(addMenuItem, addMenuItem.GroupKey) );

            this._contextMenuManager.Add(
                new MenuItem("Open")
                {
                    Command = new MenuCommand(this.OpenFileCommand),
                    Display = "Open",
                    ShortcutKey = Keys.Control | Keys.O,
                    ToolTip = "Open a Maze or Collection from file.",
                    Icon = @"Resources\Images\Buttons\OpenFolder_16x_24.bmp".CreateResourceUri()
                });
            this._contextMenuManager.Add(
                new MenuItem("Close")
                {
                    Command = new MenuCommand( this.CloseCommand ),
                    Display = "Close",
                    ToolTip = "Close the active Maze or Collection.",
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));
            
            this._contextMenuManager.Add(
                new MenuItem("Save")
                {
                    Command = new MenuCommand(this.SaveCommand),
                    Display = "Save",
                    ShortcutKey = Keys.Control | Keys.S,
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Buttons\Save_16x_24.bmp".CreateResourceUri()
                });

            this._contextMenuManager.Add(
                new MenuItem("Save As")
                {
                    Command = new MenuCommand(this.SaveAsCommand),
                    Display = "Save As...",
                    ToolTip = "Save a Maze or Collection to file.",
                    Icon = @"Resources\Images\Buttons\SaveAs_16x_24.bmp".CreateResourceUri()
                });

            this._contextMenuManager.Add(
                new MenuItem("Save All")
                {
                    Command = new MenuCommand(this.SaveAllCommand),
                    Display = "Save All",
                    ShortcutKey = Keys.Control | Keys.Shift | Keys.S,
                    ToolTip = "Save All to file.",
                    Icon = @"Resources\Images\Buttons\SaveAll_16x_24.bmp".CreateResourceUri()
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem("Validate")
                {
                    Command = new MenuCommand(this.ValidateCommand),
                    Display = "Validate",
                    ShortcutKey = Keys.Control | Keys.V,
                    ToolTip = "Validate a Maze or Collection.",
                    Icon = @"Resources\Images\Buttons\ValidateDocument_315.png".CreateResourceUri()
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem("Delete")
                {
                    Command = new MenuCommand(this.DeleteCommand),
                    Display = "Delete",
                    ShortcutKey = Keys.Delete,
                    ToolTip = "Delete a Maze or Collection.",
                    Icon = @"Resources\Images\Buttons\delete.ico".CreateResourceUri()
                });

            this._contextMenuManager.Add(
                new MenuItem( "Rename" )
                {
                    Command = new MenuCommand( this.RenameCommand, this.IsOneItemSelected ),
                    Display = "Rename",
                    ToolTip = "Rename the Maze or MazeCollection",
                });

            this._contextMenuManager.Add(new Separator(addMenuItem, addMenuItem.GroupKey));

            this._contextMenuManager.Add(
                new MenuItem("Preview")
                {
                    Command = new MenuCommand(this.PreviewInHbMame),
                    Display = "Preview in HBMAME",
                    ShortcutKey = Keys.Control | Keys.P,
                    ToolTip = "Preview a maze in HBMAME",
                    Icon = @"Resources\Images\Buttons\hbmame_32.png".CreateResourceUri()
                });
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

        private void PreviewInHbMame( object obj )
        {
        }

        private void ValidateCommand( object obj )
        {
        }

        private void SaveAsCommand( object obj )
        {
        }

        private void SaveCommand( object obj )
        {
        }

        private void OpenFileCommand(object obj)
        {
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
                    this._windowManager.Show( maze );
                }
            }
        }

        private void OnTreeViewMouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (this.treeView.SelectedNode?.Tag is Maze maze &&
            this.treeView.SelectedNode.Bounds.Contains( e.X, e.Y ) )
            {
                this._windowManager.Show( maze, true );
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

}