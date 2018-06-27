using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{

    /// <remarks />
    [ GeneratedCode( "xsd", "4.6.1055.0" ) ]
    [ Serializable ]
    //[ XmlType( Namespace = "http://askey.org/GameProfile" ) ]
    public enum ArrayType
    {
        /// <remarks />
        [ XmlEnum( "boolean" ) ]
        Boolean,

        /// <remarks />
        [ XmlEnum( "byte" ) ]
        Byte,

        /// <remarks />
        [ XmlEnum( "short" ) ]
        Short,

        /// <remarks />
        [ XmlEnum( "int" ) ]
        Int,

        /// <remarks />
        [ XmlEnum( "string" ) ]
        String,

        /// <remarks />
        [ XmlEnum( "object" ) ]
        Object
    }

}