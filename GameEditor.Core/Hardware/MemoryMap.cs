using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GameEditor.Core.Hardware
{
    public class MemoryMap : IMemoryMap
    {
        private readonly Xml.MemoryMap _description;
        private readonly List<IMemorySegment> _segments = new List<IMemorySegment>();

        public MemoryMap( Xml.MemoryMap description )
        {
            this._description = description;

            foreach ( Xml.Rom rom in description.Roms )
            {
                this._segments.Add( new MemorySegment( this, rom ) );
            }
        }

        public string Id
        {
            get
            {
                return this._description.Id;
            }
        }

        public int AddressWidth
        {
            get
            {
                return this._description.AddressWidth;
            }
        }

        public uint AddressBase
        {
            get
            {
                return Convert.ToUInt32( this._description.AddressBase, 16 );
            }
        }

        public int DataWidth
        {
            get
            {
                return this._description.DataWidth;
            }
        }

        public EndianType Endianness
        {
            get
            {
                return this._description.Endianness;
            }
        }

        public IList<IMemorySegment> Segments
        {
            get
            {
                return this._segments.AsReadOnly();
            }   
        }
    }

}
