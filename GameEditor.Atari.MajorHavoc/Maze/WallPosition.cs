using GameEditor.Core.Serialization;
using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze
{
    [Serializable]
    public sealed class WallPosition : MazePosition
    {
        private const int GridUnits = 8;
        private const int GridUnitStamps = 8;
        private const int MaximumWalls = 209;

        private byte _index;

        public WallPosition()
        {}

        private WallPosition( RomSerializationInfo si, StreamingContext context )
        {
            this._index = si.GetByte( "WallPosition" );

            /// Need to figure out how to capture maze type so we can "adjust" the
            /// row/column value.
            //this._x =//int col = stamp % _mazeStampsX;
            //this._y =//int row = stamp / _mazeStampsX;
        }

        /// <summary>
        /// Represents the index into the 1 dimensional map wall array.
        /// </summary>
        public int Index
        {
            get
            {
                return this._index;
            }
            //set
            //{
            //    this._index = value;
            //}
        }

        /// <summary>
        /// Explicit Point to WallPosition conversion operator.
        /// </summary>
        /// <param name="p"></param>
        public static explicit operator WallPosition( Point p )
        {
            int col = p.X / ( GridUnits * GridUnitStamps );
            int row = p.Y / ( GridUnits * GridUnitStamps );

            WallPosition wp = new WallPosition();

            //wp._index = Math.Max( Math.Min( ( row * _mazeStampsX ) + col, MAXWALLS ), 0 );

            return wp;
        }

        /// <summary>
        /// Implicit WallPositoin to Point conversion operator
        /// </summary>
        /// <param name="wp"></param>
        public static implicit operator Point( WallPosition wp )
        {
            return new Point( 
                wp.X * GridUnits * GridUnitStamps,
                wp.Y * GridUnits * GridUnitStamps );
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "WallPosition", this._index );
        }
    }
}
