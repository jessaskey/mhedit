using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// Boots are a special gift in the maze, when the player is wearing the boots, they have the 
    /// ability to levitate in place by holding the jump button. 
    /// </summary>
    [Serializable]
    public class Boots : MazeObject
    {
        public Boots()
            : base( 1,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.booties_obj.png" ),
                    new Point( 0x00, 0x34 ),
                    new Point( 8, 8 ) )
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
