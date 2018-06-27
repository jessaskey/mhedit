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
    [ XmlRoot( Namespace = "http://askey.org/GameProfile", IsNullable = false ) ]
    public class Array
    {
        private string _idField;

        private PageRef _pageRefField;

        private int _sizeField = 1;

        private ArrayStorage _storageField = ArrayStorage.Value;

        private ArrayType _typeField;

        /// <remarks />
        public PageRef PageRef
        {
            get { return this._pageRefField; }
            set { this._pageRefField = value; }
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
        public ArrayType Type
        {
            get { return this._typeField; }
            set { this._typeField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( ArrayStorage.Value ) ]
        public ArrayStorage Storage
        {
            get { return this._storageField; }
            set { this._storageField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 1 ) ]
        public int Size
        {
            get { return this._sizeField; }
            set { this._sizeField = value; }
        }
    }

}