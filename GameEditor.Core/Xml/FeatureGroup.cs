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
    public class FeatureGroup
    {
        private string _idField;

        private bool _isDefaultField;

        private bool _isDefaultFieldSpecified;

        private object[] _itemsField;

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
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public bool IsDefault
        {
            get { return this._isDefaultField; }
            set { this._isDefaultField = value; }
        }

        /// <remarks />
        [ XmlIgnore ]
        public bool IsDefaultSpecified
        {
            get { return this._isDefaultFieldSpecified; }
            set { this._isDefaultFieldSpecified = value; }
        }
    }

}