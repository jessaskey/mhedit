using GameEditor.Core.Extensions;
using GameEditor.Core.Hardware;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameEditor.Core.Hardware
{

    internal class MemoryPage : IMemoryPage
    {
        private readonly Xml.Page _description;
        private readonly IMemorySegment _segment;
        private readonly List<BankReference> _bankReferences = new List<BankReference>();

        public MemoryPage( IMemorySegment segment, Xml.Page description )
        {
            this._segment = segment;

            this._description = description;
        }

        public string Id
        {
            get
            {
                return this._description.Id;
            }
        }

        public IMemorySegment Segment
        {
            get
            {
                return this._segment;
            }
        }

        public IList<BankReference> BankReferences
        {
            get
            {
                return this._bankReferences;
            }
        }

        public int Read( uint address, byte[] buffer, int offset = 0, int count = 1 )
        {
            /// Make address zero-oriented (relative) to this page.
            address -= this.Segment.Address;

            int bytesRead = 0;

            using ( IEnumerator<BankReference> enumerator =
                                this._bankReferences.GetEnumerator() )
            {
                while ( count > 0 && enumerator.MoveNext() )
                {
                    bytesRead = 
                        enumerator.Current.Read( address, buffer, offset, count );

                    offset += bytesRead;

                    count -= bytesRead;

                    /// I need to make an address object or work out some way to
                    /// configure the addressable width of the hardware. I could have
                    /// a 32-bit data bus but address resolution of a byte controlled
                    /// by the lower 2 bits of the address bus... 
                    address += (uint)( bytesRead * ( this.Segment.Map.DataWidth / 8 ) );
                }
            }

            return bytesRead;
        }

        public int Write( uint address, byte[] buffer, int offset = 0, int count = 1 )
        {
            /// Make address zero-oriented (relative) to this page.
            address -= this.Segment.Address;

            int bytesWritten = 0;

            using ( IEnumerator<BankReference> enumerator =
                                this._bankReferences.GetEnumerator() )
            {
                while ( count > 0 && enumerator.MoveNext() )
                {
                    bytesWritten = 
                        enumerator.Current.Write( address, buffer, offset, count );

                    offset += bytesWritten;

                    count -= bytesWritten;

                    /// I need to make an address object or work out some way to
                    /// configure the addressable width of the hardware. I could have
                    /// a 32-bit data bus but address resolution of a byte controlled
                    /// by the lower 2 bits of the address bus... 
                    address += (uint)( bytesWritten * ( this.Segment.Map.DataWidth / 8 ) );
                }
            }

            /// I think we want this yes? Maybe up at the stream level though.
            if ( count != 0 )
            {
                throw new DataMisalignedException( string.Format(
                    "Failed to write complete buffer to page {0}", this.Id ) );
            }

            return bytesWritten;
        }
    }
}