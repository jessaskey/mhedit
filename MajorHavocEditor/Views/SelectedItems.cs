﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    /// <summary>
    /// Provides TreeNode implementation details for the objects contained
    /// in the TreeView's ItemsSource. 
    /// </summary>
    public interface IItemsSourceDelegate
    {
        /// <summary>
        /// Creates a TreeNode for the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        TreeNode CreateNode( object item );

        /// <summary>
        /// Determines if the TreeNode is the node containing the Item.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Equals( TreeNode node, object item );

        /// <summary>
        /// Returns an Enumerable for the children of the item, or null
        /// if the item has no children. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IEnumerable GetEnumerable( object item );
    }

    /// <summary>
    /// Public facing type for the TreeView.ItemsSource
    /// </summary>
    public interface IItemsSource : IList
    {
        IItemsSourceDelegate ItemsDelegate { get; set; }
    }

    public class SelectedItems : ISelectedNodes
    {
        private readonly IList _items;
        private readonly TryFindNode _tryFindNode;
        private readonly List<TreeNode> _nodes = new List<TreeNode>();

        public SelectedItems( TryFindNode tryFindNode )
            : this( new ObservableCollection<object>(), tryFindNode )
        {
        }

        public SelectedItems( IList items, TryFindNode tryFindNode )
        {
            this._items = items;
            this._tryFindNode = tryFindNode;

            if ( items is INotifyCollectionChanged incc )
            {
                incc.CollectionChanged += this.OnItemsCollectionChanged;
            }
        }

        private void OnItemsCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
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
            this._items.Add( node.Tag );
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
        public IList Items
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