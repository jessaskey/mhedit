using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// TripPad Pyroids have a position that is specially encoded.
    /// </summary>
    [Serializable]
    public class TripPadPyroidPosition : MazePosition
    {
        public TripPadPyroidPosition()
        { }

        protected TripPadPyroidPosition( RomSerializationInfo si, StreamingContext context )
        {
            /// this is seriously messed up. Have to talk with Jess.
            byte valueX = si.GetByte( "MazePositionX" );

            PyroidStyle style = ( valueX & 0x80 ) != 0 ? PyroidStyle.Single : PyroidStyle.Double;

            valueX &= 0x7f;

            /// default to this value, if velocity warrants it will change the LSB
            /// Why did we switch to 0x1f here?
            this._x = (short)( ( valueX & 0x1f ) << 8 | 0x40 );

            this._y = (short)( si.GetByte( "MazePositionY" ) << 8 | 0x80 );
        }

        public override void Reset()
        {
            /// I don't know what the default values for x/y should be.
            /// And do we 
            throw new NotImplementedException();
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            throw new NotImplementedException();
            si.AddValue( "MazePositionX", (byte)( this._x >> 8 ) );

            si.AddValue( "MazePositionY", (byte)( this._y >> 8 ) );
        }
    }


    [Serializable]
    [ConcreteType( typeof( TripPadPyroidPosition ) )]
    [SerializationSurrogate( typeof( TripPad ) )]
    public sealed class TripPadPyroid : MazeObject
    {
        private byte _velocity = 0;

        public TripPadPyroid()
            : base( "TripPadPyroid", new TripPadPyroidPosition() )
        {}

        private TripPadPyroid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            byte velocity = si.GetByte( "Velocity" );

            this._velocity = (byte)( velocity & 0x7f );

            if ( ( velocity & 0x80 ) == 0 )
            {
                this.Position.X = (short)( ( this.Position.X & 0x00 ) | 0x80 );
            }
            else
            {
                this._velocity = (byte)( this._velocity * -1 );
            }
        }

        public int Velocity
        {
            get
            {
                return this._velocity;
            }
            set
            {
                /// Values must be confined to less than 0x7F????
                if ( value > -128 &&  value < 128)
                {
                    throw new ArgumentOutOfRangeException( nameof( value ),
                        value, $"Must be -128 < value < 128." );
                }

                this._velocity = (byte)value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            byte velocity = this._velocity;

            if ( velocity < 0 )
            {
                /// make positive
                velocity = (byte)( this._velocity * -1 );
            }

            if ( (this.Position.X & 0xFF ) == 0x40 )
            {
                velocity |= 0x80;
            }

            si.AddValue( "Velocity", this._velocity );
        }
    }
}
