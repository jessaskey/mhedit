using System;
using System.Windows.Forms;
using mhedit.Containers;
using MajorHavocEditor.Views.Dialogs;

namespace MajorHavocEditor.Services
{
    public interface IRomManager
    {
        MazeCollection LoadFrom( string path = null );

        void Create( MazeCollection mazeCollection, string path = null );

        void InsertMaze( Maze maze, int level = 0, string path = null );
    }

    public class RomManager : IRomManager
    {
#region Implementation of IRomManager

        /// <inheritdoc />
        public MazeCollection LoadFrom( string path = null )
        {
            DialogLoadROM dlr = new DialogLoadROM( path );

            DialogResult dr = dlr.ShowDialog();

            return dr == DialogResult.OK ? dlr.MazeCollection : null;
        }

        /// <inheritdoc />
        public void Create( MazeCollection mazeCollection, string path = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void InsertMaze( Maze maze, int level = 0, string path = null )
        {
            throw new NotImplementedException();
        }

#endregion
    }
}
