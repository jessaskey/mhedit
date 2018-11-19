using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc
{
    /// <summary>
    /// Pulls a Maze Position from a pair of short values that are ordered X, Y.
    /// </summary>
    [Serializable]
    public class HighResolutionMazePosition : MazePosition
    {
        public HighResolutionMazePosition()
        { }

        protected HighResolutionMazePosition( RomSerializationInfo si, StreamingContext context )
        {
            short valueX = si.GetInt16( "MazePositionX" );

            short valueY = si.GetInt16( "MazePositionY" );
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "MazePositionX", this._x );

            si.AddValue( "MazePositionY", this._y );
        }
    }

}
