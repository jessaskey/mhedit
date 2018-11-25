using GameEditor.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    /// <summary>
    /// This is the string manipulation code for Major Havoc. We shall see if it
    /// is used on other Atari games.
    /// </summary>
    internal class HavocStringEncoding : Encoding
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

        static HavocStringEncoding()
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
        public HavocStringEncoding()
            :base( 0, new EncoderExceptionFallback(), new DecoderReplacementFallback())
        { }

        public static char DecodeByte( byte aByte )
        {
            return BytetoCharEncoding[ aByte & 0x7f ];
        }

        public static byte EncodeChar( char aChar )
        {
            return CharToByteEncoding[ aChar ];
        }

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
    }
}
