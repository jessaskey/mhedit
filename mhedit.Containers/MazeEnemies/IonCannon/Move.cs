﻿using mhedit.Containers.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed partial class Move : IonCannonInstruction
    {
        private int _waitFrames;
        private SimpleVelocity _velocity = new SimpleVelocity();

        public Move()
            : base( Commands.Move )
        {
            this._velocity.PropertyChanged += this.ForwardIsDirtyPropertyChanged;
        }

        //private Move( RomSerializationInfo si, StreamingContext context )
        //    : this()
        //{
        //    this._waitFrames = ( si.GetByte( "PackedInfo" ) & 0x3F ) << 2;

        //    if ( this._waitFrames > 0 )
        //    {
        //        this._velocity.X = si.GetSByte( "X" );

        //        this._velocity.Y = si.GetSByte( "Y" );
        //    }
        //}

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

                this.SetField( ref this._waitFrames, value & 0xFC );
            }
        }

        [TypeConverter( typeof( SimpleVelocityTypeConverter ) )]
        public SimpleVelocity Velocity
        {
            get
            {
                return _velocity;
            }
        }

        [XmlIgnore]
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty | this._velocity.IsDirty;
            }
            set
            {
                base.IsDirty = this._velocity.IsDirty = value;
            }
        }

        public override void GetObjectData( List<byte> bytes )
        {
            bytes.Add( this.SerializeCommand( (byte)( this._waitFrames >> 2 ) ) );

            if ( this._waitFrames > 0 )
            {
                bytes.Add( (byte)this._velocity.X );

                bytes.Add( (byte)this._velocity.Y );
            }
        }

        public override string ToString()
        {
            return $"Move";
            //return $"Move [ {this.Velocity}, WaitFrames:{this.WaitFrames} ]";
        }
    }
}
