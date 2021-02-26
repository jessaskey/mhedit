using System;
using System.Collections.Generic;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// KeyPouch allows users to hold keys beyond a single level. 
    /// </summary>
    [Serializable]
    public class KeyPouch : MazeObject
    {
        public KeyPouch()
            : base( Constants.MAXOBJECTS_KEYPOUCH,
                ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.keypouch_32.png" ),
                new Point( 0x00, 0x34 ),
                new Point( 16, 16 ) )
        { }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                    0 : 64;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange( DataConverter.PointToByteArrayPacked( this.Position ) );
            return bytes.ToArray();
        }

        public override byte[] ToBytes( object obj )
        {
            return ToBytes();
        }
    }
}
