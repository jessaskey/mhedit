using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeEnemies
{
    public enum PyroidStyle
    {
        Double = 0,
        Single = 1
    }
    /// <summary>
    /// Pyroids are the common 'spark-like' enemies in the maze. They have a speed and velocity
    /// component and freeze when the reactoid is touched.
    /// </summary>
    [Serializable]
    public class TripPadPyroid : MazeObject
    {


        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 7;

        private Point _position;
        private Image _img;
        private int _velocity;
        private TripPad _tripPad;
        private PyroidStyle _pyroidStyle = PyroidStyle.Double;

        public TripPadPyroid()
        {
            LoadDefaultImage();
            renderOffset.X = 0;
        }

        [DescriptionAttribute("Maximum number of pyroids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [BrowsableAttribute(false)]
        public TripPad TripPad
        {
            get
            {
                return _tripPad;
            }
            set
            {
                _tripPad = value;
            }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("The start location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines the X velocity of the pyroid launched.")]
        public int Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        [DescriptionAttribute("Defines if the launched pyroid is a single or double Pyroid.")]
        public PyroidStyle PyroidStyle
        {
            get { return _pyroidStyle; }
            set { _pyroidStyle = value; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get
            {
                return new Point(_SNAP_X, _SNAP_Y);
            }
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            byte[] position = DataConverter.PointToByteArrayShort(new Point(_position.X, _position.Y + 64));
            if (_pyroidStyle == PyroidStyle.Single)
            {
                position[0] |= 0x80;
            }
            bytes.AddRange(position);

            byte velocity = (byte)Math.Abs(_velocity);
            if (_velocity < 0)
            {
                velocity |= 0x80;
            }
            bytes.Add(velocity);
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
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.pyroidr_obj.ico");
        }
    }
}
