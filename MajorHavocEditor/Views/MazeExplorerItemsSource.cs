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
                    new NotifyCollectionChangedEventArgs( NotifyCollectionChangedAction.Reset ) );

                if ( maze.MazeObjects is INotifyCollectionChanged incc )
                {
                    incc.CollectionChanged += this.OnMazeObjectsCollectionChanged;
                }
            }

            private void OnMazeObjectsCollectionChanged( object sender,
                NotifyCollectionChangedEventArgs e )
            {
                if ( e.Action == NotifyCollectionChangedAction.Reset )
                {
                    var sorted = this._maze.MazeObjects
                                     .ToLookup( o => o.GetType() )
                                     .OrderBy( o => o.Key.Name == typeof( MazeWall ).Name )
                                     .ThenBy( o => o.Key.Name )
                                     .Select( g => new NamedGrouping( g ) )
                                     .ToList();

                    foreach ( NamedGrouping grouping in sorted )
                    {
                        this._groupedMazeObjects.Add( grouping );
                    }
                }
                else if ( e.Action == NotifyCollectionChangedAction.Add )
                {
                    var sorted = e.NewItems
                                  .Cast<MazeObject>()
                                  .ToLookup( o => o.GetType() )
                                  .OrderBy( o => o.Key.Name )
                                  .ToList();

                    foreach ( IGrouping<Type, MazeObject> grouping in sorted )
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
                                found.Add( mazeObject );
                            }
                        }
                    }
                }
                else if ( e.Action == NotifyCollectionChangedAction.Remove )
                {
                    var sorted = e.OldItems
                                  .Cast<MazeObject>()
                                  .ToLookup( o => o.GetType() )
                                  .OrderBy( o => o.Key.Name )
                                  .ToList();

                    foreach ( IGrouping<Type, MazeObject> grouping in sorted )
                    {
                        NamedGrouping found =
                            this._groupedMazeObjects.FirstOrDefault(
                                g => grouping.Key.Name.Equals( g.Key.Name ) );

                        // This should never be null!!
                        if ( found != null )
                        {
                            /// Removing the entire Group??
                            if ( found.Count == grouping.Count() )
                            {
                                this._groupedMazeObjects.Remove( found );
                            }
                            else
                            {
                                foreach ( MazeObject toRemove in grouping )
                                {
                                    found.Remove( toRemove );
                                }
                            }
                        }
                    }
                }
            }

#region Implementation of IItemsSourceDelegate

            /// <inheritdoc />
            public TreeNode CreateNode( object item )
            {
                string imageKey = BoundTreeNode.GetImageKey(item);

                return new BoundTreeNode( ( (IName) item ).Name )
                       {
                           Tag = item,
                           ImageKey = imageKey,
                           SelectedImageKey = imageKey,
                       }
                    .ConnectPropertyChanged( item as INotifyPropertyChanged );
            }

            /// <inheritdoc />
            public void OnRemoveNode( TreeNode node )
            {
                ( (BoundTreeNode) node ).DisconnectPropertyChanged(
                    node.Tag as INotifyPropertyChanged );
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
                    /// Lie, forward to our groups collection.
                    return this._groupedMazeObjects;
                }
                else if ( item.Tag is IEnumerable enumerable )
                {
                    return enumerable;
                }

                return null;
            }

#endregion

        }
    }

}
