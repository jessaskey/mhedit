using System;
using System.Collections.Generic;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    [Serializable]
    public class Spikes : MazeObject
    {
        public Spikes()
            : base( Constants.MAXOBJECTS_SPIKES,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.spikes_obj.png" ),
                    new Point( 0x80, 0xB0 ),
                    new Point( 32, 20 ) )
        { }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being the same size as a maze stamp AND the image needs
            /// to be displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                -56 : 8;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayPacked(this.Position));
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
