using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    public enum TransporterDirection : int
    {
        /// <summary>
        /// Open to the right
        /// </summary>
        Right = 0,
        /// <summary>
        /// Open to the left
        /// </summary>
        Left
    }

    [Serializable]
    public class Transporter : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 8;

        private Point _position;
        private TransporterDirection _direction = TransporterDirection.Right;
        private Image _img;
        private BitArray _transportability = new BitArray(32, true);
        private ObjectColor _color = ObjectColor.Red;

        public Transporter()
        {
            LoadDefaultImage();
            //renderOffset.X = 32;
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

        [CategoryAttribute("Direction")]
        [DescriptionAttribute("The Direction of the transporter.")]
        public TransporterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        [CategoryAttribute("Transportability")]
        [DescriptionAttribute("Flags setting the transportability data for moving objects in the maze.")]
        public BitArray Transportability
        {
            get { return _transportability; }
            set { _transportability = value; }
        }

        [DescriptionAttribute("Maximum number of transports allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [DescriptionAttribute("The color of the door. Doors can only be opened by keys of the same color.")]
        public ObjectColor Color
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
            List<byte> bytes = new List<byte>();

            byte colorByte = (byte)(((byte)_color) & 0x0F);
            if (_direction == TransporterDirection.Right)
            {
                colorByte += 0x10;
            }
            bytes.Add(colorByte);
            bytes.AddRange(Context.PointToByteArrayPacked(new Point(_position.X, _position.Y + 64)));

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
                _img = ResourceFactory.ReplaceColor(_img, System.Drawing.Color.Yellow, MazeFactory.GetObjectColor(_color));
                if (_direction == TransporterDirection.Right)
                {
                    _img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.transporter_obj.ico");
        }
    }
}
