using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeEnemies
{
    /// <summary>
    /// TripPad objects are maze enemies that when stepped on by the player will launch a pyroid from 
    /// a predefined location at a specific speed and velocity.
    /// </summary>
    [Serializable]
    class TripPad : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 8;

        private Point _position;
        private TripPadPyroid _pyroid;
        private Image _img;

        public TripPad()
        {
            Init(new Point(0, 0));
        }

        public TripPad(Point position)
        {
            Init(position);
        }

        private void Init(Point position)
        {
            //base.mazeObjectType = MazeObjectType.TripPad;
            LoadDefaultImage();
            _position = position;
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

        [DescriptionAttribute("Maximum number of trip pads allowed in this maze.")]
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

        [DescriptionAttribute("The pyroid associated with this trip pad.")]
        [TypeConverter(typeof(TypeConverters.TripPadPyroidTypeConverter))]
        public TripPadPyroid Pyroid
        {
            get { return _pyroid; }
            set { _pyroid = value; }
        }

        private void LoadDefaultImage()
        {
            _img = Resource.GetResourceImage("images.objects.trippad_obj.ico");
        }

     }
}
