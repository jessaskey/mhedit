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
    public class GameInfo
    {
        private string _comment;

        private string _manufacturer;

        private string _name;

        private string _releaseDate;

        private string _version;

        /// <remarks />
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <remarks />
        public string Version
        {
            get { return this._version; }
            set { this._version = value; }
        }

        /// <remarks />
        public string Manufacturer
        {
            get { return this._manufacturer; }
            set { this._manufacturer = value; }
        }

        /// <remarks />
        [ XmlElement( DataType = "gYearMonth" ) ]
        public string ReleaseDate
        {
            get { return this._releaseDate; }
            set { this._releaseDate = value; }
        }

        /// <remarks />
        public string Comment
        {
            get { return this._comment; }
            set { this._comment = value; }
        }
    }

}