using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
            private readonly List<object> _treeSelections = new List<object>();

            public SelectedItemsModerator( IList<MazeObject> mazeSelections )
            {
                this._mazeSelections = mazeSelections;

                if (mazeSelections is INotifyCollectionChanged incc)
                {
                    incc.CollectionChanged += this.OnItemsCollectionChanged;
                }
            }

            private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.OnPropertyChanged(nameof(this.SelectedObjects));
            }

            public object[] SelectedObjects
            {
                get
                {
                    return this._treeSelections
                               .Concat( this._mazeSelections )
                               .ToArray();
                }
            }

#region Implementation of IEnumerable

            /// <inheritdoc />
            public IEnumerator GetEnumerator()
            {
                return this._treeSelections
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
                    this._treeSelections.Add( value );

                    if ( value is IGrouping<Type, MazeObject> grouping )
                    {
                        foreach (MazeObject o in grouping)
                        {
                            this._mazeSelections.Add(o);
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
                add { ((INotifyCollectionChanged)this._mazeSelections).CollectionChanged += value; }
                remove { ((INotifyCollectionChanged)this._mazeSelections).CollectionChanged -= value; }
            }

 #endregion
        }
    }

}
