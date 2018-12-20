using GameEditor.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies
{
    /// <summary>
    /// I could have sworn that this enum isn't real!! When I looked at the ROMs
    /// I saw Owen put 2 trips in the same place to get 2 Pyroids launched??
    /// </summary>
    //public enum TripReleases
    //{
    //    TwoPyroids = 0,
    //    OnePyroid = 0x80
    //}


    /// <summary>
    /// TripPadPyroids have a wacky packed position algo that must be implemented in
    /// the TripPadPyroid object itself. As a result the IRomSerializable interface
    /// is implemented with empty methods.
    /// </summary>
    [Serializable]
    public class TripPadPyroidPosition : MazePosition
    {
        public TripPadPyroidPosition()
        {}

        protected TripPadPyroidPosition( RomSerializationInfo si, StreamingContext context )
        {}

        // TODO: Must implement way to keep LSB from being modified!
        public override short X
        {
            set { this._x = value; }
        }

        public override short Y
        {
            set { this._y = value; }
        }

        public override void Reset()
        {
            /// I don't know what the default values for x/y should be.
            throw new NotImplementedException();
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {}
    }


    [Serializable]
    [ConcreteType( typeof( TripPadPyroidPosition ) )]
    [SerializationSurrogate( typeof( TripPad ) )]
    public sealed class TripPadPyroid : MazeObject
    {
        private sbyte _velocity = 0;
        private bool _releaseTwoPyroids = true;

        public TripPadPyroid()
            : base( "TripPadPyroid", new TripPadPyroidPosition() )
        {}

        private TripPadPyroid( RomSerializationInfo si, StreamingContext context )
            : base( si, context )
        {
            /// this is seriously messed up. Have to talk with Jess.
            byte valueX = si.GetByte( "MazePositionX" );

            /// Move the Y value into the MSB of Position.Y 
            this.Position.Y = (short)( si.GetByte( "MazePositionY" ) << 8 | 0x80 );

            /// Bit7 determines # of pyroids released when tripped. Bit7 set indicates
            /// a single pyroid.
            this._releaseTwoPyroids = ( valueX & 0x80 ) == 0x00;

            byte velocity = si.GetByte( "Velocity" );

            /// Record velocity as if it were positive.
            this.Velocity = velocity & 0x7F;

            /// Use bit7 to set the LSB of Position.X and choose the sign of the
            /// velocity value.
            if ( ( velocity & 0x80 ) == 0 )
            {
                this.Position.X = 0x80;
            }
            else
            {
                this.Position.X = 0x40;

                this.Velocity *= -1;
            }

            /// Move the X value into the MSB of Position.X 
            /// Why did we switch to 0x1f here?
            //valueX &= 0x7f;
            this.Position.X |= (short)( ( valueX & 0x1f ) << 8 );
        }

        /// <summary>
        /// Looking at the Production ROM data the max/min value for velocity
        /// was +-6. Technically speaking we could allow +-128 and that would
        /// still allow the funky packing algo to work properly. Lets limit it
        /// to +-16 though.
        /// </summary>
        public int Velocity
        {
            get
            {
                return this._velocity;
            }
            set
            {
                if ( value > -16 &&  value < 16)
                {
                    throw new ArgumentOutOfRangeException( nameof( value ),
                        value, $"Must be -16 < value < 16." );
                }

                this._velocity = (sbyte)value;

                /// TODO: Must implement update to LSB of position based upon velocity
                /// sign!
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// True if 2 pyroids are released when the trippad is triggered.
        /// </summary>
        public bool ReleaseTwoPyroids
        {
            get
            {
                return this._releaseTwoPyroids;
            }

            set
            {
                this._releaseTwoPyroids = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            base.GetObjectData( si, context );

            byte valueX = (byte)( this.Position.X >> 8 );

            valueX |= (byte)( this.ReleaseTwoPyroids ? 0x00 : 0x80 );

            si.AddValue( "MazePositionX", valueX );

            si.AddValue( "MazePositionY", (byte)( this.Position.Y >> 8 ) );

            /// Record velocity as if it were positive.
            byte velocity = 
                (byte)( this.Velocity > 0 ? this.Velocity : this.Velocity * -1 );

            /// Use Bit7 to indicate +-;
            velocity |= (byte)( this.Velocity > 0 ? 0x00 : 0x80 );

            si.AddValue( "Velocity", velocity );
        }
    }
}
