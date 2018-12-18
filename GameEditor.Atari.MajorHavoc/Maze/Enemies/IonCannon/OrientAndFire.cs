﻿using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class OrientAndFire : IonCannonBehavior
    {
        private Orientation _orientation = Orientation.Down;
        private RotateSpeed _rotateSpeed = RotateSpeed.Slow;
        private int _shotSpeed = 0;

        public OrientAndFire()
            : base( Commands.OrientAndFire )
        { }

        private OrientAndFire( RomSerializationInfo si, StreamingContext context )
            : this()
        {
            byte value = si.GetByte( "PackedInfo" );

            this._orientation = (Orientation)( ( value & 0x38 ) >> 3 );

            this._rotateSpeed = (RotateSpeed)( ( value & 0x06 ) >> 1 );

            if ( (value & 0x01) != 0 )
            {
                this._shotSpeed = si.GetByte( "ShotSpeed" );
            }
        }

        public Orientation Orientation
        {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
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
                _rotateSpeed = value;
            }
        }

        public int ShotSpeed
        {
            get
            {
                return _shotSpeed;
            }
            set
            {
                if ( value > 255 || value < 0 )
                {
                    throw new ArgumentOutOfRangeException( nameof( ShotSpeed ),
                        value, "Must be 0 < value < 255." );
                }

                _shotSpeed = value;
            }
        }

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            int value = (int)this._orientation << 3;

            value |= (int)this._rotateSpeed << 1;

            value |= this._shotSpeed > 0 ? 0x01 : 0x00;

            si.AddValue( "PackedInfo", this.SerializeCommand( (byte)value ) );

            if ( this._shotSpeed > 0 )
            {
                si.AddValue( "ShotSpeed", (byte)this._shotSpeed );
            }
        }
    }
}
