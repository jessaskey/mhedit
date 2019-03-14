using mhedit.Containers.TypeConverters;
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
        private SimpleVelocity _velocity;

        public Move()
            : base( Commands.Move )
        {
            this.Velocity = new SimpleVelocity();
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
            get { return _velocity; }
            set
            {
                this.SetField( ref this._velocity, value );

                this._velocity.PropertyChanged += this.ForwardPropertyChanged;
            }
        }

        #region Implementation of IChangeTracking

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged |
                    this._velocity.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._velocity.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion

        public override void GetObjectData( List<byte> bytes )
        {
            bytes.Add( this.SerializeCommand( (byte)( this._waitFrames >> 2 ) ) );

            if ( this._waitFrames > 0 )
            {
                bytes.Add( (byte)this._velocity.X );

                bytes.Add( (byte)this._velocity.Y );
            }
        }

        //public override string ToString()
        //{
        //    //return $"Move [ {this.Velocity}, WaitFrames:{this.WaitFrames} ]";
        //}
    }
}
