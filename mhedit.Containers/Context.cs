using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    public static class Context
    {
        //Scale Factor between Atari and Canvas
        private static int _vectorGridSize = 256;
        private static int _canvasGridSize = 64;

        private static int _scaleFactor = _vectorGridSize / _canvasGridSize;

        private static int _canvasGridOffsetX = 3;
        private static int _canvasGridOffsetY = -3;

        /// <summary>
        /// Converts an Atari vector tuple into an editor
        /// surface coordinate.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Point ConvertVectorToPixels(Tuple<short, short> vector)
        {
            // Atari Vectors are in the 4th quadrant
            // 
            // Origin X -> Positive
            // Y
            // |
            // Negative
            //
            // Standard C# Winforms are in a weird 1st quadrant with Y inverted... I don't know how to explain that.
            // This method translates an Atari vector passed as a usigned word tuple (X,Y) and converts it
            // into a Point that can be plotted on the Design Screen. 
            // 
            // Atari has each grid square equal to 0x100 (256 decimal)
            // While our design surface is 0x40 (64 decimal) pixels to a grid square
            //
            // Because of this, we need to scale vectors to Pixes DOWN by a factor of 4 or a >> 2
            //
            // It gets even better... the MH Grid System is a little wonky where the main grid for the maze sections is larger 
            // than the object grid... the object grid is relative to each maze pattern. 

            Point pixels = new Point();
            pixels.X = (vector.Item1 / _scaleFactor) + (_canvasGridOffsetX * _canvasGridSize);  //shift 3 pixel grids to the right
            pixels.Y = Math.Abs((vector.Item2 / _scaleFactor)) + (_canvasGridOffsetY * _canvasGridSize) -32; //shift 4 pixel grids up
            return pixels;
        }


        /// <summary>
        /// Converts an editor coordinate into an Atari coordinate.
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static Point ConvertPixelsToVector(Point pixels)
        {
            Point vector = new Point();
            vector.X = (pixels.X - (_canvasGridOffsetX * _canvasGridSize)) * _scaleFactor;
            vector.Y = -1 * ((pixels.Y + 32 - (_canvasGridOffsetY * _canvasGridSize)) * _scaleFactor);
            return vector;
        }

        public static byte[] PointToByteArrayLong(Point point)
        {
            byte xh = (byte)((point.X & 0xff00) >> 8);
            byte xl = (byte)(point.X & 0x00ff);
            byte yh = (byte)((point.Y & 0xff00) >> 8);
            byte yl = (byte)(point.Y & 0x00ff);
            return new byte[] { xl, xh, yl, yh };
        }

        public static Tuple<short, short> ByteArrayLongToPoint(byte[] bytes)
        {
            short x = (short)(bytes[0] + (bytes[1] << 8));
            short y = (short)(bytes[2] + (bytes[3] << 8));
            Tuple<short, short> vector = new Tuple<short, short>(x, y);
            return vector;
        }
        

        public static Tuple<short, short> BytePackedToVector(byte b, Point staticLsb)
        {
            byte[] longBytes = new byte[4];

            longBytes[0] = 0;
            longBytes[1] = (byte)((b & 0x0f) + 1);
            longBytes[2] = 0;
            longBytes[3] = (byte)((b >> 4) | 0xf0);

            //some packed objects have a pre-defined LSB byte in the code
            //we need to set this upon decoding of the packed data
            //for encoding, it doesn't matter since it goes away.
            if (staticLsb != Point.Empty)
            {
                longBytes[0] = (byte)staticLsb.X; // (byte)_objectLSBs[type].X;
                longBytes[2] = (byte)staticLsb.Y; //(byte)_objectLSBs[type].Y;
            }

            return ByteArrayLongToPoint(longBytes);

            //ushort yh = (ushort)((((b >> 4) & 0x0f) << 8) | 0xf000);
            //ushort xh = (ushort)(((b & 0x0f) + 1) << 8);
            //return new Tuple<ushort, ushort>(xh, yh);
        }

        public static byte[] PointToByteArrayShort(Point point)
        {
            Point vector = new Point((int)(point.X << 2) - 0x300, (0xfd00 - (int)(point.Y << 2)));
            byte xh = (byte)((vector.X & 0xff00) >> 8);
            byte yh = (byte)((vector.Y & 0xff00) >> 8);
            return new byte[] { xh, yh };
        }

        public static byte[] PointToByteArrayPacked(Point point)
        {
            Point vector = new Point((int)(point.X << 2) - 0x400, (0xFD00 - ((int)(point.Y << 2))));
            byte xh = (byte)((vector.X & 0x0f00) >> 8);
            byte yh = (byte)((vector.Y & 0x0f00) >> 4);
            return new byte[] { (byte)(xh | yh) };
        }



    }
}
