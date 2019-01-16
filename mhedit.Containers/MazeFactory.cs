using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using mhedit.Containers.MazeObjects;
using System.Linq;

namespace mhedit.Containers
{

    public static class MazeFactory
    {
        public struct MazeBaseData
        {
            public List<MazeWall> mazeWallBase;
            public int mazeStampsX;
            public int mazeStampsY;
        }

        public static Color GetObjectColor(ObjectColor color)
        {
            switch (color)
            {
                case ObjectColor.Black:
                    return System.Drawing.Color.DarkGray;
                case ObjectColor.Blue:
                    return System.Drawing.Color.Blue;
                case ObjectColor.Bluer:
                    return System.Drawing.Color.BlueViolet;
                case ObjectColor.Cyan:
                    return System.Drawing.Color.Cyan;
                case ObjectColor.Cyanr:
                    return System.Drawing.Color.FromArgb(0x80, 0xFF, 0xFF);
                case ObjectColor.Green:
                    return System.Drawing.Color.FromArgb(0x00, 0xc0, 0x00);
                case ObjectColor.Greenr:
                    return System.Drawing.Color.FromArgb(0x80, 0xFF, 0x00);
                case ObjectColor.Orange:
                    return System.Drawing.Color.Orange;
                case ObjectColor.Pink:
                    return System.Drawing.Color.HotPink;
                case ObjectColor.Purple:
                    return System.Drawing.Color.FromArgb(0xd0, 0x00, 0xd0);
                case ObjectColor.Flash:
                    return System.Drawing.Color.Silver;
                case ObjectColor.Red2:
                    return System.Drawing.Color.FromArgb(0xe0, 0x00, 0x00);
                case ObjectColor.Red:
                    return System.Drawing.Color.FromArgb(0xFF, 0x00, 0x00);
                case ObjectColor.White:
                    return System.Drawing.Color.White;
                case ObjectColor.Whiter:
                    return System.Drawing.Color.FromArgb(0xFF, 0xC0, 0xC0);
                case ObjectColor.Yellow:
                    return System.Drawing.Color.Yellow;
            }
            return Color.Yellow;
        }
        public static MazeBaseData GetBaseMap(MazeType type)
        {
            int mazeStampsX = 0;
            int mazeStampsY = 0;
            List<MazeWall> mazeWallBase = new List<MazeWall>();
            switch (type)
            {
                case MazeType.TypeA:
                    mazeStampsX = 16;
                    mazeStampsY = 7;
                    //row1
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() { MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.Empty,MazeWallType.Empty,MazeWallType.LeftDown,MazeWallType.RightDown,
                                                                MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                                                                MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty}));

                    //row2
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() { MazeWallType.Empty, MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.RightUp,
                                                                MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,
                                                                MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty }));

                    //row3
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightDown,MazeWallType.Horizontal,
                                                                MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.LeftDown,MazeWallType.Vertical,
                                                                MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Empty }));

                    //row4
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Empty,
                                                                MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,
                                                                MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Empty }));
                    //row5
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.Horizontal,
                                                                MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.Horizontal,
                                                                MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Empty }));
                                                                
                    //row6
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,
                                                                MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightDown,
                                                                MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty }));
                    //row7
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>() { MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                                                                MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,
                                                                MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftUp,
                                                                MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty }));

                    break;
                case MazeType.TypeB:
                    mazeStampsX = 21;
                    mazeStampsY = 8;
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.RightDown,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row2
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,
                        MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row3
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.LeftUp,
                        MazeWallType.Horizontal,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.RightDown,MazeWallType.Empty,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.Empty
                        }));
                    //row4
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Empty
                        }));
                    //row5
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightDown,MazeWallType.LeftDown,
                        MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Empty
                        }));
                    //row6
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.Vertical,
                        MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Empty
                        }));
                    //row7
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightUp,
                        MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Empty
                        }));
                    //row8
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightDown,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    break;
                case MazeType.TypeC:
                    mazeStampsX = 21;
                    mazeStampsY = 9;
                    //row1
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.Empty,MazeWallType.Empty,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row2
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightUp,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.LeftDown,
                        MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        }));
                    //row3
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,
                        MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row4
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.RightDown,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Empty
                        }));
                    //row5
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.RightUp,
                        MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Vertical,MazeWallType.Empty
                        }));
                    //row6
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Empty
                        }));
                    //row7
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightDown,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty
                        }));
                    //row8
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.LeftUp,
                        MazeWallType.RightDown,MazeWallType.LeftDown,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row9
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    break;
                case MazeType.TypeD:
                    mazeStampsX = 19;
                    mazeStampsY = 11;
                    //row1
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.Empty,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,
                        MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row2
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightUp,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,
                        MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row3
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.LeftUp,
                        MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,MazeWallType.Empty
                        }));
                    //row4
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Empty,
                        MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.LeftDown,
                        MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Empty
                        }));
                    //row5
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.RightDown,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,
                        MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row6
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Vertical,
                        MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row7
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Vertical,
                        MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftDown,MazeWallType.Empty,MazeWallType.Vertical,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row8
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Vertical,
                        MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,
                        MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.Empty
                        }));
                    //row9
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.LeftDown,MazeWallType.RightUp,
                        MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.RightDown,MazeWallType.LeftUp,MazeWallType.Empty
                        }));
                    //row10
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.RightUp,MazeWallType.Horizontal,
                        MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.LeftDown,
                        MazeWallType.RightDown,MazeWallType.Horizontal,MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    //row11
                    mazeWallBase.AddRange(
                        BuildMazeWalls(new List<MazeWallType>()
                        {MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,
                        MazeWallType.RightUp,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,MazeWallType.Horizontal,
                        MazeWallType.LeftUp,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty,MazeWallType.Empty
                        }));
                    break;
            }
            MazeBaseData mazeData = new MazeBaseData();
            mazeData.mazeWallBase = mazeWallBase;
            mazeData.mazeStampsX = mazeStampsX;
            mazeData.mazeStampsY = mazeStampsY;
            return mazeData;
        }

        private static List<MazeWall> BuildMazeWalls(List<MazeWallType> wallTypes)
        {
            return wallTypes.Select(w => new MazeWall(w)).ToList();
        }
    }
}
