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
    public class EpromRef
    {
        private string _endField;

        private string _idField;

        private string _startField;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
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
        public string End
        {
            get { return this._endField; }
            set { this._endField = value; }
        }
    }

}