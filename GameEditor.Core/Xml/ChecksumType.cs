using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{

    /// <remarks />
    [ GeneratedCode( "xsd", "4.6.1055.0" ) ]
    [ Serializable ]
    //[ XmlType( Namespace = "http://askey.org/GameProfile" ) ]
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