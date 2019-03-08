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
                    new Point( 0, 64 ) )
        { }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// LightningV's require a special adjustment for drag/drop operations to make
            /// the drop behavior/location logical from the Users perspective. This is
            /// due to the Image being the same size as a maze stamp AND the image needs
            /// to be displayed between 2 maze stamps.
            /// Thus, we force the object to the:
            ///     Lower stamp if the drop location is in the bottom half of a stamp.
            ///     Current stamp if the drop location is in the top half of a stamp.
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                32 : 96;

            return adjusted;
        }

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
