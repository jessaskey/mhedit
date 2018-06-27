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
        private List<Level> _level;

        private int _maximum = -1;

        private int _minimum = 0;

        /// <remarks />
        [ XmlElement( "Level" ) ]
        public List<Level> Level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( -1 ) ]
        public int Maximum
        {
            get { return this._maximum; }
            set { this._maximum = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 1 ) ]
        public int Minimum
        {
            get { return this._minimum; }
            set { this._minimum = value; }
        }
    }

}