using System;
using System.Collections.Generic;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class Pause : IonCannonInstruction
    {
        private int _waitFrames;

        public Pause()
            : base( Commands.Pause )
        { }

        //private Pause( RomSerializationInfo si, StreamingContext context )
        //    : this()
        //{
        //    this._waitFrames = ( si.GetByte( "PackedInfo" ) & 0x3F ) << 2;
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

        public override void GetObjectData( List<byte> bytes )
        {
            bytes.Add( this.SerializeCommand( (byte)( this._waitFrames >> 2 ) ) );
        }

        //public override string ToString()
        //{
        //    //return $"Pause  [ WaitFrames:{this.WaitFrames} ]";
        //}
    }
}
