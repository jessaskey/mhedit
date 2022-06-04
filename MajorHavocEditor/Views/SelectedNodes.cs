using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public partial class MultiSelectTreeView
    {
        private class SelectedNodes : ISelectedNodes
        {
            private readonly IList _items;
            private readonly TryFindNode _tryFindNode;
            private readonly List<TreeNode> _nodes = new List<TreeNode>();

            public SelectedNodes( TryFindNode tryFindNode )
                : this( new ObservableCollection<object>(), tryFindNode )
            {
            }

            public SelectedNodes( IList items, TryFindNode tryFindNode )
            {
                this._items = items;
                this._tryFindNode = tryFindNode;

                if ( items is INotifyCollectionChanged incc )
                {
                    incc.CollectionChanged += this.OnItemsCollectionChanged;
                }
            }

            private void OnItemsCollectionChanged( object sender,
                NotifyCollectionChangedEventArgs e )
            {
                if ( e.Action == NotifyCollectionChangedAction.Reset )
                {
                    foreach ( TreeNode node in this._nodes )
                    {
                        node.Checked = false;

                        Debug.WriteLine( $"Removed {node.Tag}" );
                    }

                    this._nodes.Clear();
                }
                else if ( e.Action == NotifyCollectionChangedAction.Add )
                {
                    foreach ( object newItem in e.NewItems )
                    {
                        Debug.WriteLine( $"Added {newItem}" );

                        if ( this._tryFindNode( newItem, out TreeNode node ) )
                        {
                            node.Checked = true;

                            this._nodes.Add( node );

                            node.TreeView.SelectedNode = node;
                        }
                        else // Adding an item that isn't in the treeview?!
                        {
                            throw new InvalidOperationException(
                                $"{newItem} isn't contained in ItemsSource.");
                        }
                    }
                }
                else if ( e.Action == NotifyCollectionChangedAction.Remove )
                {
                    foreach ( object oldItem in e.OldItems )
                    {
                        Debug.WriteLine( $"Removed {oldItem}" );

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
            void ISelectedNodes.Add( TreeNode node )
            {
                if ( !this._items.Contains( node.Tag ) )
                {
                    this._items.Add(node.Tag);
                }
            }

            /// <inheritdoc />
            void ISelectedNodes.Clear()
            {
                this._items.Clear();
            }

            /// <inheritdoc />
            bool ISelectedNodes.Contains( TreeNode node )
            {
                return this._nodes.Contains( node );
            }

            /// <inheritdoc />
            bool ISelectedNodes.Remove( TreeNode node )
            {
                this._items.Remove( node.Tag );

                return true;
            }

            /// <inheritdoc />
            public IList SelectedItems
            {
                get { return this._items; }
            }

            /// <inheritdoc />
            IEnumerator<TreeNode> IEnumerable<TreeNode>.GetEnumerator()
            {
                return this._nodes.GetEnumerator();
            }

#endregion

#region Implementation of IEnumerable

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                // IEnumerable<TreeNode> inherits from IEnumerable so return the NODES collection!!!
                return this._nodes.GetEnumerator();
            }

#endregion
        }
    }

}