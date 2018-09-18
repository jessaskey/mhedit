using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// Designed to provide stream like access to a memory page, where the
    /// Position property acts as the address to be read/written. Since streams
    /// manage position in bytes, this stream class must manage the address
    /// based upon the hardware design and properly increment/decrement based
    /// upon the byte width of a memory location. For memory segments that access
    /// memory in bytes this is straight forward, but if the memory is setup to
    /// as a 16-bit data word then each memory address represents 2 bytes. So
    /// each read/write of 2 bytes will increment the Position.
    /// 
    /// Currently only supporting 16-bit addressing.
    /// </summary>
    public class MemoryPageStream : Stream
    {
        private readonly IMemoryPage _page;
        private long _address;

        public MemoryPageStream( IMemoryPage page )
            : this( page, page.Segment.Address )
        {}

        public MemoryPageStream( IMemoryPage page, uint address )
        {
            this._page = page;

            this._address = address;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                return this._page.Segment.Size;
            }
        }

        public override long Position
        {
            get
            {
                return this._address;
            }

            set
            {
                /// don't allow the position to move outside the range of valid address
                /// space.
                if ( value < this._page.Segment.Address )
                {
                    throw new ArgumentOutOfRangeException( string.Format(
                        "An attempt was made to move the page address before the beginning" +
                        " of the {0} page", this._page.Id ) );
                }

                if ( value >= ( this._page.Segment.Address + this._page.Segment.Size ) )
                {
                    throw new ArgumentOutOfRangeException( string.Format(
                        "An attempt was made to move the page address past the end" +
                        " of the {0} page", this._page.Id ) );
                }

                this._address = value;
            }
        }

        public override void Flush()
        {
            /// nothing to do..
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            if ( buffer == null )
            {
                throw new ArgumentNullException(
                    "buffer", string.Format( "Read Page {0}", this._page.Id ) );
            }

            if ( offset < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    "offset", string.Format( "Read Page {0}", this._page.Id ) );
            }

            if ( count < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    "count", string.Format( "Read Page {0}", this._page.Id ) );
            }

            if ( ( offset + count ) > buffer.Length )
            {
                throw new ArgumentException( string.Format(
                    "Offset {0} + count {1} > buffer.Length {2} when reading Page {3}",
                    offset, count, buffer.Length, this._page.Id ), "offset" );
            }

            /// Only supporting 16 bit addressing ATM...
            int bytesRead = this._page.Read( (uint)this._address, buffer, offset, count );

            this.Seek( bytesRead, SeekOrigin.Current );

            return bytesRead;
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            /// convert the number of bytes of offset to a change in address.
            offset /= ( this._page.Segment.Map.DataWidth / 8 );

            /// Modify a local so that invalid values are not recorded in the object.
            long address = this._address;

            switch ( origin )
            {
                case SeekOrigin.Begin:
                    address = this._page.Segment.Address;
                    break;

                case SeekOrigin.Current:
                    break;

                case SeekOrigin.End:
                    address = this._page.Segment.Address + this._page.Segment.Size - 1;
                    break;
            }

            /// this will throw if the new position is invalid.
            this.Position = address += offset;

            return this.Position;
        }

        public override void SetLength( long value )
        {
            throw new NotSupportedException( string.Format(
                "SetLength( long ), Page {0}", this._page.Id ) );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            if ( buffer == null )
            {
                throw new ArgumentNullException(
                    "buffer", string.Format( "Write Page {0}", this._page.Id ) );
            }

            if ( offset < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    "offset", string.Format( "Write Page {0}", this._page.Id ) );
            }

            if ( count < 0 )
            {
                throw new ArgumentOutOfRangeException(
                    "count", string.Format( "Write Page {0}", this._page.Id ) );
            }

            if ( ( offset + count ) > buffer.Length )
            {
                throw new ArgumentException( string.Format(
                    "Offset {0} + count {1} > buffer.Length {2} when writing Page {3}",
                    offset, count, buffer.Length, this._page.Id ), "offset" );
            }

            /// Only supporting 16 bit addressing ATM...
            int bytesWritten = this._page.Read( (uint)this._address, buffer, offset, count );

            /// Should we be throwing if the write fails?? I'm trying to force a write to
            /// be the width of the databus, or it doesn't occur.
            /// Can't really throw if you write past the end of the page....Or can you?
            //if ( bytesWritten != count )
            //{
            //    throw new IOException( string.Format(
            //        "Write Failure page {0}", this._page.Id ) );
            //}

            this.Seek( bytesWritten, SeekOrigin.Current );
        }
    }
}
