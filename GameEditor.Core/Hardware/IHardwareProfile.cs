using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Hardware
{
    public interface IHardwareDescription
    {
        IList<IMemoryMap> MemoryMaps { get; }

        /// <summary>
        /// Return a MemoryPageStream for the page reference provided.
        /// </summary>
        /// <param name="pageRef"></param>
        /// <returns>A MemoryPageStream for reading or writing to the page.</returns>
        MemoryPageStream FindPageStream( Xml.PageRef pageRef );

        /// <summary>
        /// Get a MemoryPage object from the Map based upon a reference.
        /// </summary>
        /// <param name="pageRef"></param>
        /// <returns>The Memory Page referenced.</returns>
        /// <throws></throws>
        IMemoryPage FindMemoryPage( Xml.PageRef pageRef );
    }
}
