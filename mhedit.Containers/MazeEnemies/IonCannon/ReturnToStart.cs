using System;
using System.Collections.Generic;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class ReturnToStart : IonCannonInstruction
    {
        public ReturnToStart() 
            : base( Commands.ReturnToStart )
        {}

        //private ReturnToStart( RomSerializationInfo si, StreamingContext context )
        //    : this()
        //{}

        public override void GetObjectData( List<byte> bytes )
        {
            /// Return To Start is a single null byte. just write out 0
            bytes.Add( this.SerializeCommand( 0 ) );
        }

        //public override string ToString()
        //{
        //    return $"Return to Start";
        //}
    }
}
