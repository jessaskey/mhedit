using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Forms;

namespace MajorHavocEditor.Views
{

    public partial class MultiSelectTreeView
    {
        private class NodeManager
        {
            private class DefaultDelegate : IItemsSourceDelegate
            {
                public static readonly IItemsSourceDelegate Instance =
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
            private readonly IList _items;
            private IItemsSourceDelegate _itemsDelegate = DefaultDelegate.Instance;

            public NodeManager( MultiSelectTreeView treeView )
                : this( new ObservableCollection<object>(), treeView )
            {
            }

            public NodeManager( IList items, MultiSelectTreeView treeView )
            {
                this._items = items;

                this._treeView = treeView;

                if ( items is INotifyCollectionChanged incc )
                {
                    incc.CollectionChanged += this.OnItemsCollectionChanged;
                }
            }

            public IItemsSourceDelegate ItemsDelegate
            {
                get { return this._itemsDelegate; }
                set
                {
                    this._itemsDelegate = value ?? DefaultDelegate.Instance;
                    //value ?? throw new ArgumentNullException(nameof(this.ItemsDelegate));
                }
            }

            public IList Items
            {
                get { return this._items; }
            }

            private void OnItemsCollectionChanged( object sender,
                NotifyCollectionChangedEventArgs e )
            {
                try
                {
                    this._treeView.BeginUpdate();

                    if ( e.Action == NotifyCollectionChangedAction.Reset )
                    {
                        // Find root NodeCollection to add this leaf to.
                        TreeNodeCollection root =
                            ReferenceEquals( sender, this._items ) ?
                                this._treeView.Nodes :
                                this._treeView.FindNodeOrDefault(
                                    n => this._itemsDelegate.Equals( n, sender ) ).Nodes;

                        root.Clear();

                        // reload any nodes that are remaining.
                        this.OnItemsCollectionChanged( sender,
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Add, (IList) sender ) );
                    }
                    else if ( e.Action == NotifyCollectionChangedAction.Add )
                    {
                        // Find root NodeCollection to add this leaf to.
                        TreeNodeCollection root =
                            ReferenceEquals( sender, this._items ) ?
                                this._treeView.Nodes :
                                this._treeView.FindNodeOrDefault(
                                    n => this._itemsDelegate.Equals( n, sender ) ).Nodes;

                        foreach ( object newItem in e.NewItems )
                        {
                            Debug.WriteLine( $"Added {newItem}" );

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

                if ( this.TryGetEnumerable( item, out IEnumerable enumerable ) )
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
    }

}
