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
        private string _address;

        private string _id;

        private List<Page> _pages;

        private string _size;

        private List<Bank> _banks;

        /// <remarks />
        [ XmlArrayItem( "Bank", IsNullable = false ) ]
        public List<Bank> Banks
        {
            get { return this._banks; }
            set { this._banks = value; }
        }

        /// <remarks />
        [ XmlArrayItem( "Page", typeof( Page ), IsNullable = false ) ]
        public List<Page> Pages
        {
            get { return this._pages; }
            set { this._pages = value; }
        }

        /// <remarks />
        [ XmlAttribute( DataType = "ID" ) ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Address
        {
            get { return this._address; }
            set { this._address = value; }
        }

        /// <remarks />
        [ XmlAttribute ]
        public string Size
        {
            get { return this._size; }
            set { this._size = value; }
        }
    }

}