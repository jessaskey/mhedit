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
        private byte[] _delimiterField;

        private string _idField;

        private int _orderField;

        /// <remarks />
        [ XmlElement( DataType = "hexBinary" ) ]
        public byte[] Delimiter
        {
            get { return this._delimiterField; }
            set { this._delimiterField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 0 ) ]
        public int Order
        {
            get { return this._orderField; }
            set { this._orderField = value; }
        }
    }

}