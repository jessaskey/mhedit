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
        public RomSerializer()
        {}

        /// <summary>
        ///  Forwards into the base RomSerializer.Deserialize() method.
        /// </summary>
        public T Deserialize<T>( Stream serializationStream, int? length = null )
        {
            return this.Deserialize<T>(
                new HavocBinaryReader( serializationStream ), length );
        }

        /// <summary>
        ///  Forwards into the base RomSerializer.Serialize() method.
        /// </summary>
        public void Serialize( Stream serializationStream, object graph )
        {
            this.Serialize( new HavocBinaryWriter( serializationStream ), graph );
        }
    }
}
