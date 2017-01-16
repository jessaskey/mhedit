using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeEnemies
{
    enum PyroidStyle
    {
        Double = 0,
        Single = 1
    }
    /// <summary>
    /// Pyroids are the common 'spark-like' enemies in the maze. They have a speed and velocity
    /// component and freeze when the reactoid is touched.
    /// </summary>
    [Serializable]
    class TripPadPyroid : MazeObject
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
            Init(new Point(0,0));
        }

        public TripPadPyroid(Point position)
        {
            Init(position);
        }

        public override string ToString()
        {
            return name;
        }

        private void Init(Point position)
        {
            LoadDefaultImage();
            renderOffset.X = 32;
            _position = position;
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
            _img = Resource.GetResourceImage("images.objects.pyroidr_obj.ico");
        }
    }
}
