using GameEditor.Core.Hardware;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{

    /// <remarks />
    [ GeneratedCode( "xsd", "4.6.1055.0" ) ]
    [ Serializable ]
    [ DebuggerStepThrough ]
    [ DesignerCategory( "code" ) ]
    //[ XmlType( AnonymousType = true, Namespace = "http://askey.org/GameProfile" ) ]
    public class MemoryMap
    {
        private string _addressBase = "0x0000";

        private int _addressWidth;

        private int _busWidth = 8;

        private EndianType _endianness = EndianType.Little;

        private string _id;

        private List<Rom> _rom;

        /// <remarks />
        /// This should have a base class like Segments or something.
        [ XmlElement( "ROM" ) ]
        public List<Rom> Roms
        {
            get { return this._rom; }
            set { this._rom = value; }
        }

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int AddressWidth
        {
            get { return this._addressWidth; }
            set { this._addressWidth = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( "0x0000" ) ]
        public string AddressBase
        {
            get { return this._addressBase; }
            set { this._addressBase = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 8 ) ]
        public int DataWidth
        {
            get { return this._busWidth; }
            set { this._busWidth = value; }
        }

        /// <remarks />
        [XmlAttribute]
        [DefaultValue( EndianType.Little )]
        public EndianType Endianness
        {
            get { return this._endianness; }
            set { this._endianness = value; }
        }

    }

}