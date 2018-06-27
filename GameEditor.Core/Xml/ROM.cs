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
    public class Rom
    {
        private string _addressField;

        private string _idField;

        private List<Page> _pagesField;

        private string _sizeField;

        private List<Bank> _banksField;

        /// <remarks />
        [ XmlArrayItem( "Bank", IsNullable = false ) ]
        public List<Bank> Banks
        {
            get { return this._banksField; }
            set { this._banksField = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "Page", typeof( Page ), IsNullable = false ) ]
        public List<Page> Pages
        {
            get { return this._pagesField; }
            set { this._pagesField = value; }
        }

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._idField; }
            set { this._idField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Address
        {
            get { return this._addressField; }
            set { this._addressField = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Size
        {
            get { return this._sizeField; }
            set { this._sizeField = value; }
        }
    }

}