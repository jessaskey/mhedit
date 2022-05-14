using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Krypton.Toolkit;
using mhedit.Containers;
using MajorHavocEditor.Interfaces.Ui;

namespace MHavocEditor.Views
{
    public partial class MazeExplorer : UserControl, IUserInterface
    {
        private readonly ICollection<MazeObject> _mazeObjects;
        private readonly ICollection<MazeObject> _selectedObjects;

        public MazeExplorer()
            : this(new ObservableCollection<MazeObject>(),
                new ObservableCollection<MazeObject>())
        { }

        public MazeExplorer( ICollection<MazeObject> mazeObjects,
            ICollection<MazeObject> selectedObjects )
        {
            this._mazeObjects = mazeObjects;

            this._selectedObjects = selectedObjects;

            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size( 200, 200 );

            //this.treeView.ImageList = new ImageList(this.components)
            //                          {
            //                              TransparentColor = Color.Fuchsia,
            //                          };

            //this.treeView.ImageList.Images.Add(this.LoadImage("ThumbnailViewHS.bmp"));
            //this.treeView.ImageList.Images.Add(this.LoadImage("maze_a.bmp"));
            //this.treeView.ImageList.Images.Add(this.LoadImage("maze_b.bmp"));
            //this.treeView.ImageList.Images.Add(this.LoadImage("maze_c.bmp"));
            //this.treeView.ImageList.Images.Add(this.LoadImage("maze_d.bmp"));

            this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
            this.treeView.BeforeSelect += this.OnTreeViewBeforeSelect;

            this.ConstructObjectView(mazeObjects);

            if (mazeObjects is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += this.OnMazeObjectsCollectionChanged;
            }

            if (selectedObjects is INotifyCollectionChanged incc2)
            {
                incc2.CollectionChanged += this.OnSelectedObjectsCollectionChanged;
            }
        }

        private void OnTreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if ( this.treeView.SelectedNode?.Tag is MazeObject mazeObject )
            {
                mazeObject.Selected = false;
            }

            if (!ModifierKeys.HasFlag(Keys.Control))
            {
                //foreach (TreeNode selectedNode in this._selectedNodes)
                //{
                //    selectedNode.Checked = false;
                //    e.Node.BackColor = Color.Empty;
                //}
            }
        }

        private void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView.SelectedNode?.Tag is MazeObject mazeObject)
            {
                mazeObject.Selected = true;

                e.Node.Checked = true;
                //this._selectedNodes.Add(e.Node);

                //if (ModifierKeys.HasFlag(Keys.Control))
                //{
                //    e.Node.BackColor = Color.Aqua;
                //}
            }
        }

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

                    treeView.BeginUpdate();

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
                treeView.EndUpdate();
            }
        }

        private void OnSelectedObjectsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
            }
            else if ( e.Action == NotifyCollectionChangedAction.Add )
            {
            }
            else if ( e.Action == NotifyCollectionChangedAction.Remove )
            {
            }
        }

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
