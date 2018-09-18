using GameEditor.Core.Hardware;
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
        private EpromRef _ePromRef;

        private string _id;

        private StorageDefinition _storageDefinition;

        private ChecksumType _type = ChecksumType.Additive;

        /// <remarks />
        [ XmlElement( "EPROMRef", typeof( EpromRef ) ) ]
        public EpromRef EpromRef
        {
            get { return this._ePromRef; }
            set { this._ePromRef = value; }
        }

        /// <remarks />
        public StorageDefinition StorageDefinition
        {
            get { return this._storageDefinition; }
            set { this._storageDefinition = value; }
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
        [ DefaultValue( ChecksumType.Additive ) ]
        public ChecksumType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }
    }

}