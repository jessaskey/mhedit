using System;
using System.CodeDom.Compiler;
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
    public class Checksum
    {
        private EpromRef _ePromRefField;

        private string _idField;

        private StorageDefinition _storageDefinitionField;

        private ChecksumType _typeField = ChecksumType.Additive;

        /// <remarks />
        [ XmlElement( "EPROMRef", typeof( EpromRef ) ) ]
        public EpromRef EpromRef
        {
            get { return this._ePromRefField; }
            set { this._ePromRefField = value; }
        }

        /// <remarks />
        public StorageDefinition StorageDefinition
        {
            get { return this._storageDefinitionField; }
            set { this._storageDefinitionField = value; }
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
        [ DefaultValue( ChecksumType.Additive ) ]
        public ChecksumType Type
        {
            get { return this._typeField; }
            set { this._typeField = value; }
        }
    }

}