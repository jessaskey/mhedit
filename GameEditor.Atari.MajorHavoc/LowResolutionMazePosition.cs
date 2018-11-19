using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc
{
    /// <summary>
    /// Pulls a Maze Position from a single byte. X is least significant nibble
    /// and Y is most significant nibble.
    /// </summary>
    [Serializable]
    public class LowResolutionMazePosition : MazePosition
    {
        public LowResolutionMazePosition()
        {
            this._x = 0x0100;
        }

        protected LowResolutionMazePosition( RomSerializationInfo si, StreamingContext context )
        {
            byte value = si.GetByte( "LowResolutionMazePosition" );

            /// LSN plus 1 shifted to upper byte.
            this._x = (short)( ( ( value & 0x0F ) + 1 ) << 8 );

            /// MSN shifted to upper byte and then negated.
            this._y = (short)( ( ( value & 0xF0 ) << 4 ) + 0xF000 );
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            byte value = (byte)( ( ( this._x & 0x0f00 ) >> 8 ) - 1 );

            value += (byte)( ( this._y & 0x0F00 ) >> 4 );

            si.AddValue( "LowResolutionMazePosition", value );
        }
    }

}
