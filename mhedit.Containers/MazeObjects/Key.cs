using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// Keys are used to open locks of a matching color
    /// </summary>
    [Serializable]
    public class Key : MazeObject
    {
        private const string ImageResource = "mhedit.Containers.Images.Objects.key_obj.png";
        private ObjectColor _color = ObjectColor.Yellow;

        public Key()
            : base( Constants.MAXOBJECTS_KEY,
                    ResourceFactory.GetResourceImage( ImageResource ),
                    new Point( 0x00, 0x40 ),
                    new Point( 8, 8 ) )
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

        [DescriptionAttribute("The color of the key. The key will only open doors with the same color.")]
        public ObjectColor KeyColor
        {
            get { return _color; }
            set
            {
                if ( this._color != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image =
                        ResourceFactory.ReplaceColor(
                            ResourceFactory.GetResourceImage( ImageResource ),
                            Color.Yellow,
                            MazeFactory.GetObjectColor( value ) );

                    this.SetField( ref this._color, value );
                }
            }
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
