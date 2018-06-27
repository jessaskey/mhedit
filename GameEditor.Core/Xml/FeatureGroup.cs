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
        private string _id;

        private bool _isDefault;

        private bool _isDefaultSpecified;

        private object[] _items;

        /// <remarks />
        [ XmlElement( "FeatureGroupRef", typeof( FeatureGroupRef ) ) ]
        [ XmlElement( "FeatureRef", typeof( FeatureRef ) ) ]
        public object[] Items
        {
            get { return this._items; }
            set { this._items = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public bool IsDefault
        {
            get { return this._isDefault; }
            set { this._isDefault = value; }
        }

        /// <remarks />
        [ XmlIgnore ]
        public bool IsDefaultSpecified
        {
            get { return this._isDefaultSpecified; }
            set { this._isDefaultSpecified = value; }
        }
    }

}