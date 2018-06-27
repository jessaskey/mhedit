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
    public class ProgramMemory
    {
        private TextSegment _textSegmentField;

        /// <remarks />
        public TextSegment TextSegment
        {
            get { return this._textSegmentField; }
            set { this._textSegmentField = value; }
        }
    }

}