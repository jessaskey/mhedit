using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// Represents a MemoryDevice, such as an EPROM that stores data or code for a game.
    /// Devices don't concern themselves with endian, they just store bytes of data in
    /// address order and are accessed in groups of bytes based upon the devices
    /// <see cref="Width"/>.
    /// </summary>
    public interface IMemoryDevice
    {
        /// <summary>
        /// A unique id for this Device.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The fully qualified path to the file that represents this Device's image.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The Size of the Device, or how many Data Words the device holds. This
        /// value implies the width of the Device's address bus.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// The Width of the Devices's Data bus in bits.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Reload the Device image from the original file. See the <see cref="Path"/>
        /// property. This will discard any changes to the image since the last
        /// <see cref="Write"/>.
        /// </summary>
        //void Reload();

        /// <summary>
        /// Write the existing image out to the file at the specified. The original file
        /// designated by <see cref="Path"/> can't be overwritten.
        /// </summary>
        /// <param name="path">Fully qualified path to the file to be overwritten.</param>
        /// <param name="overwrite">When true, overwrites any existing file except the
        /// file specified by the <see cref="Path"/>.</param>
        //void Write( Path path, bool overwrite );

        /// <summary>
        /// Reads a sequence of bytes from the Device into the provided buffer. As some
        /// Devices are wider than a byte, the number of bytes returned is dependent
        /// upon the <see cref="Width"/> of the Device.  Access to bytes within the
        /// Device data word are accomplished as a read of a full data word and the
        /// byte of interest selected from within.
        /// </summary>
        /// <param name="address">The zero-based address, relative to the Device,
        /// of the first word of data to be read from the Device.</param>
        /// <param name="buffer">When this method returns, the buffer contains the
        /// stream of bytes stored in the Device starting from address.</param>
        /// <param name="offset">The zero-based byte offset into the buffer at which
        /// to begin storing the byte stream.</param>
        /// <param name="words">The maximum number of data words to be read from the
        /// Device.</param>
        /// <returns>The total number of bytes read into the buffer. For Devices where
        /// the <see cref="Width"/> is greater than a byte the number of bytes returned
        /// will be a multiple of the width. For example, a 16-bit Device results in a
        /// 2 byte read per requested. It may also be less than requested if the
        /// address and count exceed the size of the Device, or zero (0) if the address
        /// is beyond the <see cref="Size"/> of the Device.  </returns>
        int Read( uint address, byte[] buffer, int offset = 0, int words = 1 );

        /// <summary>
        /// Writes a sequence of bytes to the Device from the provided buffer. As some
        /// Devices are wider than a byte, the number of bytes written is dependent
        /// upon the <see cref="Width"/> of the Device. Access to bytes within the
        /// Bank data word must be performed as a read with a logical OR and
        /// subsequent write.
        /// </summary>
        /// <param name="address">The zero-based address, relative to the Device,
        /// of the first word of data to be written to the Device.</param>
        /// <param name="buffer">When this method returns, the buffer contents will
        /// have been written to the Device starting at the address and ending at 
        /// Address + Count.</param>
        /// <param name="offset">The zero-based byte offset into the buffer where the
        /// byte stream to be written begins.</param>
        /// <param name="words">The maximum number of data words to be written to the
        /// Device.</param>
        /// <returns>The total number of bytes written into the buffer. For Devices where
        /// the <see cref="Width"/> is greater than a byte the number of bytes written
        /// will be a multiple of the width. For example, a 16-bit Device results in a
        /// 2 byte write per requested. It may also be less than requested if the
        /// address and count exceed the size of the Device, or zero (0) if the address
        /// is beyond the <see cref="Size"/> of the Device.  </returns>
        int Write( uint address, byte[] buffer, int offset = 0, int words = 1 );

    }
}
