using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace GameEditor.Core.Xml
{

    [ GeneratedCode( "xsd", "4.6.1055.0" ) ]
    [ Serializable ]
    [ DebuggerStepThrough ]
    [ DesignerCategory( "code" ) ]
    //[ XmlType( AnonymousType = true, Namespace = "http://askey.org/GameProfile" ) ]
    [ XmlRoot( Namespace = "http://askey.org/GameProfile", IsNullable = false ) ]
    public class StorageDefinitionRef
    {
        private byte[] _delimiter;

        private string _id;

        private int _order;

        /// <remarks />
        [ XmlElement( DataType = "hexBinary" ) ]
        public byte[] Delimiter
        {
            get { return this._delimiter; }
            set { this._delimiter = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 0 ) ]
        public int Order
        {
            get { return this._order; }
            set { this._order = value; }
        }
    }

}