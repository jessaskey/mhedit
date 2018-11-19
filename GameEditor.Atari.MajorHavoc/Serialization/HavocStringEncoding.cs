using System;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    internal class HavocStringEncoding : Encoding
    {
        public override int GetByteCount( char[] chars, int index, int count )
        {
            throw new NotImplementedException();
        }

        public override int GetBytes( char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex )
        {
            throw new NotImplementedException();
        }

        public override int GetCharCount( byte[] bytes, int index, int count )
        {
            throw new NotImplementedException();
        }

        public override int GetChars( byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex )
        {
            throw new NotImplementedException();
        }

        public override int GetMaxByteCount( int charCount )
        {
            return charCount;
        }

        public override int GetMaxCharCount( int byteCount )
        {
            /// TODO: Validate a byte per char in MH code.
            return byteCount;
        }
    }

}
