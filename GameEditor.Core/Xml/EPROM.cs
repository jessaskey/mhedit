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
    public class Eprom
    {
        private string _id;

        private string _name;

        private int _size;

        private int _width;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int Width
        {
            get { return this._width; }
            set { this._width = value; }
        }
    }

}