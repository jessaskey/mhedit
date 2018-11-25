using System.IO;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    internal class HavocBinaryWriter : BinaryWriter
    {
        private readonly Encoding _encoding;

        public HavocBinaryWriter( Stream stream )
            : this( stream, new HavocStringEncoding() )
        { }

        private HavocBinaryWriter( Stream stream, Encoding encoding )
            : base( stream, encoding )
        {
            this._encoding = encoding;
        }

        public override void Write( string value )
        {
            /// Don't call into base class as BinaryWriter will insert
            /// encoded string length at the start of the string.

            /// Not positive but are all strings prefixed with 0xA3? NO!
            /// Move past the unknown byte at the start of every string.
            this.BaseStream.Position++;

            char[] chars = value.Trim().ToUpper().ToCharArray();

            /// Should be calling into GetCharCount() but this encoding
            /// is 1 to 1.
            byte[] bytes = new byte[ chars.Length ];

            int encodedLength = 
                this._encoding.GetBytes( chars, 0, chars.Length, bytes, 0 );

            /// terminate string with the sign bit.
            bytes[ bytes.Length - 1 ] += 0x80;

            this.BaseStream.Write( bytes, 0, bytes.Length );
        }
    }
}
