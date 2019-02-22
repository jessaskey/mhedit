using System;
using System.Drawing;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// The Lightning class shows the force fields in the maze. They will kill the player upon contact. 
    /// The Lightning objects have have either horizontal or vertical orientation.
    /// </summary>
    [Serializable]
    public class LightningV : MazeObject
    {
        public LightningV()
            : base( 7,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.lightning_v_obj.png" ),
                    Point.Empty, //offset of 128d is in vectors
                    new Point( 0, 64 ) ) //new Point( 32, 64 )
        { }

        public override byte[] ToBytes()
        {
            return DataConverter.PointToByteArrayPacked(this.Position);
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
