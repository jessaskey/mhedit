using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class Pause : IonCannonBehavior
    {
        private int _waitFrames;

        public Pause()
            : base( Commands.Pause )
        { }

        private Pause( RomSerializationInfo si, StreamingContext context )
            : this()
        {
            this._waitFrames = ( si.GetByte( "PackedInfo" ) & 0x3F ) << 2;
        }

        public int WaitFrames
        {
            get
            {
                return _waitFrames;
            }
            set
            {
                if ( value > 255 || value < 0 )
                {
                    throw new ArgumentOutOfRangeException( nameof( WaitFrames ),
                        value, "Must be 0 < value < 255." );
                }

                _waitFrames = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "PackedInfo", 
                this.SerializeCommand( (byte)( this._waitFrames >> 2 ) ) );
        }
    }
}
