using GameEditor.Core.Xml;
using System;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// A MemoryBank class that encapsulates multiple IMemoryDevices and manages access
    /// to the data in them to maintain proper representation with respect to the DataBus.
    /// </summary>
    internal class MultipleDeviceBank : MemoryBank
    {
        public MultipleDeviceBank( Bank description, int dataBusWidth )
            : base( description, dataBusWidth )
        {}

        public override int Read( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            /// validate address
            if ( 0 > address || address > this.Size )
            {
                throw new ArgumentOutOfRangeException( string.Format(
                    "Read address {0} is outside of bank {1}.",
                    address, this.Id ) );
            }

            ///// Convert the byte count into the number of read operations by
            ///// dividing the bytes requested by the byte width of the bank.
            ///// This forces a min read length of the bank width.
            //int numberOfBankReads = words / ( this.Width / 8 );

            /// Truncate to length of bank.
            int numberOfBankReads = address + words > this.Size ?
                (int)( this.Size - address ) :
                words;

            int bytesRead = 0;

            do
            {
                /// With multiple devices in a bank, each device must be read at the same
                /// memory address and interlaced into the buffer to be in serialized order.
                foreach ( IMemoryDevice device in this._devices )
                {
                    bytesRead += device.Read( address, buffer, ( offset + bytesRead ) );
                }

                /// move to next address.
                address++;
            }
            while ( --numberOfBankReads > 0 );

            return bytesRead;
        }

        public override int Write( uint address, byte[] buffer, int offset = 0, int words = 1 )
        {
            /// validate address
            if ( 0 > address || address > this.Size )
            {
                throw new ArgumentOutOfRangeException( string.Format(
                    "Write address {0} is outside of bank {1}.",
                    address, this.Id ) );
            }

            ///// Convert the byte count into the number of read operations by
            ///// dividing the bytes requested by the byte width of the bank.
            ///// This forces a min read length of the bank width.
            //int numberOfBankWrites = words / ( this.Width / 8 );

            /// Truncate to length of bank.
            int numberOfBankWrites = address + words > this.Size ?
                (int)( this.Size - address ) :
                words;

            int bytesWritten = 0;

            do
            {
                /// With multiple devices in a bank, each device must be read at the same
                /// memory address and interlaced into the buffer to be in serialized order.
                foreach ( IMemoryDevice device in this._devices )
                {
                    bytesWritten += device.Read( address, buffer, ( offset + bytesWritten ) );
                }

                /// move to next address.
                address++;
            }
            while ( --numberOfBankWrites > 0 );

            return bytesWritten;
        }
    }
}
