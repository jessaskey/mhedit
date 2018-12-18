using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// Pyroids are the common 'spark-like' enemies in the maze. They have a speed and velocity
    /// component and freeze when the reactoid is touched.
    /// </summary>
    [Serializable]
    public class Maxoid : MazeObject
    {
        public enum MaxSpeed : int
        {
            Slowest = 0,
            Slow,
            Medium,
            Agressive
        }

        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 8;

        private Point _position;
        private Image _img;
        private Velocity _velocity;
        private int _triggerDistance;
        private MaxSpeed _speed;

        public Maxoid()
        {
            LoadDefaultImage();
            _velocity = new Velocity();
            renderOffset.X = 8;
            renderOffset.Y = 8;
        }

        [DescriptionAttribute("Maximum number of maxoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
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

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how the object moves within the maze and at what speed.")]
        [TypeConverter(typeof(TypeConverters.VelocityTypeConverter))]
        public Velocity Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how fast Max moves after Rex.")]
        public MaxSpeed Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how many maze squares between Max and Rex before Max will start persuit.")]
        public int TriggerDistance
        {
            get { return _triggerDistance; }
            set { _triggerDistance = value; }
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
            bytes.AddRange(Context.PointToByteArrayLong(Context.ConvertPixelsToVector(_position)));
            int speedDistance =  ((byte)(((int)_speed)<<4)&0x30) +((byte)(_triggerDistance&0x0F));
            bytes.Add((byte)speedDistance);
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
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.roboid_obj.ico");
        }
    }
}
