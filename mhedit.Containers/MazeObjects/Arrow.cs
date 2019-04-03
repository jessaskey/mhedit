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
    public class Arrow : MazeObject
    {
        private ArrowDirection _arrowDirection;

        public Arrow()
            : this( ArrowDirection.Right )
        {}

        private Arrow( ArrowDirection direction)
            : base(Constants.MAXOBJECTS_ARROW,
                    ImageFactory.Create( direction ),
                    new Point( 0xc0, 0x40 ),
                    new Point( 8, 8 ) ) // This is half the image size.
        {
            this._arrowDirection = direction;
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("Defined the direction that the arrow is pointing.")]
        public ArrowDirection ArrowDirection
        {
            get { return _arrowDirection; }
            set
            {
                if( this._arrowDirection != value )
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
            bytes.Add((byte)_arrowDirection);
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private class ImageFactory
        {
            public static Image Create( ArrowDirection arrowDirection )
            {
                Image image = null;

                switch ( arrowDirection )
                {
                    case ArrowDirection.Right:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_square_obj.png" );
                        //rotation is okay
                        break;
                    case ArrowDirection.Down:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_square_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate90FlipNone );
                        break;
                    case ArrowDirection.Left:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_square_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate180FlipNone );
                        break;
                    case ArrowDirection.Up:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_square_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate270FlipNone );
                        break;
                    case ArrowDirection.UpRight:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_angle_obj.png" );
                        //rotation okay
                        break;
                    case ArrowDirection.DownRight:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_angle_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate90FlipNone );
                        break;
                    case ArrowDirection.DownLeft:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_angle_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate180FlipNone );
                        break;
                    case ArrowDirection.UpLeft:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_angle_obj.png" );
                        image.RotateFlip( RotateFlipType.Rotate270FlipNone );
                        break;
                    case ArrowDirection.Question:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.arrow_question_obj.png" );
                        break;
                }

                return image;
            }
        }
    }
}

