using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace GameEditor.Core.Hardware
{

    [Serializable]
    public enum EndianType
    {
        /// <remarks />
        [XmlEnum( "big" )]
        Big,

        /// <remarks />
        [XmlEnum( "little" )]
        Little
    }

}
