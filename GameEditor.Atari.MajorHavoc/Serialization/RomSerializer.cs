using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Atari.MajorHavoc.Serialization
{
    /// <summary>
    /// Provides the proper encoding for serialization operations on
    /// Major Havoc ROMs 
    /// </summary>
    public class RomSerializer : Core.Serialization.RomSerializer
    {
        public RomSerializer()
            :base( new StringEncoding() )
        {}
    }
}
