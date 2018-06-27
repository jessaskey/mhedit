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
        private string _addressBaseField = "0x0000";

        private int _addressWidthField;

        private int _busWidthField = 8;

        private string _idField;

        private List<Rom> _romField;

        /// <remarks />
        [ XmlElement( "ROM" ) ]
        //[ XmlArrayItem( "ROM", IsNullable = false ) ]
        public List<Rom> Rom
        {
            get { return this._romField; }
            set { this._romField = value; }
        }

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int AddressWidth
        {
            get { return this._addressWidthField; }
            set { this._addressWidthField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( "0x0000" ) ]
        public string AddressBase
        {
            get { return this._addressBaseField; }
            set { this._addressBaseField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 8 ) ]
        public int BusWidth
        {
            get { return this._busWidthField; }
            set { this._busWidthField = value; }
        }
    }

}