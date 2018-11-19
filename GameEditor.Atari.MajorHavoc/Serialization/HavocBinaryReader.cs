using System.IO;
using System.Text;

namespace GameEditor.Atari.MajorHavoc
{
    internal class HavocBinaryReader : BinaryReader
    {
        public HavocBinaryReader( Stream stream )
            : base( stream, new HavocStringEncoding() )
        { }
    }

}
