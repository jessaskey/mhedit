using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using mhedit.Containers.MazeEnemies;
using mhedit.Containers.MazeObjects;
using MajorHavocEditor.Controls.Commands;

namespace MajorHavocEditor.Views
{

    public partial class MazeUi : UserControl, IUserInterface, INotifyPropertyChanged
    {
        private GameToolbox _gameToolbox = new GameToolbox();
        private MazeExplorer _mazeExplorer;
        private WindowManager _windowManager;
        private PropertyBrowser _propertyBrowser;
        private readonly IMenuManager _contextMenuManager;
        private Maze _maze;
        private readonly IList<MazeObject> _selectedMazeObjects =
            new ObservableCollection<MazeObject>();

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

            this.kryptonPanel.Controls.Add( wrapper );

            this.Dock = DockStyle.Fill;

            this.CutCommand = new DelegateCommand(
                    this.Cut,
                    this.OneOrMoreObjectsSelected )
                .ObservesProperty( () => this.SelectedMazeObjects.Count );

            this.CopyCommand = new DelegateCommand(
                    this.Copy,
                    this.OneOrMoreObjectsSelected)
                .ObservesProperty(() => this.SelectedMazeObjects.Count);

            this.PasteCommand = new DelegateCommand(
                    this.Paste,
                    this.ClipboardDataIsValid);

            this.DeleteCommand = new DelegateCommand(
                    this.Delete,
                    this.OneOrMoreObjectsSelected)
                .ObservesProperty(() => this.SelectedMazeObjects.Count);

            this._contextMenuManager = new MazeEditContextMenu( this );

            this._mazeExplorer =
                new MazeExplorer( maze, this._selectedMazeObjects, this._contextMenuManager );

            MazeController mc =
                new MazeController(maze, this._selectedMazeObjects, this._contextMenuManager)
                {
                    AutoSize = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Dock = DockStyle.None,
                };

            wrapper.Controls.Add( mc );

            this._windowManager = new WindowManager(this.kryptonDockingManager);

            this._propertyBrowser = new PropertyBrowser(this._mazeExplorer.SelectedObjects);
        }

        public Maze Maze
        {
            get { return this._maze; }
        }

        protected IList<MazeObject> SelectedMazeObjects
        {
            get { return this._selectedMazeObjects; }
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

#region Implementation of INotifyPropertyChanged

        /// <inheritdoc />

        // the only reason this is here, is so that DelegateCommand.ObservesProperty()
        // will function.
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }

#endregion

        private bool OneOrMoreObjectsSelected()
        {
            return this.SelectedMazeObjects.Count >= 1;
        }

        private bool ClipboardDataIsValid()
        {
            return Clipboard.ContainsData( ClipboardFormatId );
        }

        public ICommand CutCommand { get; set; }

        public ICommand CopyCommand { get; set; }

        public ICommand PasteCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        private void Cut()
        {
            this.RemoveObjectsFromMaze( this.CopySelectedToClipboard() );
        }

        private void Copy()
        {
            CopySelectedToClipboard();
        }

        private void Paste()
        {
            if ( Clipboard.ContainsData( ClipboardFormatId ) )
            {
                IEnumerable< MazeObject> mazeObjectsToAdd =
                    Clipboard.GetData( ClipboardFormatId ) as IEnumerable<MazeObject>;

                /// Always clear any selection before paste.
                this.SelectedMazeObjects.Clear();

                /// TODO: Deal with adding to many of any given object.
                //bool promptUser = true;

                //_maze.MazeObjects.OrderBy(o => o.GetType() == typeof(MazeWall)).
                //      ThenBy(o => o.GetType().Name).ToList().ConvertAll(m => (IName)m) );

                foreach ( MazeObject mazeObject in mazeObjectsToAdd)
                {
                    //if (this._maze.GetObjectTypeCount(mazeObject.GetType()) >= mazeObject.MaxObjects)
                    //{
                    //    MessageBox.Show($"You can't add any more {mazeObject.GetType().Name} objects.",
                    //        "The Homeworld is near", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //}

                    /// Clear the name and accept changes as the new action here is
                    /// simply adding the object..
                    /// TODO: Make overwrite of name an option?
                    //mazeObject.Name = string.Empty;

                    mazeObject.AcceptChanges();

                    this._maze.MazeObjects.Add( mazeObject );

                    /// MazeObject.Selected was true when it was Cut but it still needs added to
                    /// the selected collection.
                    this._selectedMazeObjects.Add( mazeObject );

                    if ( mazeObject is TripPad tripPad )
                    {
                        this._maze.MazeObjects.Add( tripPad.Pyroid );
                    }
                }
            }
        }

        private void Delete()
        {
            IList<MazeObject> toDelete = this.SelectedMazeObjects;

            string plural = toDelete.Count == 1 ?
                                toDelete[ 0 ].Name :
                                $"all {toDelete.Count} objects";

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete {plural}?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question );

            if ( result == DialogResult.Yes )
            {
                this.RemoveObjectsFromMaze( this.RemoveTripPadPyroids( toDelete ) );
            }
        }

        private static string ClipboardFormatId = "MazeObjectFormat";

        private IEnumerable<MazeObject> CopySelectedToClipboard()
        {
            IEnumerable<MazeObject> noTripPyroids =
                this.RemoveTripPadPyroids( this.SelectedMazeObjects );

            if ( noTripPyroids.Any() )
            {
                /// Wrap in DataObject so that this data is cleared from clipboard on exit.
                Clipboard.SetDataObject( new DataObject( ClipboardFormatId, noTripPyroids), false );
            }

            return noTripPyroids;
        }

        private IEnumerable<MazeObject> RemoveTripPadPyroids( IEnumerable<MazeObject> mazeObjects )
        {
            IEnumerable<MazeObject> tripPyroids = mazeObjects.OfType<TripPadPyroid>();

            if ( tripPyroids.Any() )
            {
                MessageBox.Show(
                    "Cut/Copy/Paste operations don't support individual TripPadPyroids. Select parent TripPad to perform this action." +
                    $"{Environment.NewLine}{Environment.NewLine}The TripPadPyroids will be removed from the operation.",
                    "Selection includes TripPadPyroid",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
            }

            // return new collection to avoid collection manipulation issues.
            return mazeObjects.Except( tripPyroids ).ToList();
        }

        private void RemoveObjectsFromMaze( IEnumerable<MazeObject> objectsToRemove )
        {
            foreach ( MazeObject mazeObject in objectsToRemove )
            {
                if ( mazeObject is TripPad tripPad )
                {
                    this._maze.MazeObjects.Remove( tripPad.Pyroid );
                }

                this._maze.MazeObjects.Remove( mazeObject );
            }
        }
    }

    class MazeEditContextMenu : ContextMenuManager
    {
        private readonly DelegateCommandBase _pasteCommand;

        public MazeEditContextMenu( MazeUi ui )
        {
            this._pasteCommand = ui.PasteCommand as DelegateCommandBase;

            Guid ccpGroup = Guid.NewGuid();

            IMenuItem cutItem =
                new MenuItem("MazeUi_Cut")
                {
                    GroupKey = ccpGroup,
                    Command = ui.CutCommand,
                    ShortcutKey = Keys.Control | Keys.X,
                    Display = "Cut",
                    Icon = @"Resources\Images\Menu\Cut_6523_24.bmp".CreateResourceUri()
                };
            this.Add( cutItem );

            this.Add(
                new MenuItem("MazeUi_Copy")
                {
                    GroupKey = ccpGroup,
                    Command = ui.CopyCommand,
                    ShortcutKey = Keys.Control | Keys.C,
                    Display = "Copy",
                    Icon = @"Resources\Images\Menu\Copy_6524_24.bmp".CreateResourceUri()
                });
            this.Add(
                new MenuItem("MazeUi_Paste")
                {
                    GroupKey = ccpGroup,
                    Command = ui.PasteCommand,
                    ShortcutKey = Keys.Control | Keys.V,
                    Display = "Paste",
                    Icon = @"Resources\Images\Menu\Paste_6520_24.bmp".CreateResourceUri()
                });

            this.Add(new Separator( cutItem, ccpGroup));

            this.Add(
                new MenuItem("MazeUi_Delete")
                {
                    GroupKey = Guid.NewGuid(),
                    Command = ui.DeleteCommand,
                    ShortcutKey = Keys.Delete,
                    Display = "Delete",
                    Icon = @"Resources\Images\Menu\delete.ico".CreateResourceUri()
                });

            ( (ContextMenuStrip) this.ToolStrip ).Opening += this.OnContextMenuOpening;
        }

        private void OnContextMenuOpening( object sender, CancelEventArgs e )
        {
            this._pasteCommand.RaiseCanExecuteChanged();
        }
    }

}
