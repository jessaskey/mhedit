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
        private string _end;

        private string _id;

        private string _start;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
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
        public string End
        {
            get { return this._end; }
            set { this._end = value; }
        }
    }

}