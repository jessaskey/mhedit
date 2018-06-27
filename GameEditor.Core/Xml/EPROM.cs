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
        private string _idField;

        private string _nameField;

        private int _sizeField;

        private int _widthField;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Name
        {
            get { return this._nameField; }
            set { this._nameField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int Size
        {
            get { return this._sizeField; }
            set { this._sizeField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public int Width
        {
            get { return this._widthField; }
            set { this._widthField = value; }
        }
    }

}