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
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 10;

        private Point _position;
        private ArrowDirection _arrowDirection = ArrowDirection.Right;
        private Image _img;

        public ArrowOut()
        {
            //base.mazeObjectType = MazeObjectType.Arrow;
            LoadDefaultImage();
            _position = Point.Empty;
            renderOffset.X = 0;
            renderOffset.Y = 8;
            staticLsb = new Point(0xc0, 0x40);
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("The start location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("Defined the direction that the arrow is pointing.")]
        public ArrowDirection ArrowDirection
        {
            get { return _arrowDirection; }
            set { _arrowDirection = value; }
        }

        [DescriptionAttribute("Maximum number of reactoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Context.PointToByteArrayPacked(_position));
            bytes.Add((byte)_arrowDirection);
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        [BrowsableAttribute(false)]
        public override Image Image
        {
            get
            {
                LoadDefaultImage();
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadDefaultImage()
        {
            switch (_arrowDirection)
            {
                case ArrowDirection.Right:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_square_obj.ico");
                    //rotation is okay
                    break;
                case ArrowDirection.Down:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_square_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case ArrowDirection.Left:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_square_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case ArrowDirection.Up:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_square_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case ArrowDirection.UpRight:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_angle_obj.ico");
                    //rotation okay
                    break;
                case ArrowDirection.DownRight:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_angle_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case ArrowDirection.DownLeft:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_angle_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case ArrowDirection.UpLeft:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_angle_obj.ico");
                    _img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                case ArrowDirection.Question:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrow_question_obj.ico");
                    break;
            }
        }

    }
}

