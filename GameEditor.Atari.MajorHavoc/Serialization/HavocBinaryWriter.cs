using System.IO;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    internal class HavocBinaryWriter : BinaryWriter
    {
        public HavocBinaryWriter( Stream stream )
            : base( stream, new HavocStringEncoding() )
        {}
    }

}
