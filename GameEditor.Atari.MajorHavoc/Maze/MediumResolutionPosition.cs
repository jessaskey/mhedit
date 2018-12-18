using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze
{
    /// <summary>
    /// Pulls a Maze Position from a pair of byte values that are ordered X, Y.
    /// </summary>
    [Serializable]
    public class MediumResolutionPosition : MazePosition
    {
        public MediumResolutionPosition()
        { }

        protected MediumResolutionPosition( RomSerializationInfo si, StreamingContext context )
        {
            byte valueX = si.GetByte( "MazePositionX" );

            byte valueY = si.GetByte( "MazePositionY" );
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            byte value = (byte)this._x;

            si.AddValue( "MazePositionX", value );

            value = (byte)this._y;

            si.AddValue( "MazePositionY", value );
        }
    }

}
