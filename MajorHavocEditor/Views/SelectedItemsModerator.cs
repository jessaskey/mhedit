using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Views
{

    public partial class MazeExplorer
    {
        /// <summary>
        /// Allow MazeObject selection to pass back and forth between the tree
        /// and the Maze but reserve the other tree items locally.
        /// </summary>
        private class SelectedItemsModerator : NotifyPropertyChangedBase, IList, INotifyCollectionChanged
        {
            private readonly IList<MazeObject> _mazeSelections;
            private readonly ObservableCollection<object> _treeSelections = 
                new ObservableCollection<object>();

            public SelectedItemsModerator( IList<MazeObject> mazeSelections )
            {
                this._mazeSelections = mazeSelections;
            }

#region Implementation of IEnumerable

            /// <inheritdoc />
            public IEnumerator GetEnumerator()
            {
                /// Right now this object manages only a Maze, all it's MazeObjects,
                /// and the groups that organize the MazObjects. However, should never
                /// include the Group objects in the SelectedItems collection!
                return this._treeSelections
                           .Where( o => o is not IGrouping<Type, MazeObject> )
                           .Concat( this._mazeSelections )
                           .GetEnumerator();
            }

#endregion

#region Implementation of ICollection

            /// <inheritdoc />
            public void CopyTo( Array array, int index )
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public int Count
            {
                get { return this._treeSelections.Count + this._mazeSelections.Count; }
            }

            /// <inheritdoc />
            public bool IsSynchronized
            {
                get { return false; }
            }

            /// <inheritdoc />
            public object SyncRoot
            {
                get { return this._treeSelections; }
            }

#endregion

#region Implementation of IList

            /// <inheritdoc />
            public int Add( object value )
            {
                if ( value is MazeObject mazeObject )
                {
                    this._mazeSelections.Add( mazeObject );
                }
                else
                {
                    this._treeSelections.Add(value);

                    if ( ModifierKeys.HasFlag( Keys.Control ) )
                    {
                        if (value is IGrouping<Type, MazeObject> grouping)
                        {
                            foreach (MazeObject o in grouping)
                            {
                                this._mazeSelections.Add(o);
                            }
                        }
                    }
                }

                // Lie.. Ha!
                return 0;
            }

            /// <inheritdoc />
            public void Clear()
            {
                this._treeSelections.Clear();
                
                this._mazeSelections.Clear();
            }

            /// <inheritdoc />
            public bool Contains( object value )
            {
                return this._treeSelections.Contains( value ) ||
                       this._mazeSelections.Contains( value );
            }

            /// <inheritdoc />
            public int IndexOf( object value )
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void Insert( int index, object value )
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void Remove( object value )
            {
                if (value is MazeObject mazeObject)
                {
                    this._mazeSelections.Remove(mazeObject);
                }
                else
                {
                    this._treeSelections.Remove(value);

                    if (value is IGrouping<Type, MazeObject> grouping)
                    {
                        foreach (MazeObject o in grouping)
                        {
                            this._mazeSelections.Remove(o);
                        }
                    }
                }
            }

            /// <inheritdoc />
            public void RemoveAt( int index )
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsFixedSize
            {
                get { return false; }
            }

            /// <inheritdoc />
            public bool IsReadOnly
            {
                get { return false; }
            }

            /// <inheritdoc />
            public object this[ int index ]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

#endregion

#region Implementation of INotifyCollectionChanged

            /// <inheritdoc />
            event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
            {
                // Forward directly into the MazeObjects collection.
                add
                {
                    ((INotifyCollectionChanged)this._mazeSelections).CollectionChanged += value;
                    ((INotifyCollectionChanged)this._treeSelections).CollectionChanged += value;
                }
                remove
                {
                    ((INotifyCollectionChanged)this._mazeSelections).CollectionChanged -= value;
                    ((INotifyCollectionChanged)this._treeSelections).CollectionChanged -= value;
                }
            }

 #endregion
        }

        /// <summary>
        /// This class allows the TreeView to have a cleaner view of the
        /// editable objects in a Maze, which includes grouping of maze
        /// objects and access to the Maze's properties.
        /// </summary>
        private class MazeExplorerItemsSource : List<object>, IItemsSourceDelegate
        {
            private readonly Maze _maze;
            private readonly ObservableCollection<NamedGrouping> _groupedMazeObjects =
                new ObservableCollection<NamedGrouping>();

            public MazeExplorerItemsSource( Maze maze )
            {
                this._maze = maze;

                /// In this hierarchy the maze is always at the top.. there's only
                /// one of them, and it's never removed. This simplifies a few
                /// design features:
                /// - Inherit from simple List<object> because the top collection
                /// never changes.
                this.Add( maze );

                // Force update to the existing values in the maze
                this.OnMazeObjectsCollectionChanged( maze.MazeObjects,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                if (maze.MazeObjects is INotifyCollectionChanged incc)
                {
                    incc.CollectionChanged += this.OnMazeObjectsCollectionChanged;
                }
            }

            private void OnMazeObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    var sorted = this._maze.MazeObjects
                                  .ToLookup(o => o.GetType())
                                  .OrderBy(o => o.Key.Name == typeof(MazeWall).Name)
                                  .ThenBy(o => o.Key.Name)
                                  .Select( g => new NamedGrouping(g) )
                                  .ToList();

                    foreach (NamedGrouping grouping in sorted )
                    {
                        this._groupedMazeObjects.Add( grouping );
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    var sorted = e.NewItems
                                  .Cast<MazeObject>()
                                  .ToLookup(o => o.GetType())
                                  .OrderBy(o => o.Key.Name)
                                  .ToList();

                    foreach (IGrouping<Type, MazeObject> grouping in sorted)
                    {
                        NamedGrouping found =
                            this._groupedMazeObjects.FirstOrDefault(
                                g => grouping.Key.Name.Equals( g.Key.Name ) );

                        /// adding a new group.
                        if ( found == null )
                        {
                            NamedGrouping before =
                                this._groupedMazeObjects.FirstOrDefault(
                                    g => g.Name.CompareTo( grouping.Key.Name ) > 0 );

                            int index = this._groupedMazeObjects.IndexOf( before );

                            this._groupedMazeObjects.Insert(
                                index < 0 ? this._groupedMazeObjects.Count : index,
                                new NamedGrouping( grouping ) );
                        }
                        else
                        {
                            foreach ( MazeObject mazeObject in grouping )
                            {
                                found.Add( mazeObject);
                            }
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var sorted = e.OldItems
                                  .Cast<MazeObject>()
                                  .ToLookup(o => o.GetType())
                                  .OrderBy(o => o.Key.Name)
                                  .ToList();

                    foreach (IGrouping<Type, MazeObject> grouping in sorted)
                    {
                        NamedGrouping found =
                            this._groupedMazeObjects.FirstOrDefault(
                                g => grouping.Key.Name.Equals(g.Key.Name));

                        // This should never be null!!
                        if (found != null)
                        {
                            /// Removing the entire Group??
                            if ( found.Count == grouping.Count() )
                            {
                                this._groupedMazeObjects.Remove(found);
                            }
                            else
                            {
                                foreach (MazeObject toRemove in grouping)
                                {
                                    found.Remove(toRemove);
                                }
                            }
                        }
                    }
                }
            }

#region Implementation of IItemsSourceDelegate

            /// <inheritdoc />
            public TreeNode CreateNode(object item)
            {
                int imageIndex = 5;

                return new BoundTreeNode(((IName)item).Name)
                       {
                           Tag = item,
                           ImageIndex = imageIndex,
                           SelectedImageIndex = imageIndex,
                       }
                    .ConnectPropertyChanged(item as INotifyPropertyChanged);
            }

            /// <inheritdoc />
            public void OnRemoveNode(TreeNode node)
            {
                ((BoundTreeNode)node).DisconnectPropertyChanged(
                    node.Tag as INotifyPropertyChanged );
            }

            /// <inheritdoc />
            public bool Equals(TreeNode node, object item)
            {
                return ReferenceEquals(node.Tag, item);
            }

            /// <inheritdoc />
            public IEnumerable GetEnumerable(TreeNode item)
            {
                if (item.Tag is Maze maze)
                {
                    /// Lie, forward to our groups collection.
                    return this._groupedMazeObjects;
                }
                else if (item.Tag is IEnumerable enumerable)
                {
                    return enumerable;
                }

                return null;
            }

#endregion

        }
    }

}
