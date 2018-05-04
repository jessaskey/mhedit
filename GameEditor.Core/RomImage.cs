using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core
{
    public class RomImage
    {
        private BinaryReader _file;
        private int _baseAddress;

        public RomImage( string filePath, int baseAddress )
        {
            this._baseAddress = baseAddress;

            this._file = new BinaryReader(
                File.Open( filePath, FileMode.Open, FileAccess.Read ) );
        }

        byte Byte()
        {
            return this._file.ReadByte();
        }

        byte Byte( int address )
        {
            this._file.BaseStream.Position = address - this._baseAddress;

            return this.Byte();
        }
    }
}
