using System;
using System.Collections.Generic;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class OrientAndFire : IonCannonInstruction
    {
        private Orientation _orientation = Orientation.Down;
        private RotateSpeed _rotateSpeed = RotateSpeed.Slow;
        private int _shotSpeed = 1;

        public OrientAndFire()
            : base( Commands.OrientAndFire )
        { }

        //private OrientAndFire( RomSerializationInfo si, StreamingContext context )
        //    : this()
        //{
        //    byte value = si.GetByte( "PackedInfo" );

        //    this._orientation = (Orientation)( ( value & 0x38 ) >> 3 );

        //    this._rotateSpeed = (RotateSpeed)( ( value & 0x06 ) >> 1 );

        //    if ( (value & 0x01) != 0 )
        //    {
        //        this._shotSpeed = si.GetByte( "ShotSpeed" );
        //    }
        //}

        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                this.SetField( ref this._orientation, value );
            }
        }

        public RotateSpeed RotateSpeed
        {
            get
            {
                return _rotateSpeed;
            }
            set
            {
                this.SetField( ref this._rotateSpeed, value );
            }
        }

        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=1;Maximum=255" )]
        public int ShotSpeed
        {
            get
            {
                return _shotSpeed;
            }
            set
            {
                this.SetField( ref this._shotSpeed, value );
            }
        }

        public override void GetObjectData( List<byte> bytes )
        {
            int value = (int)this._orientation << 3;

            value |= (int)this._rotateSpeed << 1;

            value |= this._shotSpeed > 0 ? 0x01 : 0x00;

            bytes.Add( this.SerializeCommand( (byte)value ) );

            if ( this._shotSpeed > 0 )
            {
                bytes.Add( (byte)this._shotSpeed );
            }
        }

        //public override string ToString()
        //{
        //    //return $"Orient and Fire [ {this.Orientation}, {this.RotateSpeed}, ShotSpeed:{this.ShotSpeed}]";
        //}
    }
}
