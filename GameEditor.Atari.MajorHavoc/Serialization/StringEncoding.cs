using GameEditor.Core;
using GameEditor.Core.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    /// <summary>
    /// This is the string manipulation code for Major Havoc. We shall see if it
    /// is used on other Atari games.
    /// </summary>
    internal class StringEncoding : Encoding, IStringEncoding
    {
        /// <summary>
        /// Valid characters. Note: I took a guess that the 2nd '.' in the original
        /// char list was the '©'. That makes the dictionaries happy because no
        /// duplicate key.
        /// </summary>
        private static readonly string ValidCharacterString =
            " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ.©!-,%:";

        private static readonly Dictionary<int, char> BytetoCharEncoding =
            new Dictionary<int, char>();

        private static readonly Dictionary<char, byte> CharToByteEncoding =
            new Dictionary<char, byte>();

        static StringEncoding()
        {
            /// create hash tables for the encodings
            for( int i = 0; i < ValidCharacterString.Length; ++i )
            {
                BytetoCharEncoding.Add( i * 2, ValidCharacterString[ i ] );
                CharToByteEncoding.Add( ValidCharacterString[ i ], (byte)( i * 2 ) );
            }
        }

        /// <summary>
        /// Throw if the character can't be encoded into a byte, and replace a
        /// byte with '?' if it can't be decoded.
        /// </summary>
        public StringEncoding()
            :base( 0, new EncoderExceptionFallback(), new DecoderReplacementFallback())
        { }

        public override string EncodingName { get { return "Atari, Major Havoc"; } }

        public override int GetByteCount( char[] chars, int index, int count )
        {
            if ( chars == null )
            {
                throw new ArgumentNullException( nameof( chars ) );
            }

            if ( index < 0 || count < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    index < 0 ? nameof( index ) : nameof( count ),
                    "Expected non negative value." );
            }

            if ( chars.Length - index < count )
            {
                throw new ArgumentOutOfRangeException( nameof( chars ),
                    "Index and count exceed buffer." );
            }

            return count;
        }

        public override int GetBytes( char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex )
        {
            if ( chars == null || bytes == null )
            {
                throw new ArgumentNullException(
                    chars == null ? nameof( chars ) : nameof( bytes ));
            }

            if ( charIndex < 0 || charCount < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    charIndex < 0 ? nameof( charIndex ) : nameof( charCount ),
                    "Expected non negative value." );
            }

            if ( chars.Length - charIndex < charCount )
            {
                throw new ArgumentOutOfRangeException( nameof( chars ),
                    "Index and count exceed buffer." );
            }

            if ( byteIndex < 0 || byteIndex > bytes.Length )
            {
                throw new ArgumentOutOfRangeException( nameof( byteIndex ),
                    "Index exceeds buffer." );
            }

            for ( int i = 0; i < charCount; ++i )
            {
                bytes[ byteIndex + i ] = EncodeChar( chars[ charIndex + i ] );
            }

            return charCount;
        }

        public override int GetCharCount( byte[] bytes, int index, int count )
        {
            if ( bytes == null )
            {
                throw new ArgumentNullException( nameof( bytes ) );
            }

            if ( index < 0 || count < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    index < 0 ? nameof( index ) : nameof( count ),
                    "Expected non negative value." );
            }

            if ( bytes.Length - index < count )
            {
                throw new ArgumentOutOfRangeException( nameof( bytes ),
                    "Index and count exceed buffer." );
            }

            return count;
        }

        public override int GetChars( byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex )
        {
            if ( chars == null || bytes == null )
            {
                throw new ArgumentNullException(
                    chars == null ? nameof( chars ) : nameof( bytes ) );
            }

            if ( byteIndex < 0 || byteCount < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    byteIndex < 0 ? nameof( byteIndex ) : nameof( byteCount ),
                    "Expected non negative value." );
            }

            if ( bytes.Length - byteIndex < byteCount )
            {
                throw new ArgumentOutOfRangeException( nameof( bytes ),
                    "Index and count exceed buffer." );
            }

            if ( charIndex < 0 || charIndex > chars.Length )
            {
                throw new ArgumentOutOfRangeException( nameof( charIndex ),
                    "Index exceeds buffer." );
            }

            for ( int i = 0; i < byteCount; ++i )
            {
                chars[ i + charIndex ] = DecodeByte( bytes[ i + byteIndex ] );
            }

            return byteCount;
        }

        public override int GetMaxByteCount( int charCount )
        {
            if ( charCount < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    nameof( charCount ),
                    "Expected non negative value." );
            }

            return charCount;
        }

        public override int GetMaxCharCount( int byteCount )
        {
            if ( byteCount < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    nameof( byteCount ),
                    "Expected non negative value." );
            }

            return byteCount;
        }

        public string ReadString( Stream stream )
        {
            /// Not positive but are all strings prefixed with 0xA3? NO!
            /// Move past the unknown byte at the start of every string.
            long savedPosition = stream.Position++;

            /// Read strings in 32 byte chunks and adjust to actual length
            char[] chars = new char[ 32 ];
            byte[] bytes = new byte[ chars.Length ];

            StringBuilder builder = new StringBuilder();

            int lastChar;

            do
            {
                int bytesRead = stream.Read( bytes, 0, bytes.Length );

                /// look for the sign bit indicating the last char.
                lastChar = Array.FindIndex( bytes, 0, bytesRead, b => ( b & 0x80 ) != 0 );

                builder.Append( chars, 0,
                    this.GetChars(
                        bytes, 0, lastChar < 0 ? bytes.Length : lastChar + 1, chars, 0 )
                    );
            }
            while ( lastChar == -1 );

            /// Move stream to after last char.
            stream.Position = savedPosition + builder.Length;

            return builder.ToString();
        }

        public void Write( Stream stream, string value )
        {
            /// Not positive but are all strings prefixed with 0xA3? NO!
            /// Move past the unknown byte at the start of every string.
            stream.Position++;

            char[] chars = value.Trim().ToUpper().ToCharArray();

            /// Should be calling into GetCharCount() but this encoding
            /// is 1 to 1.
            byte[] bytes = new byte[ chars.Length ];

            int encodedLength =
                this.GetBytes( chars, 0, chars.Length, bytes, 0 );

            /// terminate string with the sign bit.
            bytes[ bytes.Length - 1 ] += 0x80;

            stream.Write( bytes, 0, bytes.Length );
        }

        private static char DecodeByte( byte aByte )
        {
            return BytetoCharEncoding[ aByte & 0x7f ];
        }

        private static byte EncodeChar( char aChar )
        {
            return CharToByteEncoding[ aChar ];
        }
    }
}
