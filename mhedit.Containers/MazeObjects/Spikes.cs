using System;
using System.Collections.Generic;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    [Serializable]
    public class Spikes : MazeObject
    {
        public Spikes()
            : base( 5,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.spikes_obj.png" ),
                    Point.Empty,
                    new Point( 0, 64 ),
                    new Point( 0, 32 ) )
        { }

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
