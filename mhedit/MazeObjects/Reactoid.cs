using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
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
            Init(new Point(0,0));
        }

        public Reactoid(Point position)
        {
            Init(position);
        }

        private void Init(Point position)
        {
            //base.mazeObjectType = MazeObjectType.Reactoid;
            LoadDefaultImage();
            _position = position;
            renderOffset.X = 12;
            renderOffset.Y = 16;
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

        private void LoadDefaultImage()
        {
            _img = Resource.GetResourceImage("images.objects.reactoid_obj.ico");
        }
    }
}
