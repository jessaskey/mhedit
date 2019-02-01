using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// Base class for cannon behavior programming.
    /// </summary>
    [Serializable]
    [XmlInclude( typeof( ReturnToStart ) )]
    [XmlInclude( typeof( OrientAndFire ) )]
    [XmlInclude( typeof( Move ) )]
    [XmlInclude( typeof( Pause ) )]
    public abstract class IonCannonInstruction
    {
        private readonly Commands _command;

        public IonCannonInstruction( Commands command )
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
            /// IonCannon command is serialized into upper 2 bits of byte.
            return (byte)( ( value & 0x3F ) | (byte)this.Command << 6 );
        }

        public abstract void GetObjectData( List<byte> bytes );
    }
}
