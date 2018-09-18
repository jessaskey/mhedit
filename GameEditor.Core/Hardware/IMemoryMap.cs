using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEditor.Core.Hardware
{

    public interface IMemoryMap
    {
        string Id { get; }

        int AddressWidth { get; }

        uint AddressBase { get; }

        int DataWidth { get; }

        EndianType Endianness { get; }

        IList<IMemorySegment> Segments { get; }
    }
}
