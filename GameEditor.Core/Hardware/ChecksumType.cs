using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace GameEditor.Core.Hardware
{

    [ Serializable ]
    public enum ChecksumType
    {
        /// <remarks />
        Crc32,

        /// <remarks />
        Crc16,

        /// <remarks />
        Additive
    }

}