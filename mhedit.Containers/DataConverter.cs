using System;
using System.Drawing;

namespace mhedit.Containers
{
    public static class DataConverter
    {
        private static int _vectorGridSize = 256;
        private const int GRIDUNITS = 8;
        private const int GRIDUNITSTAMPS = 8;
        private static int _canvasGridOffsetX = 3;
        private static int _canvasGridOffsetY = -3;

        /// <summary>
        /// The number of pixels in a Maze Grid/Stamp within the Editor.
        /// </summary>
        public static int CanvasGridSize = GRIDUNITSTAMPS * GRIDUNITS;

        /// <summary>
        /// The number of pixels of Padding around the Maze Canvas in the editor.
        /// For aesthetics only.
        /// </summary>
        public const int PADDING = 10;

        /// <summary>
        /// Scale Factor between Atari and Editor Canvas.
        /// </summary>
        public static int PositionScaleFactor = _vectorGridSize / CanvasGridSize;

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
            pixels.X = (vector.Item1 / PositionScaleFactor) + (_canvasGridOffsetX * CanvasGridSize);  //shift 3 pixel grids to the right
            pixels.Y = Math.Abs((vector.Item2 / PositionScaleFactor)) + (_canvasGridOffsetY * CanvasGridSize) -32; //shift 4 pixel grids up
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
            vector.X = (pixels.X - (_canvasGridOffsetX * CanvasGridSize)) * PositionScaleFactor;
            vector.Y = -1 * ((pixels.Y + 32 - (_canvasGridOffsetY * CanvasGridSize)) * PositionScaleFactor);
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

        public static Tuple<short, short> ByteArrayLongToPoint( byte[] bytes, Point staticLsb = new Point() )
        {
            return AdjustForStaticLsb( bytes, ref staticLsb );
        }


        public static Tuple<short, short> BytePackedToVector(byte b, Point staticLsb)
        {
            byte[] longBytes = new byte[4];

            longBytes[0] = 0;
            longBytes[1] = (byte)((b & 0x0f) + 1);
            longBytes[2] = 0;
            longBytes[3] = (byte)((b >> 4) | 0xf0);

            return AdjustForStaticLsb( longBytes, ref staticLsb );
        }

        private static Tuple<short, short> AdjustForStaticLsb( byte[] bytes, ref Point staticLsb )
        {
            //some packed objects have a pre-defined LSB byte in the code
            //we need to set this upon decoding of the packed data
            //for encoding, it doesn't matter since it goes away.
            if ( staticLsb.X != Point.Empty.X )
            {
                bytes[ 0 ] = (byte)staticLsb.X;
            }

            if ( staticLsb.Y != Point.Empty.Y )
            {
                bytes[ 2 ] = (byte)staticLsb.Y;
            }

            return new Tuple<short, short>(
                (short) ( bytes[ 0 ] + ( bytes[ 1 ] << 8 ) ),
                (short) ( bytes[ 2 ] + ( bytes[ 3 ] << 8 ) ) );
        }

        public static byte[] PointToByteArrayShort(Point point)
        {
            Point vector = new Point( (int)( point.X << 2 ) - 0x300, ( 0xfd00 - (int)( ( point.Y + 32 ) << 2 ) ) );
            byte xh = (byte)((vector.X & 0xff00) >> 8);
            byte yh = (byte)((vector.Y & 0xff00) >> 8);
            return new byte[] { xh, yh };
        }

        public static byte[] PointToByteArrayPacked(Point point)
        {
            Point vector = new Point( (int)( point.X << 2 ) - 0x400, ( 0xfd00 - ( (int)( ( point.Y + 32 ) << 2 ) ) ) );
            byte xh = (byte)((vector.X & 0x0f00) >> 8);
            byte yh = (byte)((vector.Y & 0x0f00) >> 4);
            return new byte[] { (byte)(xh | yh) };
        }

        public static int ToDecimal(int value)
        {
            return Convert.ToInt16(("0x" + value.ToString()), 16);
        }

        public static int FromDecimal(int value)
        {
            return Convert.ToInt16(value.ToString("X2"), 10);
        }
    }
}
