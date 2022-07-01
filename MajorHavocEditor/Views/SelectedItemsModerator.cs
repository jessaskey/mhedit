using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    }

}
