using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// Arrow objects are placed in the maze to give the player help in finding the correct way to the 
    /// reactoid. Arrows may also be placed to deceive the player.
    /// </summary>
    [Serializable]
    public class ArrowOut : MazeObject
    {
        private ArrowOutDirection _arrowDirection = ArrowOutDirection.Right;

        public ArrowOut()
            : this( ArrowOutDirection.Right )
        { }

        private ArrowOut( ArrowOutDirection direction )
            : base( 10,
                    ImageFactory.Create( direction ),
                    new Point( 0xc0, 0x40 ),
                    new Point( 16, 16 ) ) // This is half the image size.
        {
            this._arrowDirection = direction;
        }

        [CategoryAttribute( "Custom" )]
        [DescriptionAttribute( "Defined the direction that the arrow is pointing." )]
        public ArrowOutDirection ArrowDirection
        {
            get { return _arrowDirection; }
            set
            {
                if ( this._arrowDirection != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ImageFactory.Create( value );

                    this.SetField( ref this._arrowDirection, value );
                }
            }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayPacked(this.Position));
            bytes.Add((byte)(_arrowDirection+9)); //Offset for OUT Arrow values
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private class ImageFactory
        {
            public static Image Create( ArrowOutDirection arrowDirection )
            {
                Image image = null;

                switch ( arrowDirection )
                {
                    case ArrowOutDirection.Left:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_out_left_obj.png" );
                        break;
                    case ArrowOutDirection.Down:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_out_down_obj.png" );
                        break;
                    case ArrowOutDirection.Up:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_out_up_obj.png" );
                        break;
                    case ArrowOutDirection.Right:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_out_right_obj.png" );
                        break;
                }

                return image;
            }
        }
    }
}

