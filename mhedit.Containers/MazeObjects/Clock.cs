using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{        
    /// <summary>
    /// The clock object may be placed once in any maze. When triggered, maze enemies slow down.
    /// </summary>
    [Serializable]
    public class Clock : MazeObject
    {
        public Clock()
            : base( 1,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.clock_obj.png" ),
                    new Point( 0x00, 0x40 ),
                    new Point( 20, 20 ) )
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
            bytes.AddRange(DataConverter.PointToByteArrayPacked(this.Position));
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object o)
        {
            return ToBytes();
        }
    }
}
