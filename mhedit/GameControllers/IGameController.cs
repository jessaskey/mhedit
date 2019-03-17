using mhedit.Containers;
using System;
using System.Collections.Generic;

namespace mhedit.GameControllers
{
    public interface IGameController : IName
    {

        byte[] GetBytesFromString(string text);

        Tuple<ushort, int> GetAddress(string location);

        bool WriteFiles(string mamePath);

        bool EncodeObjects(MazeCollection collection, Maze maze);

        byte ReadByte(ushort address, int offset);

        MazeCollection LoadMazes(string sourceFilePath, List<string> loadMessages);

        string LastError { get; }
    }
}
