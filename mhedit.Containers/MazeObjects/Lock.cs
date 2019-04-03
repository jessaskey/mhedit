using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{

    /// <summary>
    /// Locks are not passable unless the player has a key of matching color.
    /// </summary>
    [Serializable]
    public class Lock : MazeObject
    {
        private const string ImageResource = "mhedit.Containers.Images.Objects.lock_obj.png";
        private ObjectColor _color = ObjectColor.Yellow;

        public Lock()
            : base( Constants.MAXOBJECTS_LOCK,
                    ResourceFactory.GetResourceImage( ImageResource ),
                    new Point( 0x80, 0x80 ),
                    new Point( 32, 32 ) )
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
                -32 : 32;

            return adjusted;
        }

        [DescriptionAttribute( "The color of the Lock. The Lock will only open doors with the same color." )]
        public ObjectColor LockColor
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
            throw new Exception("Lock requires it's related key to be passed into the ToBytes(object) method.");
        }

        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Key)
            {
                bytes.Add((byte)_color);
                bytes.AddRange(DataConverter.PointToByteArrayPacked(((Key)obj).Position));
                bytes.AddRange(DataConverter.PointToByteArrayPacked(new Point(this.Position.X, this.Position.Y)));
            }
            else
            {
                throw new Exception("Lock.ToByte() requires it's related key to be passed into the ToBytes(object) method.");
            }
            return bytes.ToArray();
        }
    }
}
