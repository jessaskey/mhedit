using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Atari.MajorHavoc.Serialization
{
    /// <summary>
    /// Provides the proper binary reader/writer for serialization of
    /// Major Havoc ROMs 
    /// </summary>
    public class RomSerializer : Core.Serialization.RomSerializer
    {
        public RomSerializer( Type type )
            : base( type )
        { }

        public RomSerializer( Type iEnumerable, int length )
            : base( iEnumerable, length )
        { }

        public override object Deserialize( Stream serializationStream )
        {
            return this.Deserialize( new HavocBinaryReader( serializationStream ) );
        }

        public override void Serialize( Stream serializationStream, object graph )
        {
            this.Serialize( new HavocBinaryWriter( serializationStream ), graph );
        }
    }
}
