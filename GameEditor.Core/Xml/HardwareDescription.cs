using GameEditor.Core.Hardware;
using System;
using System.CodeDom.Compiler;
using System.Collections;
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
    public class HardwareDescription
    {
        private List<MemoryMap> _memoryMaps;

        /// <remarks />
        [ XmlElement( "MemoryMap" ) ]
        public List<MemoryMap> MemoryMaps
        {
            get { return this._memoryMaps; }
            set { this._memoryMaps = value; }
        }

    }
}
