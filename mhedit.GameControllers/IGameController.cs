﻿using mhedit.Containers;
using System;
using System.Collections.Generic;

namespace mhedit.GameControllers
{
    public interface IGameController : IName
    {

        bool LoadTemplate(string sourceRomPath);

        byte[] GetBytesFromString(string text);

        ushort GetAddress(string location);

        bool WriteFiles(string mamePath);

        bool EncodeObjects(MazeCollection collection, Maze maze);

        byte ReadByte(ushort address, int offset);

        MazeCollection LoadMazes(List<string> loadMessages);

        string LastError { get; }
    }
}
