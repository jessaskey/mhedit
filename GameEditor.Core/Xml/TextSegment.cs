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
    public class TextSegment
    {
        private List<Checksum> _checksumField;

        private List<Data> _dataField;

        /// <remarks />
        [ XmlElement( "Data" ) ]
        public List<Data> Data
        {
            get { return this._dataField; }
            set { this._dataField = value; }
        }

        /// <remarks />
        [ XmlElement( "Checksum" ) ]
        public List<Checksum> Checksum
        {
            get { return this._checksumField; }
            set { this._checksumField = value; }
        }
    }

}