using mhedit.Containers;
using System.Collections.Generic;

namespace mhedit.GameControllers
{
    public interface IGameController : IName
    {

        bool LoadTemplate(string sourceRomPath);

        byte[] GetBytesFromString(string text);

        bool WriteFiles(string mamePath, string driverName);

        bool EncodeObjects( MazeCollection mazeCollection );

        /// <summary>
        /// This is a kludge until I separate concerns of serialization
        /// to ROM and outside changes for MAME.
        /// </summary>
        /// <param name="mazeToStartOn"></param>
        void SetStartingMaze( int mazeToStartOn = 0 );

        MazeCollection LoadMazes(List<string> loadMessages);

        string LastError { get; }
    }
}
