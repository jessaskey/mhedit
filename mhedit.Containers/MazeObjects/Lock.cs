using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{

    /// <summary>
    /// Locks are not passable unless the player has a key of matching color.
    /// </summary>
    [Serializable]
    public class Lock : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 3;

        private Point _position;
        private ObjectColor _color = ObjectColor.Yellow;
        private Image _img;

        public Lock()
        {
            LoadDefaultImage();
            renderOffset.X = 32;
            renderOffset.Y = 32;
            staticLsb = new Point(0x80, 0x80);
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

        [DescriptionAttribute("Maximum number of reactoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [DescriptionAttribute("The color of the door. Doors can only be opened by keys of the same color.")]
        public ObjectColor LockColor
        {
            get { return _color; }
            set { _color = value; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            throw new Exception("Lock requires it's related key to be passed into the ToBytes(object) method.");
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Key)
            {
                bytes.Add((byte)_color);
                bytes.AddRange(Context.PointToByteArrayPacked(((Key)obj).Position));
                bytes.AddRange(Context.PointToByteArrayPacked(new Point(_position.X, _position.Y + 64)));
            }
            else
            {
                throw new Exception("Lock.ToByte() requires it's related key to be passed into the ToBytes(object) method.");
            }
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override Image Image
        {
            get
            {
                LoadDefaultImage();
                _img = ResourceFactory.ReplaceColor(_img, Color.Yellow, MazeFactory.GetObjectColor(_color));
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.lock_obj.ico");
        }
    }
}
