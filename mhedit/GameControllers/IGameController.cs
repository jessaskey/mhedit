using mhedit.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.GameControllers
{
    public interface IGameController
    {

        byte[] GetBytesFromString(string text);

        Tuple<ushort, int> GetAddress(string location);

        bool WriteFiles();

        bool SerializeObjects(Maze maze);
    }
}
