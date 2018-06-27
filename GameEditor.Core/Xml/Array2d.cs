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
    public class Array2D : Array
    {
        private int _size2Field = 1;

        /// <remarks />
        [ XmlAttribute ]
        [ DefaultValue( 1 ) ]
        public int Size2
        {
            get { return this._size2Field; }
            set { this._size2Field = value; }
        }
    }

}