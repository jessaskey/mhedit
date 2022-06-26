using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Views;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Services
{

    public class GameManager
    {
        private ObservableCollection<object> _gameObjects = new ObservableCollection<object>();
        private ObservableCollection<object> _selected = new ObservableCollection<object>();
        private readonly IFileManager _fileManager;
        private readonly IRomManager _romManager;
        private readonly IMameManager _mameManager;
        private readonly IWindowManager _windowManager;
        private readonly IValidationService _validationService;

        public GameManager( IFileManager fileManager, IRomManager romManager,
            IMameManager mameManager, IWindowManager windowManager,
            IValidationService validationService )
        {
            this._fileManager = fileManager;
            this._romManager = romManager;
            this._mameManager = mameManager;
            this._windowManager = windowManager;
            this._validationService = validationService;

            this.AddMazeCommand = new MenuCommand(
                this.AddMaze,
                this.OneOrLessObjectsSelected );

            this.AddMazeCollectionCommand = new MenuCommand(
                this.AddMazeCollection,
                this.OneOrLessObjectsSelected );

            this.SaveCommand = new MenuCommand(
                _ => this._fileManager.Save( (IFileProperties) this._selected[ 0 ] ),
                this.OneObjectSelected );

            this.SaveAsCommand = new MenuCommand(
                _ => this._fileManager.Save( (IFileProperties) this._selected[ 0 ], true ),
                this.OneObjectSelected );

            this.SaveAllCommand = new MenuCommand(
                this.SaveAll,
                this.OneOrMoreObjectsSelected );

            this.OpenCommand = new MenuCommand( this.OpenFile );

            this.CloseCommand = new MenuCommand(
                this.Close,
                this.OneObjectSelected );

            this.LoadFromRomCommand = new MenuCommand(
                _ => this._romManager.LoadFrom() );

            this.DeleteCommand = new MenuCommand(
                this.Delete,
                this.OneOrMoreObjectsSelected );

            this.PreviewInHbMameCommand = new MenuCommand(
                this.PreviewInHbMame,
                this.OneMazeSelected );

            this.ValidateCommand = new MenuCommand(
                this.Validate,
                this.OneOrMoreObjectsSelected );
        }

        public IList GameObjects
        {
            get { return this._gameObjects; }
        }

        public IList SelectedObjects
        {
            get { return this._selected; }
        }

        public ICommand AddMazeCommand { get; }

        public ICommand AddMazeCollectionCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand SaveAsCommand { get; }

        public ICommand SaveAllCommand { get; }

        public ICommand OpenCommand { get; }

        public ICommand CloseCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand LoadFromRomCommand { get; }

        public ICommand PreviewInHbMameCommand { get; }

        public ICommand ValidateCommand { get; }

        /// <summary>
        /// Opens MazeUI to edit maze.
        /// </summary>
        public ICommand EditMazeCommand { get; }

        private bool OneOrLessObjectsSelected( object arg )
        {
            return this._selected.Count <= 1;
        }

        private bool OneObjectSelected( object arg )
        {
            return this._selected.Count == 1;
        }

        private bool OneOrMoreObjectsSelected( object arg )
        {
            return this._selected.Count >= 1;
        }

        private bool OneMazeSelected( object arg )
        {
            return this.OneObjectSelected( arg )
                   && this._selected[ 0 ] is Maze;
        }

        private void AddMaze( object notUsed )
        {
            /// nothing selected, add at top level
            if ( this._selected.Count == 0 )
            {
                this._gameObjects.Add( new Maze() );
            }

            /// MazeCollection selected, insert at end of collection.
            else if ( this._selected[ 0 ] is MazeCollection selectedMazeCollection )
            {
                // Name is generated on add event in the MazeCollection.
                selectedMazeCollection.Mazes.Add( new Maze( string.Empty ) );
            }
            else /// Maze selected
            {
                int index = this._gameObjects.IndexOf( this._selected[ 0 ] );

                /// Top level Maze, add after.
                if ( index >= 0 )
                {
                    this._gameObjects.Insert( index + 1, new Maze() );
                }

                // Find parent collection and insert after selected...
                else
                {
                    var found =
                        this._gameObjects
                            .OfType<MazeCollection>()
                            .First( mc =>
                                    {
                                        index = mc.Mazes.IndexOf( (Maze) this._selected[ 0 ] );

                                        return index >= 0;
                                    } );

                    found.Mazes.Insert( index + 1, new Maze() );
                }
            }
        }

        private void AddMazeCollection( object notUsed )
        {
            /// nothing selected, add at top level
            if ( this._selected.Count == 0 )
            {
                this._gameObjects.Add( new MazeCollection() );
            }
            else
            {
                int index = this._gameObjects.IndexOf( this._selected[ 0 ] );

                this._gameObjects.Insert(index + 1, new MazeCollection());
            }
        }

        private void SaveAll( object notUsed )
        {
            // On save all, write MazeCollections, and Loose Mazes that
            // that exist at top level of treeview. No need to go further
            // down as collections save child mazes.
            foreach ( object topLevelItem in this._gameObjects )
            {
                if ( topLevelItem is IChangeTracking changeTracking &&
                     changeTracking.IsChanged )
                {
                    /// This will prompt for SaveAs if necessary.
                    this._fileManager.Save( (IFileProperties) topLevelItem );
                }
            }
        }

        private void OpenFile( object notUsed )
        {
            OpenFileDialog ofd =
                new OpenFileDialog
                {
                    Title = "Open Maze or Maze Collection",
                    InitialDirectory =
                        Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ),
                    Filter =
                        "Editor Files (*.mhz;*.mhc)|*.mhz;*.mhc|Mazes (*.mhz)|*.mhz|Maze Collections (*.mhc)|*.mhc",
                    CheckFileExists = true,
                };

            if ( ofd.ShowDialog() == DialogResult.OK )
            {
                object opened = Path.GetExtension( ofd.FileName ).ToLowerInvariant() switch
                {
                    ".mhz" => this._fileManager.Open<Maze>( ofd.FileName ),
                    ".mhc" => this._fileManager.Open<MazeCollection>( ofd.FileName ),
                    _ => throw new ArgumentOutOfRangeException(
                             $"{Path.GetExtension( ofd.FileName )} is not a supported extension." )
                };

                //TODO: Insert after Parent.
                if ( opened != null )
                {
                    this._gameObjects.Add( opened );
                }
            }
        }

        /// <summary>
        /// This closes a singular... Not sure about logic here
        /// </summary>
        private void Close( object notUsed )
        {
            IFileProperties itemToClose = (IFileProperties) this._selected[ 0 ];

            bool safeToRemove = itemToClose is IChangeTracking changeTracking ?
                                    !changeTracking.IsChanged :
                                    true;

            if ( !safeToRemove )
            {
                // prompt user to save...
                DialogResult result = MessageBox.Show(
                    $"Save changes to {itemToClose.Name}?",
                    "Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation );

                if ( result == DialogResult.Yes )
                {
                    safeToRemove = this._fileManager.Save( itemToClose );
                }
                else
                {
                    /// Close without save!
                    safeToRemove = result == DialogResult.No;
                }
            }

            if ( safeToRemove )
            {
                if ( !this._gameObjects.Remove( itemToClose ) )
                {
                    /// If it wasn't removed at the top cast to Maze and
                    /// remove from it's parent collection.
                    this._gameObjects.OfType<MazeCollection>()
                        .All( mc => !mc.Mazes.Remove( (Maze) itemToClose ) );
                }
            }
        }

        /// <summary>
        /// TODO: Decide if should force conditions.. like all from same parent, or same level.
        /// </summary>
        private void Delete( object notUsed )
        {
            DialogResult result = MessageBox.Show(
                this._selected.Count == 1 ?
                    $"{this._selected.Cast<IName>().First().Name} will be deleted permanently!" :
                    $"All Selected nodes will be deleted permanently!",
                "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation );

            if ( result == DialogResult.OK )
            {
                // TODO: If Mazes had a "Parent" property this would be easier...
                // Sort into groups with MazeCollections first.
                List<IGrouping<Type, object>> deleteList =
                    this._selected
                        .ToLookup( o => o.GetType() )
                        .OrderBy( o => o.Key == typeof( MazeCollection ) )
                        .ToList();

                /// BUG: Should warn of unsaved changes??
                foreach ( IGrouping<Type, object> grouping in deleteList )
                {
                    if ( grouping.Key == typeof( MazeCollection ) )
                    {
                        foreach ( MazeCollection collectionToDelete in grouping )
                        {
                            foreach ( Maze maze in collectionToDelete.Mazes )
                            {
                                this._windowManager.Remove( maze );
                            }

                            // MazeCollections are always on top level.
                            this._gameObjects.Remove( collectionToDelete );
                        }

                        // move on to 2nd grouping (Mazes)
                        continue;
                    }

                    foreach ( Maze mazeToDelete in grouping )
                    {
                        IList copy = this._gameObjects.ToList();

                        foreach ( object gameObject in copy )
                        {
                            this._windowManager.Remove( mazeToDelete );

                            if ( gameObject.Equals( mazeToDelete ) )
                            {
                                // Top level maze
                                this._gameObjects.Remove( mazeToDelete );
                            }
                            else if ( gameObject is MazeCollection mazeCollection &&
                                      mazeCollection.Mazes.Contains( mazeToDelete ) )
                            {
                                mazeCollection.Mazes.Remove( mazeToDelete );
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// BUG: This assumes collection exits!!! This needs fixed!!
        /// </summary>
        /// <param name="obj"></param>
        private void PreviewInHbMame( object notUsed )
        {
            Maze maze = (Maze) this._selected[ 0 ];

            MazeCollection collection =
                this._gameObjects.OfType<MazeCollection>()
                    .FirstOrDefault( mc => mc.Mazes.Contains( maze ) );

            if ( collection != null )
            {
                this._mameManager.Preview( collection, maze );
            }

        }

        private void Validate( object notUsed )
        {
            foreach ( object subject in this._selected )
            {
                this._validationService.ValidateAndDisplayResults( subject );
            }
        }

    }

}
