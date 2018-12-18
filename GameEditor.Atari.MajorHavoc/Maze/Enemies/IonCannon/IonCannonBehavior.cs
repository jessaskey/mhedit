using GameEditor.Core.Serialization;
using System;
using System.Runtime.Serialization;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    public abstract class IonCannonBehavior : IRomSerializable
    {
        private readonly Commands _command;

        public IonCannonBehavior( Commands command )
        {
            this._command = command;
        }

        public Commands Command
        {
            get
            {
                return this._command;
            }
        }

        protected byte SerializeCommand( byte value )
        {
            /// Cannon command is serialized into upper 2 bits of byte.
            return (byte)( ( value & 0x3F ) | (byte)this.Command << 6 );
        }

        public abstract void GetObjectData( RomSerializationInfo si, StreamingContext context );
    }
}
