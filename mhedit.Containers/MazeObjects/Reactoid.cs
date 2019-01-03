using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    [Serializable]
    public class Reactoid : MazeObject
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 1;

        private Point _position;
        private Image _img;
        private int _timer = 30;

        public Reactoid()
        {
            LoadDefaultImage();
            renderOffset.X = 15; //12;
            renderOffset.Y = 24; // 16; // 16;

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

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get
            {
                return new Point(_SNAP_X, _SNAP_Y);
            }
        }

        [DescriptionAttribute("Maximum number of reactoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The amount of time allowed to exit the maze upon triggering the reactoid.")]
        public int Timer
        {
            get { return _timer; }
            set { _timer = value; }
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

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Point)
            {
                //Position
                bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(_position)));
            }
            else if (obj is int)
            {
                //Decimal Mode here requires extra conversion for Timer value
                bytes.Add((byte)Convert.ToInt16(("0x" + _timer.ToString()), 16));
            }
            else
            {

            }
            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            throw new Exception("Reactoid must be serialized in parts. Use other ToBytes(object) method.");
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.reactoid_obj.ico");
        }
    }
}
