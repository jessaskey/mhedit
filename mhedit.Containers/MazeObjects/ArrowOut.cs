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
        private ArrowOutDirection _arrowDirection = ArrowOutDirection.Right;
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
            bytes.AddRange(DataConverter.PointToByteArrayPacked(_position));
            bytes.Add((byte)(_arrowDirection+8)); //Offset for OUT Arrow values
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
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrowout_right_obj.ico");
                    break;
                case ArrowDirection.Down:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrowout_down_obj.ico");
                    break;
                case ArrowDirection.Left:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrowout_left_obj.ico");
                    break;
                case ArrowDirection.Up:
                    _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.arrowout_right_obj.ico");
                    break;
            }
        }

    }
}

