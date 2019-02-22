using System;
using System.Collections.Generic;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// The Hand may be placed on any maze. If it is not disabled by the player, it will automatically 
    /// turn off the reactoid if it is triggered.
    /// </summary>
    [Serializable]
    public class Hand : MazeObject
    {
        public Hand()
            : base( 1,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.hand_obj.png" ),
                    new Point( 0x3c, 0x01 ),
                    new Point( 10, 10 ) )
        {}

        public override byte[] ToBytes()
        {
            throw new Exception("Hand must be serialized with Rectoid position passed.");
        }

        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Point)
            {
                Point reactoidPosition = (Point)obj;
                byte[] handLocation = DataConverter.PointToByteArrayShort(this.Position);
                bytes.AddRange(handLocation);
                //Context.PointToByteArrayLong(Context.ConvertPixelsToVector(this.Position))
                byte[] reactoidLocation = DataConverter.PointToByteArrayShort(reactoidPosition);
                byte[] reactoidLocation2 = DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(reactoidPosition));
                int xAccordians = ((reactoidLocation[0] - handLocation[0]) * 2) - 1;
                int yAccordians = (handLocation[1] - reactoidLocation[1]) * 2 ; //double Y-accordians
                int xSize = 0x03; //static 3
                int ySize = 4 + (xAccordians / 3);
                bytes.AddRange(new byte[] { (byte)xAccordians, (byte)yAccordians, 0x3F, 0x0B, 0x1F, (byte)ySize, (byte)xSize });
            }
            else
            {
                throw new Exception("Parameter passed must be a Reactoid.Position object.");
            }

            return bytes.ToArray();
        }
    }
}
