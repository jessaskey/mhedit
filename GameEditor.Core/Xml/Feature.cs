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
    public class Feature
    {
        private string _idField;

        private object[] _itemsField;

        private int _maximumField = 1;

        /// <remarks />
        [ XmlElement( "StorageDefinition", typeof( StorageDefinition ) ) ]
        [ XmlElement( "StorageDefinitionRef", typeof( StorageDefinitionRef ) ) ]
        public object[] Items
        {
            get { return this._itemsField; }
            set { this._itemsField = value; }
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
        [ DefaultValue( 1 ) ]
        public int Maximum
        {
            get { return this._maximumField; }
            set { this._maximumField = value; }
        }
    }

}