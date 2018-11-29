using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEditor.Core.Serialization
{
    internal class RomWriter : BinaryWriter
    {
        private readonly Encoding _encoding;
        private readonly IStringEncoding _iStringEncoding;

        public RomWriter( Stream stream, Encoding encoding )
            : base( stream, encoding, true )
        {
            this._encoding = encoding;

            this._iStringEncoding = encoding as IStringEncoding;
        }

        public override void Write( string value )
        {
            /// Don't call into base class as BinaryWriter will insert
            /// encoded string length at the start of the string.
            if ( this._iStringEncoding == null )
            {
                base.Write( value );
            }
            else
            {
                this._iStringEncoding.Write( this.BaseStream, value );
            }
        }
    }
}
