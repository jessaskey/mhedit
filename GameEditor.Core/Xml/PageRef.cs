﻿using GameEditor.Core.Extensions;
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
    public class PageRef
    {
        private string _id;
        private int _address;

        /// <remarks />
        [ XmlAttribute ]
        public string Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        [ XmlIgnore ]
        public int Address
        {
            get { return this._address; }
            set { this._address = value; }
        }

        [ XmlText ]
        public string Value
        {
            get { return this._address.ToHex(); }
            set { this._address = value.ToInt(); }
        }
    }

}