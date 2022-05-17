using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows.Forms;
using Krypton.Toolkit;
using mhedit;
using mhedit.Containers;
using mhedit.Containers.Validation.MajorHavoc;
using MajorHavocEditor.Controls.Menu;
using MajorHavocEditor.Interfaces.Ui;

namespace MajorHavocEditor.Views
{

    public static class TreeNodeExtensions
    {
        /// <summary>
        /// This is enough to look through 2 levels of node hierarchy. 
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static TreeNode FindNodeOrDefault(this TreeView tree, object itemId)
        {
            var result = tree.Nodes.Cast<TreeNode>()
                             .FirstOrDefault(node => node.Tag.Equals(itemId));

            return result ?? tree.Nodes.Cast<TreeNode>()
                                 .SelectMany( n => n.Nodes.Cast<TreeNode>() )
                                 .FirstOrDefault(node => node.Tag.Equals(itemId));
        }
    }

    public partial class GameExplorer : UserControl, IUserInterface
    {
        internal interface ISelectedNodes : IEnumerable<TreeNode>
        {
            int Count { get; }
            void Add(TreeNode node);
            void Clear();
            bool Contains(TreeNode node);
            bool Remove(TreeNode node);
        }

        public delegate bool TryFindNode(object key, out TreeNode found);

        public class SelectedItems : ISelectedNodes //List<TreeNode>
        {

            //private readonly Dictionary<object, TreeNode> _lookup =
            //    new Dictionary<object, TreeNode>();
            public readonly IList _items;
            private readonly TryFindNode _tryFindNode;
            private readonly List<TreeNode> _nodes = new List<TreeNode>();

            public SelectedItems( TryFindNode tryFindNode )
                    : this(new ObservableCollection<object>(), tryFindNode)
            { }

            public SelectedItems(IList items, TryFindNode tryFindNode )
            {
                this._items = items;
                this._tryFindNode = tryFindNode;

                if (items is INotifyCollectionChanged incc)
                {
                    incc.CollectionChanged += this.OnItemsCollectionChanged;
                }
            }

            private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    foreach ( TreeNode node in this._nodes )
                    {
                        node.Checked = false;
                    }

                    this._nodes.Clear();
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (object newItem in e.NewItems)
                    {
                        if (this._tryFindNode(newItem, out TreeNode node))
                        {
                            node.Checked = true;

                            this._nodes.Add( node );
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach ( object oldItem in e.OldItems )
                    {
                        if ( this._tryFindNode( oldItem, out TreeNode node ) )
                        {
                            node.Checked = false;

                            this._nodes.Remove( node );
                        }
                    }
                }
            }

#region Implementation of ISelectedNodes

            /// <inheritdoc />
            int ISelectedNodes.Count
            {
                get { return this._nodes.Count; }
            }

            /// <inheritdoc />
            void ISelectedNodes.Add(TreeNode node)
            {
                //node.Checked = true;

                this._items.Add(node.Tag);
            }

            /// <inheritdoc />
            void ISelectedNodes.Clear()
            {
                this._items.Clear();
            }

            /// <inheritdoc />
            bool ISelectedNodes.Contains(TreeNode node)
            {
                return this._nodes.Contains( node );
            }

            /// <inheritdoc />
            bool ISelectedNodes.Remove(TreeNode node)
            {
                //node.Checked = false;

                this._items.Remove(node.Tag);

                return true;
            }

            /// <inheritdoc />
            IEnumerator<TreeNode> IEnumerable<TreeNode>.GetEnumerator()
            {
                return this._nodes.GetEnumerator();
            }

#endregion

#region Implementation of IEnumerable

            /// <inheritdoc />
            public IEnumerator GetEnumerator()
            {
                return this._items.GetEnumerator();
            }

#endregion
        }





        private readonly IWindowManager _windowManager;
        private readonly ISelectedNodes _selectedNodes;

        private static readonly string ResourcePath =
            $"/{typeof(GameExplorer).Assembly.GetName().Name};component/Resources/Images/";

        private TreeNode _mouseDownMultiSelectNode;
        private TreeNode _mouseDownClickedNode;
        //private bool _cancelLabelEdit;

        public GameExplorer()
            :this(null, null)
        { }

        public GameExplorer( IMenuManager menuManager, IWindowManager windowManager )
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;

            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            this.MinimumSize = new Size(200, 200);

            this._windowManager = windowManager;

            this.treeView.ImageList = new ImageList( this.components )
                                      {
                                          TransparentColor = Color.Fuchsia,
                                      };

            this.treeView.ImageList.Images.Add(this.LoadImage("ThumbnailViewHS.bmp"));
            this.treeView.ImageList.Images.Add(this.LoadImage("maze_a.bmp"));
            this.treeView.ImageList.Images.Add(this.LoadImage("maze_b.bmp"));
            this.treeView.ImageList.Images.Add(this.LoadImage("maze_c.bmp"));
            this.treeView.ImageList.Images.Add(this.LoadImage("maze_d.bmp"));

            //this.treeView.MouseClick += this.OnTreeViewMouseClick;
            this.treeView.AfterSelect += this.OnTreeViewAfterSelect;
            this.treeView.BeforeSelect += this.OnTreeViewBeforeSelect;
            this.treeView.MouseDoubleClick += this.OnTreeViewMouseDoubleClick;

            this.treeView.MouseDown += this.OnMouseDown;
            this.treeView.MouseUp += this.OnMouseUp;

            this.treeView.LabelEdit = true;
            this.treeView.BeforeLabelEdit += this.OnBeforeLabelEdit;
            this.treeView.AfterLabelEdit += this.OnAfterLabelEdit;

            //this.treeView.TreeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            //this.treeView.TreeView.DrawNode += this.OnDrawNode;
            this.treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView.DrawNode += this.OnDrawNode;
            //this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            //this.treeView.CheckBoxes = true;
            this.treeView.ShowLines = false;
            //this.treeView.ShowPlusMinus = false;

            this._selectedNodes = new SelectedItems(this.SearchNodesForTag);

            menuManager?.Add(
                new MenuItem( "LoadFromROM" )
                {
                    Command = new MenuCommand( this.OnLoadFromRomCommand ),
                    Display = "Load From ROM",
                    Icon = PackUriHelper.Create( ResourceLoader.ApplicationUri,
                        new Uri( $"{ResourcePath}Buttons/rom_32.png", UriKind.Relative ) )
                } );

            //if ( this._selectedNodes is INotifyCollectionChanged incc )
            //{
            //    incc.CollectionChanged += this.OnSelectedNodesChanged;
            //}
        }

        private bool SearchNodesForTag( object key, out TreeNode found )
        {
            found = this.treeView.FindNodeOrDefault( key );

            return found != null;
        }

        //private void OnSelectedNodesChanged( object sender, NotifyCollectionChangedEventArgs e )
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Reset)
        //    {
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //    }
        //}

        private void OnBeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if ((ModifierKeys & (Keys.Control | Keys.Shift)) > 0)
            {
                e.CancelEdit = true;
            }
        }

        private void OnAfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.nodelabelediteventargs?view=netframework-4.7.2
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {
                    if (e.Label.Length <= 50)
                    {
                        if (e.Label.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                        {
                            // Stop editing without canceling the label change.
                            e.Node.EndEdit(false);

                            if (e.Node.Tag is IName iName)
                            {
                                iName.Name = e.Label;
                            }
                        }
                        else
                        {
                            /* Cancel the label edit action, inform the user, and 
                               place the node in edit mode again. */
                            e.CancelEdit = true;

                            string invalid = new string(Path.GetInvalidFileNameChars()
                                                         .Where(c => !char.IsControl(c))
                                                         .ToArray());

                            MessageBox.Show($"The name \"{e.Label}\" contains invalid characters: {invalid}",
                                "Invalid Name");

                            e.Node.BeginEdit();
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Maze Name cannot be longer than 50 characters.",
                           "Node Label Edit");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and 
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("Invalid tree node label.\nThe label cannot be blank",
                       "Node Label Edit");
                    e.Node.BeginEdit();
                }
            }
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.drawnode?redirectedfrom=MSDN&view=netframework-4.7.2
        /// TODO: Move Editing/ChangeTracking outside of the Treeview? 
        /// </summary>
        private void OnEditDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // Use the default background and node text.
            e.DrawDefault = !e.Node.IsEditing;

            if (e.Node.IsEditing)
            {
                // While editing the Node Text don't paint the existing name behind.
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), e.Bounds);

                return;
            }

            // Extract the set font/color from the tree.
            Font nodeFont =
                new Font(e.Node.NodeFont ?? e.Node.TreeView.Font, FontStyle.Bold);

            // If a node tag is present, draw the IChangeTracking info if necessary.
            if ( e.Node.Tag is IChangeTracking ict )
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    ict.IsChanged ? "*" : "",
                    nodeFont,
                    new Point( e.Node.Bounds.Right + 4, e.Node.Bounds.Top ),
                    ( e.State & TreeNodeStates.Selected ) != 0 ? SystemColors.HighlightText :
                        SystemColors.ControlText,
                    TextFormatFlags.Left );
            }
        }

        protected void OnDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Rectangle adjustedBounds = new Rectangle(e.Bounds.X, e.Bounds.Y,
                this.treeView.Bounds.Width, e.Bounds.Height);

            // Retrieve the node font. If the node font has not been set,
            // use the TreeView font.
            Font nodeFont = e.Node.NodeFont ?? ((TreeView)this.treeView).Font;

            if (e.Node.IsEditing)
            {
                // While editing the Node Text don't paint the existing name behind.
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), adjustedBounds);
            }
            //Debug.WriteLine($"Node {e.Node.Text} {e.State}");
            // Draw the background and node text for a selected node.
            else if ((e.State & (TreeNodeStates.Selected | TreeNodeStates.Focused)) != 0 ||
                     e.Node.Checked )
            {
                //Debug.WriteLine( $"Node {e.Node.Text} {e.State}" );

                /// There's something weird going on here. The bounds provided by the event are different
                /// than those that the node has, AND if you use the given bounds and try to draw lines
                /// around the outside to show it's selected... it doesn't show the line on the bottom.
                /// Almost like the nodes are overlapped?!? Anyway, make the rectangle smaller and use that
                /// to get a decent selected node border that matches the stock TreeView control.
                //Rectangle adjustedBounds = new Rectangle(e.Bounds.X, e.Bounds.Y,
                //    e.Bounds.Width - 1, e.Bounds.Height - 1);
                //Rectangle adjustedBounds = new Rectangle(e.Bounds.X, e.Bounds.Y,
                //    this.treeView.Bounds.Width - 5, e.Bounds.Height - 1);


                // Draw the background of the selected node. The NodeBounds
                // method makes the highlight rectangle large enough to
                // include the text of a node tag, if one is present.
                //e.Graphics.FillRectangle(Brushes.Green, NodeBounds(e.Node));
                //TODO: Don't paint in background when Focused.
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), adjustedBounds);

                // Draw the node text.
                TextRenderer.DrawText(
                    e.Graphics, e.Node.Text, nodeFont,
                    adjustedBounds, SystemColors.HighlightText, TextFormatFlags.Left );
                //e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White,
                //    Rectangle.Inflate(e.Bounds, 2, 0));
            }

            // Use the default background and node text.
            else
            {
                e.DrawDefault = true;
            }

            // If a node tag is present, draw its string representation 
            // to the right of the label text.
            if (e.Node.Tag is IChangeTracking ict )
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    ict.IsChanged ? "*" : "",
                    nodeFont,
                    new Point(e.Node.Bounds.Right + 4, e.Node.Bounds.Top),
                    (e.State & TreeNodeStates.Selected) != 0 ? SystemColors.HighlightText : SystemColors.ControlText,
                    TextFormatFlags.Left);

                //e.Graphics.DrawString(
                //    ict.IsChanged ? ChangeTrackingBase.ModifiedBullet : "",
                //    nodeFont, Brushes.Blue, e.Node.Bounds.Right + 4, e.Node.Bounds.Top);


                //e.Graphics.DrawString(e.Node.Tag.ToString(), this.Font,
                //    Brushes.Blue, e.Bounds.Right + 2, e.Bounds.Top);
            }

            // If the node has focus, draw the focus rectangle large, making
            // it large enough to include the text of the node tag, if present.
            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = adjustedBounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                        focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }
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

        // Selects a node that is clicked on its label or tag text.
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            TreeNode clickedNode = this.treeView.GetNodeAt(e.X, e.Y);

            if ( clickedNode == null )
            {
                return;
            }

            this._mouseDownMultiSelectNode = clickedNode.Checked &&
                                          this._selectedNodes.Count > 1 &&
                                          ModifierKeys.HasFlag( Keys.Control ) &&
                                          clickedNode.Bounds.Contains( e.X, e.Y ) ?
                                              clickedNode : null;

            this._mouseDownClickedNode = this._selectedNodes.Count > 1 &&
                                         clickedNode == this.treeView.SelectedNode ?
                                             clickedNode : null;

            //TreeNode clickedNode = this.treeView.GetNodeAt(e.X, e.Y);
            //if (clickedNode != null && clickedNode.Bounds.Contains(e.X, e.Y))
            //{
            //    this._mouseDownMultiSelectNode = clickedNode;
            //}
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if ( this._mouseDownMultiSelectNode != null )
            {
                //TreeNode clickedNode = this.treeView.GetNodeAt(e.X, e.Y);

                if ( ModifierKeys.HasFlag( Keys.Control ) &&
                     this._mouseDownMultiSelectNode.Bounds.Contains( e.X, e.Y ) )
                {
                    this._mouseDownMultiSelectNode.Checked = false;

                    this._selectedNodes.Remove(this._mouseDownMultiSelectNode);
                    Debug.WriteLine($"Removed {this._mouseDownMultiSelectNode.Text}");

                    this.treeView.SelectedNode = this._selectedNodes.LastOrDefault();
                }
                else
                {
                    this._mouseDownMultiSelectNode = null;
                }
            }

            if ( this._mouseDownClickedNode != null )
            {
                if (!ModifierKeys.HasFlag(Keys.Control) &&
                    this._mouseDownClickedNode.Bounds.Contains(e.X, e.Y))
                {
                    this._mouseDownClickedNode = null;

                    foreach (TreeNode selectedNode in this._selectedNodes)
                    {
                        Debug.WriteLine($"Removed {selectedNode.Text}");
                        selectedNode.Checked = false;
                        //e.Node.BackColor = Color.Empty;
                    }

                    this._selectedNodes.Clear();
                }
            }

            //this._mouseDownMultiSelectNode?.Bounds.Contains(e.X, e.Y) ?

            //TreeNode clickedNode = this.treeView.GetNodeAt(e.X, e.Y);
            //if (clickedNode != null && clickedNode.Bounds.Contains(e.X, e.Y))
            //{
            //    if (this._mouseDownMultiSelectNode == clickedNode &&
            //        clickedNode.Checked &&
            //        this._selectedNodes.Contains(clickedNode))
            //    {
            //        clickedNode.Checked = false;

            //        this._selectedNodes.Remove(clickedNode);
            //        Debug.WriteLine($"Removed {clickedNode.Text}");

            //        this.treeView.SelectedNode = this._selectedNodes.LastOrDefault();
            //    }
            //}
        }

        private void OnTreeViewBeforeSelect( object sender, TreeViewCancelEventArgs e )
        {
            if ( !ModifierKeys.HasFlag( Keys.Control ) )
            {
                foreach ( TreeNode selectedNode in this._selectedNodes )
                {
                    Debug.WriteLine($"Removed {selectedNode.Text}");
                    selectedNode.Checked = false;
                    //e.Node.BackColor = Color.Empty;
                }

                this._selectedNodes.Clear();
            }
            else if ( e.Node == this._mouseDownMultiSelectNode )
            {
                e.Cancel = true;

                this._mouseDownMultiSelectNode = null;
            }
        }

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

            if ( !e.Node.Checked )
            {
                Debug.WriteLine( $"Added {e.Node.Text}" );

                e.Node.Checked = true;

                this._selectedNodes.Add( e.Node );
            }

                //if ( ModifierKeys.HasFlag( Keys.Control ) )
                //{
                //    e.Node.BackColor = Color.Aqua;
                //}
            //}
        }

        private void OnTreeViewMouseClick( object sender, MouseEventArgs e )
        {
            if (this.treeView.SelectedNode?.Tag is Maze maze)
            {
                //MazeUi mazeUi = (MazeUi)
                //    this._windowManager.Interfaces.FirstOrDefault(
                //        ui => ui is MazeUi mui && mui.Maze.Equals( maze ) );

                //if ( mazeUi != null )
                //{
                //    this._windowManager.Show(mazeUi);
                //}
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

        private Image LoadImage(string imagePath )
        {
            return ResourceLoader.GetEmbeddedImage( this.GetImageUri( imagePath ) );
        }

        private Uri GetImageUri( string imagePath )
        {
            return PackUriHelper.Create( ResourceLoader.ApplicationUri,
                new Uri($"{ResourcePath}{imagePath}", UriKind.Relative));
        }

        private void OnLoadFromRomCommand(object obj)
        {
            DialogLoadROM dlr = new DialogLoadROM(Path.GetFullPath("../"));

            DialogResult dr = dlr.ShowDialog();

            if (dr == DialogResult.OK)
            {
                //MazeCollectionController collectionController =
                //    new MazeCollectionController(dlr.MazeCollection);

                //TreeNode node = collectionController.TreeRender(treeView, null, toolStripButtonGrid.Checked);
                //node.ImageIndex = 0;
                //node.SelectedImageIndex = node.ImageIndex;

                //collectionController.MazeCollection.PropertyChanged += this.OnInstructionPropertyChanged;

                dlr.MazeCollection.PropertyChanged += this.OnMazeCollectionPropertyChanged;

                this.treeView.SelectedNode = this.Add(dlr.MazeCollection);
                //((SelectedItems)this._selectedNodes)._items.Add( dlr.MazeCollection.Mazes[8]);
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

                foreach (Maze maze in mazeCollection.Mazes)
                {
                    this.AddSibling(mazeCollectionNode, maze);
                }

                this.treeView.Nodes.Insert(
                    this.treeView.Nodes.IndexOf(
                        this.treeView.SelectedNode?.Parent ??
                        this.treeView.SelectedNode) + 1, mazeCollectionNode);

                mazeCollectionNode.Expand();
                if (!mazeCollectionNode.IsVisible)
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
                                 ImageIndex = (int)maze.MazeType + 1,
                                 SelectedImageIndex = (int)maze.MazeType + 1
                             };

            parent.Nodes.Add( child );
        }
    }
}
