﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mhedit.Containers;

namespace MajorHavocEditor.Views
{

    public partial class GameExplorer
    {
        private class ItemsSourceDelegate : IItemsSourceDelegate
        {
#region Implementation of IItemsSourceDelegate

            /// <inheritdoc />
            public TreeNode CreateNode( object item )
            {
                if ( item is Maze maze )
                {
                    return new TreeNode( maze.Name )
                           {
                               Tag = maze,
                               ForeColor = Color.Black,
                               ImageIndex = (int) maze.MazeType + 1,
                               SelectedImageIndex = (int) maze.MazeType + 1
                           };
                }

                MazeCollection mazeCollection = (MazeCollection) item;

                return new TreeNode( mazeCollection.Name )
                       {
                           Tag = mazeCollection,
                           ImageIndex = 0,
                           SelectedImageIndex = 0
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
                return item is MazeCollection mazeCollection ?
                           mazeCollection.Mazes :
                           null;
            }

#endregion
        }
    }

}
