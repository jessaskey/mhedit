using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public sealed class ReturnToStart : IonCannonBehavior
    {
        public ReturnToStart() 
            : base( Commands.ReturnToStart )
        {}

        private ReturnToStart( RomSerializationInfo si, StreamingContext context )
            : this()
        {}

        public override void GetObjectData( RomSerializationInfo si, StreamingContext context )
        {
            /// Return To Start is a single null byte. just write out 0
            si.AddValue( "ReturnToStart", this.SerializeCommand( 0 ) );
        }
    }
}
