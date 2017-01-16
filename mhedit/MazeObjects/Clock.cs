using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
{        
    /// <summary>
    /// The clock object may be placed once in any maze. When triggered, maze enemies slow down.
    /// </summary>
    [Serializable]
    public class Clock : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 1;

        private Point _position;
        private Image _img;

        public Clock()
        {
            Init(new Point(0, 0));
        }

        public Clock(Point position)
        {
            Init(position);
        }

        private void Init(Point position)
        {
            //base.mazeObjectType = MazeObjectType.Clock;
            LoadDefaultImage();
            _position = position;
            renderOffset.X = 16;
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
            _img = Resource.GetResourceImage("images.objects.clock_obj.ico");
        }
    }
}
