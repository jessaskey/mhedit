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
    public class Data
    {
        private string _id;

        private string _length;

        private YesNoType _reserved = YesNoType.No;

        private string _start;

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( YesNoType.No ) ]
        public YesNoType Reserved
        {
            get { return this._reserved; }
            set { this._reserved = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Start
        {
            get { return this._start; }
            set { this._start = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Length
        {
            get { return this._length; }
            set { this._length = value; }
        }
    }

}