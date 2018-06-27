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
    public class Levels
    {
        private List<Level> _levelField;

        private int _maximumField = -1;

        private int _minimumField = 0;

        /// <remarks />
        [ XmlElement( "Level" ) ]
        public List<Level> Level
        {
            get { return this._levelField; }
            set { this._levelField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( -1 ) ]
        public int Maximum
        {
            get { return this._maximumField; }
            set { this._maximumField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 1 ) ]
        public int Minimum
        {
            get { return this._minimumField; }
            set { this._minimumField = value; }
        }
    }

}