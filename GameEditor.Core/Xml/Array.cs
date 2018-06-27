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
        private string _id;

        private PageRef _pageRef;

        private int _size = 1;

        private ArrayStorage _storage = ArrayStorage.Value;

        private ArrayType _type;

        /// <remarks />
        public PageRef PageRef
        {
            get { return this._pageRef; }
            set { this._pageRef = value; }
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
        public ArrayType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( ArrayStorage.Value ) ]
        public ArrayStorage Storage
        {
            get { return this._storage; }
            set { this._storage = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 1 ) ]
        public int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }
    }

}