using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{
    public partial class MazeExplorer : UserControl, IUserInterface
    {
        /// <summary>
        /// I hate WinForms... This class is needed to properly disconnect
        /// the INotifyPropertyChanged.PropertyChanged event from the node
        /// and prevent memory leaks.
        /// </summary>
        private class BoundTreeNode : TreeNode
        {
            public BoundTreeNode(string text)
                : base(text)
            {
            }

            public TreeNode ConnectPropertyChanged(INotifyPropertyChanged inpc)
            {
                if (inpc != null)
                {
                    inpc.PropertyChanged += this.OnPropertyChanged;
                }

                return this;
            }

            public void DisconnectPropertyChanged(INotifyPropertyChanged inpc)
            {
                if (inpc != null)
                {
                    inpc.PropertyChanged -= this.OnPropertyChanged;
                }
            }

            private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName.Equals(nameof(IName.Name)))
                {
                    this.Text = ((IName)sender).Name;
                }
                //else if ( args.PropertyName.Equals( nameof( Maze.MazeType ) ) )
                //{
                //    this.ImageIndex = this.Tag is Maze maze ? (int) maze.MazeType + 1 : 0;
                //    this.SelectedImageIndex = this.ImageIndex;
                //}
                else if (args.PropertyName.Equals(nameof(MazeObject.Selected)))
                {
                    this.Checked = ((MazeObject)this.Tag).Selected;
                }
            }
        }

        private class NamedGrouping : ObservableCollection<MazeObject>, IGrouping<Type, MazeObject>, IName
        {
            private readonly Type _key;

            public NamedGrouping( IGrouping<Type, MazeObject> source )
                :base(source)
            {
                this._key = source.Key;
            }

#region Implementation of IGrouping<out Type,out MazeObject>

            /// <inheritdoc />
            public Type Key
            {
                get { return this._key; }
            }

#endregion

#region Implementation of IName

            /// <inheritdoc />
            public string Name
            {
                get { return this.Key.Name; }
                set {  }
            }

#endregion
        }

        private static readonly ImageList IconList;

        private readonly IMenuManager _contextMenuManager = new ContextMenuManager();

        private readonly IList<MazeObject> _mazeObjects;
        private readonly IList<MazeObject> _selectedObjects;

        static MazeExplorer()
        {
            IconList =
                new ImageList { TransparentColor = Color.Fuchsia }
                    .AddImages(new[]
                               {
                                   "arrow_32.png",
                                   "arrow_out_32.png",
                                   "booties_32.png",
                                   "cannon_32.png",
                                   "clock_32.png",
                                   "hand_32.png",
                                   "keypouch_32.png",
                                   "key_32.png",
                                   "lightning_32.png",
                                   "lightning_v_32.png",
                                   "lock_32.png",
                                   "oneway_32.png",
                                   "oxoid_32.png",
                                   "perkoid_32.png",
                                   "pod_32.png",
                                   "pyroid_32.png",
                                   "reactoid_32.png",
                                   "roboid_32.png",
                                   "spikes_32.png",
                                   "token_32.png",
                                   "transporter_32.png",
                                   "trippad_32.png",
                                   "wall_empty_32.png",
                                   "wall_horizontal_32.png",
                                   "wall_leftdown_32.png",
                                   "wall_leftup_32.png",
                                   "wall_rightdown_32.png",
                                   "wall_rightup_32.png",
                                   "wall_vertical_32.png",
                               })
                    .WithResourcePath("Resources/Images/Toolbox")
                    .Load();
        }

        public MazeExplorer( Maze maze, IList<MazeObject> selectedItems )
        {
            this.InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            MazeExplorerItemsSource source = new MazeExplorerItemsSource( maze );
            this.treeView.ItemsDelegate = source;
            this.treeView.ItemsSource = source;
            this.treeView.SelectedItems = new SelectedItemsModerator( selectedItems );

            this.treeView.BeforeLabelEdit += this.OnBeforeLabelEdit;

            this.treeView.ContextMenuStrip = (ContextMenuStrip)this._contextMenuManager.Menu;

            this.treeView.ImageList = IconList;

            this._mazeObjects = maze.MazeObjects;

            this._selectedObjects = selectedItems;
        }

        private void OnBeforeLabelEdit( object sender, NodeLabelEditEventArgs e )
        {
            // Don't allow edits of the Group nodes..
            e.CancelEdit = e.Node.Tag is IGrouping<Type, MazeObject>;
        }

        {
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
                return DockingPosition.Left |
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
            get { return "Maze Explorer"; }
        }

        public object Icon
        {
            get { return null; }
        }

#endregion
    }
}
