using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using mhedit.Containers.MazeObjects;

namespace mhedit.Containers
{
    /// <summary>
    /// MazeType defines the 4 basic mazes found in Major Havoc. The base dimensions and walls are different
    /// for each MazeType
    /// </summary>
    public enum MazeType : int
    {
        /// <summary>
        /// Type A Mazes are on levels 1,5,9,13,etc. This maze area is the smallest of the four
        /// and has a bounding grid of 16x7 stamps.
        /// </summary>
        TypeA = 0,
        /// <summary>
        /// Type B Mazes are on levels 2,6,10,14,etc. This maze area has a bounding grid of 21x8 stamps.
        /// </summary>
        TypeB = 1,
        /// <summary>
        /// Type C Mazes are on levels 3,7,11,15,etc. This maze area has a bounding grid of 21x9 stamps.
        /// </summary>
        TypeC = 2,
        /// <summary>
        /// Type D Mazes are on levels 4,8,12,16,etc. This maze area has a bounding grid of 19x11 stamps.
        /// </summary>
        TypeD = 3
    }

    public static class MazeFactory
    {
        public struct MazeBaseData
        {
            public MazeWall[] mazeWallBase;
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
            MazeWall[] mazeWallBase = new MazeWall[Maze.MAXWALLS];
            switch (type)
            {
                case MazeType.TypeA:
                    mazeStampsX = 16;
                    mazeStampsY = 7;
                    //row1
                    mazeWallBase[0] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[1] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[2] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[3] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[4] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[5] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[6] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[7] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[8] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[9] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[10] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[11] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[12] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[13] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[14] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[15] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[16] = new MazeWall(MazeWallType.Empty);
                    //row2
                    mazeWallBase[17] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[18] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[19] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[20] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[21] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[22] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[23] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[24] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[25] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[26] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[27] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[28] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[29] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[30] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[31] = new MazeWall(MazeWallType.Empty);

                    //row3
                    mazeWallBase[32] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[33] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[34] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[35] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[36] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[37] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[38] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[39] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[40] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[41] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[42] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[43] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[44] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[45] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[46] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[47] = new MazeWall(MazeWallType.Empty);
                    //row4
                    mazeWallBase[48] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[49] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[50] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[51] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[52] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[53] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[54] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[55] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[56] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[57] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[58] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[59] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[60] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[61] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[62] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[63] = new MazeWall(MazeWallType.Empty);
                    //row5
                    mazeWallBase[64] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[65] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[66] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[67] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[68] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[69] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[70] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[71] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[72] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[73] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[74] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[75] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[76] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[77] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[78] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[79] = new MazeWall(MazeWallType.Empty);
                    //row6
                    mazeWallBase[80] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[81] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[82] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[83] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[84] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[85] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[86] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[87] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[88] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[89] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[90] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[91] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[92] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[93] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[94] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[95] = new MazeWall(MazeWallType.Empty);
                    //row7
                    mazeWallBase[96] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[97] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[98] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[99] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[100] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[101] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[102] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[103] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[104] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[105] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[106] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[107] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[108] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[109] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[110] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[111] = new MazeWall(MazeWallType.Empty);
                    break;
                case MazeType.TypeB:
                    mazeStampsX = 21;
                    mazeStampsY = 8;
                    mazeWallBase[0] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[1] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[2] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[3] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[4] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[5] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[6] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[7] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[8] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[9] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[10] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[11] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[12] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[13] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[14] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[15] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[16] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[17] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[18] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[19] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[20] = new MazeWall(MazeWallType.Empty);
                    //row2
                    mazeWallBase[21] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[22] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[23] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[24] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[25] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[26] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[27] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[28] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[29] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[30] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[31] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[32] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[33] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[34] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[35] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[36] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[37] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[38] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[39] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[40] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[41] = new MazeWall(MazeWallType.Empty);
                    //row3
                    mazeWallBase[42] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[43] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[44] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[45] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[46] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[47] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[48] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[49] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[50] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[51] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[52] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[53] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[54] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[55] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[56] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[57] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[58] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[59] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[60] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[61] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[62] = new MazeWall(MazeWallType.Empty);
                    //row4
                    mazeWallBase[63] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[64] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[65] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[66] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[67] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[68] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[69] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[70] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[71] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[72] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[73] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[74] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[75] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[76] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[77] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[78] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[79] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[80] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[81] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[82] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[83] = new MazeWall(MazeWallType.Empty);
                    //row5
                    mazeWallBase[84] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[85] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[86] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[87] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[88] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[89] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[90] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[91] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[92] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[93] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[94] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[95] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[96] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[97] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[98] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[99] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[100] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[101] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[102] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[103] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[104] = new MazeWall(MazeWallType.Empty);
                    //row6
                    mazeWallBase[105] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[106] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[107] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[108] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[109] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[110] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[111] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[112] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[113] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[114] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[115] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[116] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[117] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[118] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[119] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[120] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[121] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[122] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[123] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[124] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[125] = new MazeWall(MazeWallType.Empty);
                    //row7
                    mazeWallBase[126] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[127] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[128] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[129] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[130] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[131] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[132] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[133] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[134] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[135] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[136] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[137] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[138] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[139] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[140] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[141] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[142] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[143] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[144] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[145] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[146] = new MazeWall(MazeWallType.Empty);
                    //row8
                    mazeWallBase[147] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[148] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[149] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[150] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[151] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[152] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[153] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[154] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[155] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[156] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[157] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[158] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[159] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[160] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[161] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[162] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[163] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[164] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[165] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[166] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[167] = new MazeWall(MazeWallType.Empty);
                    break;
                case MazeType.TypeC:
                    mazeStampsX = 21;
                    mazeStampsY = 9;
                    //row1
                    mazeWallBase[0] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[1] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[2] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[3] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[4] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[5] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[6] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[7] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[8] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[9] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[10] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[11] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[12] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[13] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[14] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[15] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[16] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[17] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[18] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[19] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[20] = new MazeWall(MazeWallType.Empty);
                    //row2
                    mazeWallBase[21] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[22] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[23] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[24] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[25] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[26] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[27] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[28] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[29] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[30] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[31] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[32] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[33] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[34] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[35] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[36] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[37] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[38] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[39] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[40] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[41] = new MazeWall(MazeWallType.Empty);
                    //row3
                    mazeWallBase[42] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[43] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[44] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[45] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[46] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[47] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[48] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[49] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[50] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[51] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[52] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[53] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[54] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[55] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[56] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[57] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[58] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[59] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[60] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[61] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[62] = new MazeWall(MazeWallType.Empty);
                    //row4
                    mazeWallBase[63] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[64] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[65] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[66] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[67] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[68] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[69] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[70] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[71] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[72] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[73] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[74] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[75] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[76] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[77] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[78] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[79] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[80] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[81] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[82] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[83] = new MazeWall(MazeWallType.Empty);
                    //row5
                    mazeWallBase[84] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[85] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[86] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[87] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[88] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[89] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[90] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[91] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[92] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[93] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[94] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[95] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[96] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[97] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[98] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[99] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[100] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[101] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[102] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[103] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[104] = new MazeWall(MazeWallType.Empty);
                    //row6
                    mazeWallBase[105] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[106] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[107] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[108] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[109] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[110] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[111] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[112] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[113] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[114] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[115] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[116] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[117] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[118] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[119] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[120] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[121] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[122] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[123] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[124] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[125] = new MazeWall(MazeWallType.Empty);
                    //row7
                    mazeWallBase[126] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[127] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[128] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[129] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[130] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[131] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[132] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[133] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[134] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[135] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[136] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[137] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[138] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[139] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[140] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[141] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[142] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[143] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[144] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[145] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[146] = new MazeWall(MazeWallType.Empty);
                    //row8
                    mazeWallBase[147] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[148] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[149] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[150] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[151] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[152] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[153] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[154] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[155] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[156] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[157] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[158] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[159] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[160] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[161] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[162] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[163] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[164] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[165] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[166] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[167] = new MazeWall(MazeWallType.Empty);
                    //row9
                    mazeWallBase[168] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[169] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[170] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[171] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[172] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[173] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[174] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[175] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[176] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[177] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[178] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[179] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[180] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[181] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[182] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[183] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[184] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[185] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[186] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[187] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[188] = new MazeWall(MazeWallType.Empty);
                    break;
                case MazeType.TypeD:
                    mazeStampsX = 19;
                    mazeStampsY = 11;
                    //row1
                    mazeWallBase[0] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[1] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[2] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[3] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[4] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[5] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[6] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[7] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[8] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[9] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[10] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[11] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[12] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[13] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[14] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[15] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[16] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[17] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[18] = new MazeWall(MazeWallType.Empty);
                    //row2
                    mazeWallBase[19] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[20] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[21] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[22] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[23] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[24] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[25] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[26] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[27] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[28] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[29] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[30] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[31] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[32] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[33] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[34] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[35] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[36] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[37] = new MazeWall(MazeWallType.Empty);
                    //row3
                    mazeWallBase[38] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[39] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[40] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[41] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[42] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[43] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[44] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[45] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[46] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[47] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[48] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[49] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[50] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[51] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[52] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[53] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[54] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[55] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[56] = new MazeWall(MazeWallType.Empty);
                    //row4
                    mazeWallBase[57] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[58] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[59] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[60] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[61] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[62] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[63] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[64] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[65] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[66] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[67] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[68] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[69] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[70] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[71] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[72] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[73] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[74] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[75] = new MazeWall(MazeWallType.Empty);
                    //row5
                    mazeWallBase[76] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[77] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[78] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[79] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[80] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[81] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[82] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[83] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[84] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[85] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[86] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[87] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[88] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[89] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[90] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[91] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[92] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[93] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[94] = new MazeWall(MazeWallType.Empty);
                    //row6
                    mazeWallBase[95] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[96] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[97] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[98] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[99] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[100] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[101] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[102] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[103] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[104] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[105] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[106] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[107] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[108] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[109] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[110] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[111] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[112] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[113] = new MazeWall(MazeWallType.Empty);
                    //row7
                    mazeWallBase[114] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[115] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[116] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[117] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[118] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[119] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[120] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[121] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[122] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[123] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[124] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[125] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[126] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[127] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[128] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[129] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[130] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[131] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[132] = new MazeWall(MazeWallType.Empty);
                    //row8
                    mazeWallBase[133] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[134] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[135] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[136] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[137] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[138] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[139] = new MazeWall(MazeWallType.Vertical);
                    mazeWallBase[140] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[141] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[142] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[143] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[144] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[145] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[146] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[147] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[148] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[149] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[150] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[151] = new MazeWall(MazeWallType.Empty);
                    //row9
                    mazeWallBase[152] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[153] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[154] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[155] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[156] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[157] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[158] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[159] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[160] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[161] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[162] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[163] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[164] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[165] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[166] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[167] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[168] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[169] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[170] = new MazeWall(MazeWallType.Empty);
                    //row10
                    mazeWallBase[171] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[172] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[173] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[174] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[175] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[176] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[177] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[178] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[179] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[180] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[181] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[182] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[183] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[184] = new MazeWall(MazeWallType.LeftDown);
                    mazeWallBase[185] = new MazeWall(MazeWallType.RightDown);
                    mazeWallBase[186] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[187] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[188] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[189] = new MazeWall(MazeWallType.Empty);
                    //row11
                    mazeWallBase[190] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[191] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[192] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[193] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[194] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[195] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[196] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[197] = new MazeWall(MazeWallType.RightUp);
                    mazeWallBase[198] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[199] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[200] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[201] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[202] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[203] = new MazeWall(MazeWallType.Horizontal);
                    mazeWallBase[204] = new MazeWall(MazeWallType.LeftUp);
                    mazeWallBase[205] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[206] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[207] = new MazeWall(MazeWallType.Empty);
                    mazeWallBase[208] = new MazeWall(MazeWallType.Empty);
                    break;
            }
            MazeBaseData mazeData = new MazeBaseData();
            mazeData.mazeWallBase = mazeWallBase;
            mazeData.mazeStampsX = mazeStampsX;
            mazeData.mazeStampsY = mazeStampsY;
            return mazeData;
        }
    }
}
