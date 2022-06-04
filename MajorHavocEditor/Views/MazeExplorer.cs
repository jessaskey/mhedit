using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using mhedit.Containers;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{
    public partial class MazeExplorer : UserControl, IUserInterface
    {
        private class ItemsSourceDelegate : IItemsSourceDelegate
        {
            /// <summary>
            /// I hate WinForms... This class is needed to properly disconnect
            /// the INotifyPropertyChanged.PropertyChanged event from the node
            /// and prevent memory leaks.
            /// </summary>
            private class BoundTreeNode : TreeNode
            {
                public BoundTreeNode( string text )
                    : base( text )
                {
                }

                public TreeNode ConnectPropertyChanged( INotifyPropertyChanged inpc )
                {
                    if ( inpc != null )
                    {
                        inpc.PropertyChanged += this.OnPropertyChanged;
                    }

                    return this;
                }

                public void DisconnectPropertyChanged( INotifyPropertyChanged inpc )
                {
                    inpc.PropertyChanged += this.OnPropertyChanged;
                }

                private void OnPropertyChanged( object sender, PropertyChangedEventArgs args )
                {
                    if ( args.PropertyName.Equals( nameof( IName.Name ) ) )
                    {
                        this.Text = ( (IName) sender ).Name;
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

#region Implementation of IItemsSourceDelegate

            /// <inheritdoc />
            public TreeNode CreateNode( object item )
            {
                int imageIndex = 5;

                return new BoundTreeNode( ( (IName) item ).Name )
                       {
                           Tag = item,
                           ImageIndex = imageIndex,
                           SelectedImageIndex = imageIndex,
                       }
                    .ConnectPropertyChanged( item as INotifyPropertyChanged) ;
            }

            /// <inheritdoc />
            public void OnRemoveNode( TreeNode node )
            {
                ( (BoundTreeNode) node ).DisconnectPropertyChanged(
                    (INotifyPropertyChanged) node.Tag );
            }

            /// <inheritdoc />
            public bool Equals( TreeNode node, object item )
            {
                return ReferenceEquals( node.Tag, item );
            }

            /// <inheritdoc />
            public IEnumerable GetEnumerable( TreeNode item )
            {
                if ( item.Tag is Maze maze )
                {
                    return maze.MazeObjects
                               .ToLookup( o => o.GetType() )
                               .OrderBy( o => o.Key == typeof( MazeWall ) )
                               .ThenBy( o => o.Key.Name )
                               .Select( g => new NamedGrouping(g) )
                               .ToList();
                }
                else if ( item.Tag is IEnumerable enumerable )
                {
                    return enumerable;
                }

                return null;
            }

#endregion
        }

        private class NamedGrouping : IGrouping<Type, MazeObject>, IName
        {
            private readonly IGrouping<Type, MazeObject> _wrapped;

            public NamedGrouping( IGrouping<Type, MazeObject> wrapped )
            {
                this._wrapped = wrapped;
            }

#region Implementation of IEnumerable

            /// <inheritdoc />
            public IEnumerator<MazeObject> GetEnumerator()
            {
                return this._wrapped.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

#endregion

#region Implementation of IGrouping<out Type,out MazeObject>

            /// <inheritdoc />
            public Type Key
            {
                get { return this._wrapped.Key; }
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

        //public MazeExplorer()
        //    : this(new ObservableCollection<MazeObject>(),
        //        new ObservableCollection<MazeObject>())
        //{ }

        public MazeExplorer( Maze maze, IList<MazeObject> selectedItems )
        {
            this.InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );


            this.treeView.ItemsDelegate = new ItemsSourceDelegate();
            this.treeView.ItemsSource.Add(maze);
            this.treeView.SelectedItems = new SelectedItemsModerator( selectedItems );

            this.treeView.ContextMenuStrip = (ContextMenuStrip)this._contextMenuManager.Menu;

            this.treeView.ImageList = IconList;

            //this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
            //this.treeView.BeforeSelect += this.OnTreeViewBeforeSelect;

            this._mazeObjects = maze.MazeObjects;

            this._selectedObjects = selectedItems;

            //this.ConstructObjectView(mazeObjects);

            // Force the TreeView to update to the existing values in the maze
            //this.OnMazeObjectsCollectionChanged( this._mazeObjects,
            //    new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ));

            //if (maze.MazeObjects is INotifyCollectionChanged incc)
            //{
            //    incc.CollectionChanged += this.OnMazeObjectsCollectionChanged;
            //}

            //if (selectedItems is INotifyCollectionChanged incc2)
            //{
            //    incc2.CollectionChanged += this.OnSelectedObjectsCollectionChanged;
            //}
        }

        //private void OnTreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
        //{
        //    if (this.treeView.SelectedNode?.Tag is MazeObject mazeObject)
        //    {
        //        mazeObject.Selected = false;
        //    }

        //    if (!ModifierKeys.HasFlag(Keys.Control))
        //    {
        //        foreach (TreeNode selectedNode in this._selectedNodes)
        //        {
        //            selectedNode.Checked = false;
        //            e.Node.BackColor = Color.Empty;
        //        }
        //    }
        //}

        //private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    if (this.treeView.SelectedNode?.Tag is MazeObject mazeObject)
        //    {
        //        mazeObject.Selected = true;

        //        e.Node.Checked = true;
        //        this._selectedNodes.Add(e.Node);

        //        if (ModifierKeys.HasFlag(Keys.Control))
        //        {
        //            e.Node.BackColor = Color.Aqua;
        //        }
        //    }
        //}

        private void ConstructObjectView( ICollection<MazeObject> mazeObjects )
        {
            try
            {
                // Sort all objects and put them under a parent...
                if ( mazeObjects != null )
                {
                    var sorted = mazeObjects
                                 .ToLookup( o => o.GetType() )
                                 .OrderBy( o => o.Key == typeof( MazeWall ) )
                                 .ThenBy( o => o.Key.Name )
                                 .ToList();

                    this.treeView.BeginUpdate();

                    foreach ( IGrouping<Type, MazeObject> grouping in sorted )
                    {
                        TreeNode groupNode = new KryptonTreeNode(
                            grouping.Key.Name,
                            grouping.Select(o => new KryptonTreeNode( o.Name ) { Tag = o, Name = o.Name} )
                                    .Cast<TreeNode>()
                                    .ToArray() )
                            {
                                Name = grouping.Key.Name,
                                ImageKey = grouping.Key.Name,
                                SelectedImageKey = grouping.Key.Name
                            };

                        this.treeView.Nodes.Add( groupNode );
                    }

                    this.treeView.CollapseAll();
                }
            }
            finally
            {
                this.treeView.EndUpdate();
            }
        }

        //private void OnSelectedObjectsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Reset)
        //    {
        //    }
        //    else if ( e.Action == NotifyCollectionChangedAction.Add )
        //    {
        //    }
        //    else if ( e.Action == NotifyCollectionChangedAction.Remove )
        //    {
        //    }
        //}

        private void OnMazeObjectsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Rebuild the tree view
                this.treeView.Nodes.Clear();

                this.ConstructObjectView( this._mazeObjects );
            }
            else if (e.Action == NotifyCollectionChangedAction.Add )
            {
                var sorted = e.NewItems
                              .Cast<MazeObject>()
                              .ToLookup( o => o.GetType() )
                              .OrderBy(o => o.Key.Name)
                              .ToList();

                int startingGroupCount = this.treeView.Nodes.Count;

                try
                {
                    this.treeView.BeginUpdate();
                    
                    foreach (IGrouping<Type, MazeObject> grouping in sorted)
                    {
                        TreeNode groupNode = this.treeView.Nodes[ grouping.Key.Name ] ??
                                          new KryptonTreeNode( grouping.Key.Name )
                                          {
                                              //Name = grouping.Key.Name,
                                              ImageKey = grouping.Key.Name,
                                              SelectedImageKey = grouping.Key.Name
                                          };

                        groupNode.Nodes.AddRange( 
                            grouping.Select(o => new KryptonTreeNode( o.Name ){ Tag = o, Name = o.Name } )
                                    .Cast<TreeNode>()
                                    .ToArray() );

                        if (string.IsNullOrEmpty( groupNode.Name ))
                        {
                            groupNode.Name = grouping.Key.Name;

                            this.treeView.Nodes.Add(groupNode);
                        }
                    }

                    if ( this.treeView.Nodes.Count != startingGroupCount )
                    {
                        TreeNode[] nodes = this.treeView.Nodes
                                               .Cast<TreeNode>()
                                               .OrderBy(n => n.Name == typeof(MazeWall).Name)
                                               .ThenBy(n => n.Name)
                                               .ToArray();

                        this.treeView.Nodes.Clear();
                        this.treeView.Nodes.AddRange(nodes);
                    }

                    // scroll to last added?
                    TreeNode lastNode = this.treeView.Nodes[sorted.Last().Key.Name].LastNode;
                    if (!lastNode.IsVisible)
                    {
                        // must end update before EnsureVisible 
                        this.treeView.EndUpdate();
                        lastNode.EnsureVisible();
                    }
                }
                finally
                {
                    this.treeView.EndUpdate();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove )
            {
                var sorted = e.OldItems
                              .Cast<MazeObject>()
                              .ToLookup(o => o.GetType())
                              .OrderBy(o => o.Key.Name)
                              .ToList();

                try
                {
                    this.treeView.BeginUpdate();

                    foreach (IGrouping<Type, MazeObject> grouping in sorted)
                    {
                        TreeNode groupNode = this.treeView.Nodes[ grouping.Key.Name ];

                        // This should never be null!!
                        if ( groupNode != null )
                        {
                            foreach ( MazeObject toRemove in grouping )
                            {
                                groupNode.Nodes.RemoveByKey( toRemove.Name );
                            }

                            if ( groupNode.Nodes.Count == 0 )
                            {
                                groupNode.Remove();
                            }
                        }
                    }
                }
                finally
                {
                    this.treeView.EndUpdate();
                }
            }

            this.Invalidate();
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
