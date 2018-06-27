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
        private string _idField;

        private string _lengthField;

        private YesNoType _reservedField = YesNoType.No;

        private string _startField;

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( YesNoType.No ) ]
        public YesNoType Reserved
        {
            get { return this._reservedField; }
            set { this._reservedField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Start
        {
            get { return this._startField; }
            set { this._startField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Length
        {
            get { return this._lengthField; }
            set { this._lengthField = value; }
        }
    }

}