using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Views
{

    public class MultiSelectTreeView : TreeView
    {
        private class TreeViewItemsSource : ObservableCollection<object>, IItemsSource
        {
            private class DefaultDelegate : IItemsSourceDelegate
            {
                public static IItemsSourceDelegate Instance =
                    new Lazy<IItemsSourceDelegate>( () => new DefaultDelegate() ).Value;

#region Implementation of IItemsSourceDelegate

                /// <inheritdoc />
                public TreeNode CreateNode( object item )
                {
                    return new TreeNode( item.ToString() )
                           {
                               Tag = item,
                           };
                }

                /// <inheritdoc />
                public bool Equals( TreeNode node, object item )
                {
                    return node.Tag.Equals( item );
                }

                /// <inheritdoc />
                public IEnumerable GetEnumerable( object item )
                {
                    return item is IEnumerable enumerable ? (IEnumerable) enumerable : null;
                }

#endregion
            }

            private readonly MultiSelectTreeView _treeView;
            private IItemsSourceDelegate _itemsDelegate = DefaultDelegate.Instance;

            public TreeViewItemsSource( MultiSelectTreeView treeView )
            {
                this._treeView = treeView;
            }

#region Implementation of IItemsSource

            /// <inheritdoc />
            public IItemsSourceDelegate ItemsDelegate
            {
                get { return this._itemsDelegate; }
                set
                {
                    this._itemsDelegate = value ?? DefaultDelegate.Instance;

                    //value ?? throw new ArgumentNullException(nameof(this.ItemsDelegate));
                }
            }

#endregion

#region Overrides of ObservableCollection<object>

            /// <inheritdoc />
            protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
            {
                base.OnCollectionChanged( e );

                this.OnItemsCollectionChanged( this, e );
            }

#endregion

            private void OnItemsCollectionChanged( object sender,
                NotifyCollectionChangedEventArgs e )
            {
                try
                {
                    this._treeView.BeginUpdate();

                    if ( e.Action == NotifyCollectionChangedAction.Reset )
                    {
                        // for now...
                        throw new NotImplementedException();

                        //foreach (TreeNode node in this.)
                        //{
                        //    Debug.WriteLine($"Removed {node.Tag}");
                        //}
                    }
                    else if ( e.Action == NotifyCollectionChangedAction.Add )
                    {
                        foreach ( object newItem in e.NewItems )
                        {
                            Debug.WriteLine( $"Added {newItem}" );

                            // BUG: Need to use Sender to determine Node Leaf!
                            // Find root NodeCollection to add this leaf to.
                            TreeNodeCollection root =
                                sender == this ?
                                    this._treeView.Nodes :
                                    this._treeView.FindNodeOrDefault(
                                        n => this._itemsDelegate.Equals( n, sender ) ).Nodes;

                            TreeNode nodeToAdd = this.CreateHierarchy( newItem );

                            nodeToAdd.Expand();

                            root.Add( nodeToAdd );

                            // scroll to last added?
                            if ( !nodeToAdd.IsVisible )
                            {
                                // must end update before EnsureVisible Hierarchy
                                this._treeView.EndUpdate();
                                nodeToAdd.EnsureVisible();
                            }
                        }
                    }
                    else if ( e.Action == NotifyCollectionChangedAction.Remove )
                    {
                        foreach ( object oldItem in e.OldItems )
                        {
                            Debug.WriteLine( $"Removed {oldItem}" );

                            this._treeView.Nodes.Remove( this.DisolveHierarchy( oldItem ) );
                        }
                    }
                    else
                    {
                        // for now...
                        throw new NotImplementedException();
                    }
                }
                finally
                {
                    this._treeView.EndUpdate();
                }
            }

            private TreeNode CreateHierarchy( object item )
            {
                TreeNode node = this._itemsDelegate.CreateNode( item );

                if ( TryGetEnumerable( item, out IEnumerable enumerable ) )
                {
                    foreach ( object child in enumerable )
                    {
                        node.Nodes.Add( this.CreateHierarchy( child ) );
                    }

                    if ( enumerable is INotifyCollectionChanged incc )
                    {
                        incc.CollectionChanged += this.OnItemsCollectionChanged;
                    }
                }

                return node;
            }

            private TreeNode DisolveHierarchy( object item )
            {
                // On recursive calls the Child TreeNode is passed to avoid nested searching.
                TreeNode node = item as TreeNode ??
                                this._treeView.FindNodeOrDefault(
                                    n => this._itemsDelegate.Equals( n, item ) );

                // Unsubscribe events on enumerable first.
                IEnumerable enumerable = this._itemsDelegate.GetEnumerable( node.Tag );

                if ( enumerable is INotifyCollectionChanged incc )
                {
                    incc.CollectionChanged -= this.OnItemsCollectionChanged;
                }

                this._treeView._selectedNodes.Remove( node );

                foreach ( TreeNode childNode in node.Nodes )
                {
                    this.DisolveHierarchy( childNode );
                }

                return node;
            }

            private bool TryGetEnumerable( object item, out IEnumerable enumerable )
            {
                return ( enumerable = this._itemsDelegate.GetEnumerable( item ) ) != null;
            }
        }

        private ISelectedNodes _selectedNodes;
        private TreeNode _mouseDownMultiSelectNode;
        private TreeNode _mouseDownSelectedNode;
        private bool _cancelUnwantedLabelEdit;

        private readonly IItemsSource _itemsSource;

        public MultiSelectTreeView()
        {
            this._selectedNodes = new SelectedItems( this.SearchNodesForTag );
            this._itemsSource = new TreeViewItemsSource( this );
            this.LabelEdit = true;
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.HideSelection = false;
            this.ShowLines = false;
        }

        /// <summary>
        /// Collection of nodes..
        /// </summary>
        public IItemsSource ItemsSource
        {
            get { return this._itemsSource; }
        }

        /// <summary>
        /// The collection of selected Items in the TreeView. If the user wants to
        /// be able to manipulate the set of SelectedItems they should set the property
        /// with their own IEnumerable that implements INotifyCollectionChanged.
        /// </summary>
        public IEnumerable SelectedItems
        {
            get { return this._selectedNodes.Items; }
            set
            {
                if ( value is IList iList )
                {
                    this._selectedNodes.Clear();

                    this._selectedNodes = new SelectedItems( iList, this.SearchNodesForTag );
                }
            }
        }

#region Overrides of TreeView

#region Label Edit Overrides

        /// <inheritdoc />
        protected override void OnBeforeLabelEdit( NodeLabelEditEventArgs e )
        {
            base.OnBeforeLabelEdit( e );

            if ( ( ModifierKeys & ( Keys.Control | Keys.Shift ) ) > 0 ||
                 this._cancelUnwantedLabelEdit )
            {
                e.CancelEdit = true;
            }
        }

        /// <inheritdoc />
        protected override void OnAfterLabelEdit( NodeLabelEditEventArgs e )
        {
            base.OnAfterLabelEdit( e );

            //https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.nodelabelediteventargs?view=netframework-4.7.2
            if ( !e.CancelEdit && e.Label != null )
            {
                try
                {
                    // Stop editing without canceling the label change.
                    e.Node.EndEdit( false );

                    // Expect the IName implementer to throw if the name is invalid.
                    if ( e.Node.Tag is IName iName )
                    {
                        iName.Name = e.Label;
                    }
                }
                catch ( Exception ex )
                {
                    // Cancel the label edit action, inform the user, and 
                    // place the node in edit mode again.
                    e.CancelEdit = true;

                    MessageBox.Show( $"{ex.Message}", "Label Edit Error" );

                    e.Node.BeginEdit();
                }
            }
        }

#endregion

#region Draw Node Overrides

        /// <inheritdoc />
        /// TODO: Move Editing/ChangeTracking outside of the Treeview? 
        protected override void OnDrawNode( DrawTreeNodeEventArgs e )
        {
            /// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.treeview.drawnode?redirectedfrom=MSDN&view=netframework-4.7.2
            //TODO: User Override
            /// Force the default of this event to be true, but give the user a chance to
            /// override.
            //e.DrawDefault = true;

            base.OnDrawNode( e );

            ///// Only draw node text if the user asks us to.
            //if ( !e.DrawDefault )
            //{
            //    return;
            //}

            //e.DrawDefault = false;

            Rectangle adjustedBounds =
                new Rectangle( e.Bounds.X, e.Bounds.Y, this.Bounds.Width, e.Bounds.Height );

            // Retrieve the node font. If the node font has not been set, use the TreeView font.
            Font nodeFont = e.Node.NodeFont ?? this.Font;

            if ( e.Node.IsEditing )
            {
                // While editing the Node Text don't paint the existing name behind.
                e.Graphics.FillRectangle( new SolidBrush( SystemColors.Window ), adjustedBounds );
            }

            // Draw the background and node text for a selected node.
            else if ( ( e.State & ( TreeNodeStates.Selected | TreeNodeStates.Focused ) ) != 0 ||
                      e.Node.Checked )
            {
                // Draw the background of the selected node. 
                //TODO: Don't paint in background when Focused.
                e.Graphics.FillRectangle( new SolidBrush( SystemColors.Highlight ),
                    adjustedBounds );

                // Draw the node text.
                TextRenderer.DrawText( e.Graphics, e.Node.Text, nodeFont, adjustedBounds,
                    SystemColors.HighlightText, TextFormatFlags.Left );
            }
            else // Use the default background and node text.
            {
                e.DrawDefault = true;
            }

            // Draw an extra char to the right if the node has been edited.
            if ( e.Node.Tag is IChangeTracking ict )
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    ict.IsChanged ? "*" : "",
                    nodeFont,
                    new Point( e.Node.Bounds.Right + 4, e.Node.Bounds.Top ),
                    ( e.State & TreeNodeStates.Selected ) != 0 ?
                        SystemColors.HighlightText :
                        SystemColors.ControlText,
                    TextFormatFlags.Left );
            }

            // If the node has focus, draw the focus rectangle.
            if ( ( e.State & TreeNodeStates.Focused ) != 0 )
            {
                using ( Pen focusPen = new Pen( Color.Black ) )
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                    Rectangle focusBounds = adjustedBounds;

                    focusBounds.Size =
                        new Size( focusBounds.Width - 1, focusBounds.Height - 1 );

                    e.Graphics.DrawRectangle( focusPen, focusBounds );
                }
            }
        }

#endregion

        /// <inheritdoc />
        protected override void OnBeforeSelect( TreeViewCancelEventArgs e )
        {
            base.OnBeforeSelect( e );

            if ( !ModifierKeys.HasFlag( Keys.Control ) && e.Action != TreeViewAction.Unknown )
            {
                this._selectedNodes.Clear();
            }
            else if ( e.Node == this._mouseDownMultiSelectNode )
            {
                e.Cancel = true;

                this._mouseDownMultiSelectNode = null;
            }
        }

        /// <inheritdoc />
        protected override void OnAfterSelect( TreeViewEventArgs e )
        {
            base.OnAfterSelect( e );

            this._selectedNodes.Add( e.Node );
        }

#endregion

#region Overrides of Control

        /// <inheritdoc />
        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            // Reset every Down/up cycle...
            this._cancelUnwantedLabelEdit = false;

            TreeNode clickedNode = this.GetNodeAt( e.X, e.Y );

            if ( clickedNode != null )
            {
                // This captures clicking on an already selected node to unselect.
                this._mouseDownMultiSelectNode =
                    clickedNode.Checked &&
                    this._selectedNodes.Count > 1 &&
                    ModifierKeys.HasFlag( Keys.Control ) &&
                    clickedNode.Bounds.Contains( e.X, e.Y ) ? clickedNode : null;

                // This captures clicking on the Treeview.SelectedNode to unselect.
                this._mouseDownSelectedNode =
                    this._selectedNodes.Count > 1 &&
                    clickedNode == this.SelectedNode ? clickedNode : null;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseUp( MouseEventArgs e )
        {
            if ( this._mouseDownMultiSelectNode != null )
            {
                // If control key still applied and MouseUp occurs on the same (MouseDown) Node.
                if ( ModifierKeys.HasFlag( Keys.Control ) &&
                     this._mouseDownMultiSelectNode.Bounds.Contains( e.X, e.Y ) )
                {
                    this._mouseDownMultiSelectNode.Checked = false;

                    this._selectedNodes.Remove( this._mouseDownMultiSelectNode );

                    this.SelectedNode = this._selectedNodes.LastOrDefault();
                }
                else
                {
                    this._mouseDownMultiSelectNode = null;
                }
            }

            if ( this._mouseDownSelectedNode != null )
            {
                if ( !ModifierKeys.HasFlag( Keys.Control ) &&
                     this._mouseDownSelectedNode.Bounds.Contains( e.X, e.Y ) )
                {
                    this._mouseDownSelectedNode = null;

                    this._selectedNodes.Clear();

                    this._cancelUnwantedLabelEdit = this.LabelEdit;
                }
            }
        }

#endregion

        private bool SearchNodesForTag( object key, out TreeNode found )
        {
            return ( found = this.FindNodeOrDefault( key ) ) != null;
        }
    }

}
