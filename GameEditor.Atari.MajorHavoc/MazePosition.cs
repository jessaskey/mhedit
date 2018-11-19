using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc
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

        public short X
        {
            get { return this._x; }
            ///TODO:: make abstract and force implement to range check.
            set { this._x = value; }
        }

        public short Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public abstract void GetObjectData( RomSerializationInfo si, StreamingContext context );
    }

}
