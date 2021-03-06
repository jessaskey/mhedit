﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.GameControllers
{
    public class ObjectEncoding
    {
        public String SourceMacro { get; set; }
        public String Group { get; set; }
        public List<byte> Bytes { get; set; }
        public String Comment { get; set; }

        public ObjectEncoding()
            : this( new List<byte>() )
        {}

        public ObjectEncoding( List<byte> bytes )
        {
            Bytes = bytes;
        }

    }
}
