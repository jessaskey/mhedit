using GameEditor.Core.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    internal class HavocBinaryReader : BinaryReader
    {
        private readonly Encoding _encoding;

        public HavocBinaryReader( Stream stream )
            : this( stream, new HavocStringEncoding() )
        { }

        private HavocBinaryReader( Stream stream, Encoding encoding )
            : base( stream, encoding )
        {
            this._encoding = encoding;
        }

        public override string ReadString()
        {
            /// Don't call into base class as BinaryReader expects the string
            /// length to be encoded at the start of the string.

            /// Not positive but are all strings prefixed with 0xA3? NO!
            /// Move past the unknown byte at the start of every string.
            long savedPosition = this.BaseStream.Position++;

            /// Read strings in 32 byte chunks and adjust to actual length
            char[] chars = new char[ 32 ];

            StringBuilder builder = new StringBuilder();

            int lastChar;

            do
            {
                byte[] stringBytes = this.ReadBytes( chars.Length );

                /// look for the sign bit indicating the last char.
                lastChar = Array.FindIndex( stringBytes, b => ( b & 0x80 ) != 0 );

                builder.Append( chars, 0,
                    this._encoding.GetChars(
                        stringBytes, 0, lastChar < 0 ? stringBytes.Length : lastChar + 1, chars, 0 )
                    );
            }
            while ( lastChar == -1 );

            /// Move stream to after last char.
            this.BaseStream.Position = savedPosition + builder.Length;

            return builder.ToString();
        }
    }
}