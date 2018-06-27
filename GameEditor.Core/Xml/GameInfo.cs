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
        private string _commentField;

        private string _manufacturerField;

        private string _nameField;

        private string _releaseDateField;

        private string _versionField;

        /// <remarks />
        public string Name
        {
            get { return this._nameField; }
            set { this._nameField = value; }
        }

        /// <remarks />
        public string Version
        {
            get { return this._versionField; }
            set { this._versionField = value; }
        }

        /// <remarks />
        public string Manufacturer
        {
            get { return this._manufacturerField; }
            set { this._manufacturerField = value; }
        }

        /// <remarks />
        [ XmlElement( DataType = "gYearMonth" ) ]
        public string ReleaseDate
        {
            get { return this._releaseDateField; }
            set { this._releaseDateField = value; }
        }

        /// <remarks />
        public string Comment
        {
            get { return this._commentField; }
            set { this._commentField = value; }
        }
    }

}