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
    [ XmlRoot( Namespace = "http://askey.org/GameProfile", IsNullable = false ) ]
    public class StorageDefinition
    {
        private List<Array> _arrayField;
        private string _idField;

        /// <remarks />
        [ XmlElement( "Array", typeof( Array ) ) ]
        [ XmlElement( "Array2d", typeof( Array2D ) ) ]
        public List<Array> Array
        {
            get { return this._arrayField; }
            set { this._arrayField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }
    }

}