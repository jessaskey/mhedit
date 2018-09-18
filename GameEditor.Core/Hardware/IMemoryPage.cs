using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// MemoryPages deal with redirecting data reads and writes to the proper address
    /// space in the MemoryBanks of the segment.
    /// </summary>
    public interface IMemoryPage
    {
        /// <summary>
        /// A unique id for this Device.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The IMemorySegment that this IMemoryPage exists in.
        /// </summary>
        IMemorySegment Segment { get; }

        /// <summary>
        /// This is the base address of the Page. This address is defined
        /// by the Memory Map of the system.
        /// </summary>
        //uint BaseAddress { get; }

        /// <summary>
        /// The size of the page's address space. The BaseAddress plus the 
        /// Size provides the upper address limit of this Page.
        /// </summary>
        //int Size { get; }

        /// <summary>
        /// Reads a sequence of bytes from the Page into the provided buffer.
        /// </summary>
        /// <param name="address">The address, relative to the page, of the first
        /// word to be read from the Page.</param>
        /// <param name="buffer">When this method returns, the buffer contains the
        /// the stream of bytes stored in the Page starting at the address.</param>
        /// <param name="offset">The zero-based byte offset into the buffer at which
        /// to begin storing the byte stream.</param>
        /// <param name="count">The number of bytes of data to be read out of the
        /// Page.</param>
        /// <returns>The number of bytes read from the Page.</returns>
        int Read( uint address, byte[] buffer, int offset = 0, int count = 1 );

        /// <summary>
        /// Writes a sequence of bytes from the provided buffer into the Page.
        /// </summary>
        /// <param name="address">The address, relative to the page, of the first
        /// word will be written into the Page.</param>
        /// <param name="buffer">When this method returns, the buffer contents will
        /// have been written to the Page starting at address.</param>
        /// <param name="offset">The zero-based byte offset into the buffer where the
        /// byte stream to be written begins.</param>
        /// <param name="count">The number of bytes of data to be written to the
        /// Page.</param>
        /// <returns>The number of bytes written to the Page.</returns>
        int Write( uint address, byte[] buffer, int offset = 0, int count = 1 );

    }
}
