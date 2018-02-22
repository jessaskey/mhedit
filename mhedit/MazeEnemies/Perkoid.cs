using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeEnemies
{
    /// <summary>
    /// The Perkoid class implements the common robots in the maze. Perkoids have a direction and
    /// velocity properties.
    /// </summary>
    [Serializable]
    class Perkoid : MazeObject
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 10;

        private Point _position;
        private Image _img;
        private Velocity _velocity;

        public Perkoid()
        {
            LoadDefaultImage();
            _velocity = new Velocity();
            renderOffset.X = 16;
            renderOffset.Y = 16;
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [DescriptionAttribute("Maximum number of perkoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
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

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
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
            _img = Resource.GetResourceImage("images.objects.perkoid_obj.ico");
        }
    }
}
