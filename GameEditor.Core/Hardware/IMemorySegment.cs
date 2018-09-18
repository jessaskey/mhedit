using System.Collections.Generic;

namespace GameEditor.Core.Hardware
{
    /// <summary>
    /// MemorySegments deal with properly defining the memory space used by the
    /// processor. Here we might deal with special addressing that selects a
    /// specific page.
    /// </summary>
    public interface IMemorySegment
    {
        /// <summary>
        /// A unique id for this Bank.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Thhe address of the first word of Memory in the Segment. This address
        /// is defined by the Memory Map of the system. All pages in this segment
        /// share this Address as their base.
        /// </summary>
        uint Address { get; }

        /// <summary>
        /// The size of the Segments's address space. The Address plus the Size
        /// provides the upper address limit of this Segment.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        IMemoryMap Map { get; }

        /// <summary>
        /// The collection of Memory Banks that make up the physical memory in this
        /// Segment.
        /// </summary>
        IList<IMemoryBank> Banks { get; }

        /// <summary>
        /// The collection of Memory Pages that make up the virtual memory in this
        /// Segment.
        /// </summary>
        IList<IMemoryPage> Pages { get; }
    }
}
