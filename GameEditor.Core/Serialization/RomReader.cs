using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEditor.Core.Serialization
{
    internal class RomReader : BinaryReader
    {
        private readonly Encoding _encoding;
        private readonly IStringEncoding _iStringEncoding;

        public RomReader( Stream stream, Encoding encoding )
            : base( stream, encoding, true )
        {
            this._encoding = encoding;

            this._iStringEncoding = encoding as IStringEncoding;
        }

        public override string ReadString()
        {
            /// Don't call into base class as BinaryWriter will insert
            /// encoded string length at the start of the string.
            if ( this._iStringEncoding == null )
            {
                return base.ReadString();
            }
            else
            {
                return this._iStringEncoding.ReadString( this.BaseStream );
            }
        }
    }
}
