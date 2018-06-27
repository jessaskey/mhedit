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
    public class Level
    {
        private List<object> _features;

        private string _name;

        /// <remarks />
        [ XmlElement( "FeatureGroupRef", typeof( FeatureGroupRef ) ) ]
        [ XmlElement( "FeatureRef", typeof( FeatureRef ) ) ]
        public List<object> Features
        {
            get { return this._features; }
            set { this._features = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
    }

}