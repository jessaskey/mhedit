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
    public class Level
    {
        private object[] _itemsField;

        private string _nameField;

        /// <remarks />
        [ XmlElement( "FeatureGroupRef", typeof( FeatureGroupRef ) ) ]
        [ XmlElement( "FeatureRef", typeof( FeatureRef ) ) ]
        public object[] Items
        {
            get { return this._itemsField; }
            set { this._itemsField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Name
        {
            get { return this._nameField; }
            set { this._nameField = value; }
        }
    }

}