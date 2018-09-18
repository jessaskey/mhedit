using System.Collections.Generic;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// A Memory Bank aggregates a set of IMemoryDevices together so that their
    /// total width is that of the Memory Map's data bus. IMemoryDevices need not
    /// be the same Width but must all be the same Size.
    /// </summary>
    public interface IMemoryBank
    {
        /// <summary>
        /// A unique id for this Bank.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The Size of the Device, or how many Data Words the device holds. This
        /// value implies the width of the Device's address bus.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// The Width of the Banks's Data bus in bits.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The set of IMemoryBanks that are grouped together to form a Bank. The
        /// list is sorted least to most significant device with respect to forming
        /// the Data Word.
        /// </summary>
        IList<IMemoryDevice> Devices { get; }

        /// <summary>
        /// Reads a sequence of bytes from the Bank into the provided buffer. As some
        /// Banks are wider than a byte, the number of bytes returned is dependent
        /// upon many factors including the <see cref="Width"/> of the Bank and the
        /// Width of the IMemoryDevices included within. Access to bytes within the
        /// Bank data word are accomplished as a read of a full data word and the
        /// byte of interest selected from within.
        /// </summary>
        /// <param name="address">The zero-based address, relative to the Bank,
        /// of the first word of data to be read from the Bank.</param>
        /// <param name="buffer">When this method returns, the buffer contains the
        /// stream of bytes stored in the Bank starting from address.</param>
        /// <param name="offset">The zero-based byte offset into the buffer at which
        /// to begin storing the byte stream.</param>
        /// <param name="words">The maximum number of bytes to be read from the
        /// Bank.</param>
        /// <returns>The total number of bytes read from the Bank. For Banks where
        /// the <see cref="Width"/> is greater than a byte the number of bytes returned
        /// will be a multiple of the width. For example, a 16-bit Bank results in a
        /// 2 byte read per requested. It may also be less than requested if the
        /// address and count exceed the size of the Bank, or zero (0) if the address
        /// is beyond the <see cref="Size"/> of the Bank.  </returns>
        int Read( uint address, byte[] buffer, int offset = 0, int words = 1 );

        /// <summary>
        /// Writes a sequence of bytes to the Bank from the provided buffer. As some
        /// devices are wider than a byte, the number of bytes written is dependent
        /// upon many factors including the <see cref="Width"/> of the Bank and the
        /// Width of the IMemoryDevices included within. Access to bytes within the
        /// Bank data word must be performed as a read with a logical OR and
        /// subsequent write.
        /// </summary>
        /// <param name="address">The zero-based address, relative to the Bank,
        /// of the first word of data to be written to the Bank.</param>
        /// <param name="buffer">When this method returns, the buffer contents will
        /// have been written to the Bank starting at the address and ending at 
        /// Address + Count.</param>
        /// <param name="offset">The zero-based byte offset into the buffer where the
        /// byte stream to be written begins.</param>
        /// <param name="words">The maximum number of data words to be written to the
        /// Bank.</param>
        /// <returns>The total number of bytes written into the buffer. For Banks where
        /// the <see cref="Width"/> is greater than a byte the number of bytes written
        /// will be a multiple of the width. For example, a 16-bit Bank results in a
        /// 2 byte write per requested. It may also be less than requested if the
        /// address and count exceed the size of the Bank, or zero (0) if the address
        /// is beyond the <see cref="Size"/> of the Bank.  </returns>
        int Write( uint address, byte[] buffer, int offset = 0, int words = 1 );
    }
}
