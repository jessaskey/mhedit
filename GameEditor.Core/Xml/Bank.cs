using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
    public class Bank
    {
        private List<Eprom> _eproms;

        private string _id;

        /// <summary>
        /// This could probably renamed to "MemoryChips" or something since
        /// Ideally we could be describing RAM banks in the future.
        /// </summary>
        [ XmlElement( "EPROM" ) ]
        public List<Eprom> Eproms
        {
            get { return this._eproms; }
            set { this._eproms = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }
    }

}