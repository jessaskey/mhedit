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
                    new Point( 16, 16 ) )
        { }

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
