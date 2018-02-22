using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
{
    /// <summary>
    /// The direction that a OneWay allows the player to pass.
    /// </summary>
    public enum OneWayDirection
    {
        /// <summary>
        /// One Way Arrows allowing travel left to right
        /// </summary>
        Right = 0,
        /// <summary>
        /// One Way Arrows allowing travel right to left
        /// </summary>
        Left
    }

    /// <summary>
    /// OneWay signs only allow the player to pass through in the direction specified.
    /// </summary>
    [Serializable]
    public class OneWay : MazeObject
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 4;

        private Point _position;
        private OneWayDirection _direction = OneWayDirection.Right;
        private Image _img;

        public OneWay()
        {
            renderOffset.X = 32;
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

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The Direction of the gate.")]
        public OneWayDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        [DescriptionAttribute("Maximum number of one way's allowed in this maze.")]
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
                LoadRightImage();
                if (_direction == OneWayDirection.Left)
                {
                    LoadLeftImage();
                }
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadLeftImage()
        {
            _img = Resource.GetResourceImage("images.objects.oneway_l_obj.ico");
        }

        private void LoadRightImage()
        {
            _img = Resource.GetResourceImage("images.objects.oneway_obj.ico");
        }
    }
}
