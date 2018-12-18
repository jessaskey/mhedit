using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed partial class Move : IonCannonBehavior
    {
        private int _waitFrames;
        private readonly SimpleVelocity _velocity = new SimpleVelocity();

        public Move()
            : base( Commands.Move )
        { }

        private Move( RomSerializationInfo si, StreamingContext context )
            : this()
        {
            this._waitFrames = ( si.GetByte( "PackedInfo" ) & 0x3F ) << 2;

            if ( this._waitFrames > 0 )
            {
                this._velocity.X = si.GetSByte( "X" );

                this._velocity.Y = si.GetSByte( "Y" );
            }
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

        public SimpleVelocity Velocity
        {
            get
            {
                return _velocity;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            si.AddValue( "PackedInfo",
                this.SerializeCommand( (byte)( this._waitFrames >> 2 ) ) );

            if ( this._waitFrames > 0)
            {
                si.AddValue( "X", (sbyte)this._velocity.X );

                si.AddValue( "Y", (sbyte)this._velocity.Y );
            }
        }
    }
}
