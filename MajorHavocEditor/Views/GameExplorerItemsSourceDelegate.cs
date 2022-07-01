using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer
    {
        private class GameExplorerItemsSourceDelegate : IItemsSourceDelegate
        {
            /// <summary>
            /// I hate WinForms... This class is needed to properly disconnect
            /// the INotifyPropertyChanged.PropertyChanged event from the node
            /// and prevent memory leaks.
            /// </summary>
            private class BoundTreeNode : TreeNode
            {
                public BoundTreeNode( string text )
                    : base( text )
                {
                }

                public TreeNode ConnectPropertyChanged( INotifyPropertyChanged inpc )
                {
                    inpc.PropertyChanged += this.OnPropertyChanged;

                    return this;
                }

                public void DisconnectPropertyChanged( INotifyPropertyChanged inpc )
                {
                    inpc.PropertyChanged += this.OnPropertyChanged;
                }

                private void OnPropertyChanged( object sender, PropertyChangedEventArgs args )
                {
                    if ( args.PropertyName.Equals( nameof( IName.Name ) ) )
                    {
                        this.Text = ( (IName) sender ).Name;
                    }
                    else if (args.PropertyName.Equals(nameof(Maze.MazeType)))
                    {
                        this.ImageIndex = this.Tag is Maze maze ? (int)maze.MazeType + 1 : 0;
                        this.SelectedImageIndex = this.ImageIndex;
                    }
                }
            }

#region Implementation of IItemsSourceDelegate

            /// <inheritdoc />
            public TreeNode CreateNode( object item )
            {
                int imageIndex = item is Maze maze ? (int) maze.MazeType + 1 : 0;

                return new BoundTreeNode(((IName)item).Name)
                       {
                           Tag = item,
                           ImageIndex = imageIndex,
                           SelectedImageIndex = imageIndex,
                       }
                    .ConnectPropertyChanged( (INotifyPropertyChanged) item );
            }

            /// <inheritdoc />
            public void OnRemoveNode( TreeNode node )
            {
                ( (BoundTreeNode) node ).DisconnectPropertyChanged(
                    (INotifyPropertyChanged) node.Tag );
            }

            /// <inheritdoc />
            public bool Equals( TreeNode node, object item )
            {
                return ReferenceEquals( node.Tag, item );
            }

            /// <inheritdoc />
            public IEnumerable GetEnumerable( TreeNode item )
            {
                return item.Tag is MazeCollection mazeCollection ?
                           mazeCollection.Mazes :
                           null;
            }

#endregion
        }
    }

}
