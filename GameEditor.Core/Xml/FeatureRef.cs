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
    [ XmlRoot( Namespace = "http://askey.org/GameProfile", IsNullable = false ) ]
    public class FeatureRef
    {
        private string _id;

        private int _maximum = -1;

        private int _minimum = 0;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
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
        [ DefaultValue( 0 ) ]
        public int Minimum
        {
            get { return this._minimum; }
            set { this._minimum = value; }
        }
    }

}