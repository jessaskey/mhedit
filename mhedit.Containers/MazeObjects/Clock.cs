using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
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
            LoadDefaultImage();
            renderOffset.X = 16;
            renderOffset.Y = 16;
            staticLsb = new Point(0x00, 0x40);
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
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.clock_obj.ico");
        }
    }
}
