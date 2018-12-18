using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze
{
    /// <summary>
    /// Atari Vectors are in the 4th quadrant
    /// 
    /// Origin
    /// (0,0)-X-----> Positive
    /// |
    /// Y
    /// |
    /// |
    /// \/
    /// Negative
    /// 
    /// Atari has each grid square equal to 0x100 (256 decimal)
    ///
    /// It gets even better... the MH Grid System is a little wonky where the main
    /// grid for the maze sections is larger than the object grid... the object grid
    /// is relative to each maze pattern. 
    /// 
    /// There are 2 types of points, HiRez and LowRez
    /// </summary>
    [Serializable]
    public abstract class MazePosition : IRomSerializable
    {
        protected short _x;
        protected short _y;

        public virtual short X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public virtual short Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        /// <summary>
        /// Reset the position to 0.0. (origin)
        /// </summary>
        public virtual void Reset()
        {
            this._x = this._y = 0;
        }

        public abstract void GetObjectData( RomSerializationInfo si, StreamingContext context );
    }

}
