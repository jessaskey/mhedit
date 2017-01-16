using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
{
    /// <summary>
    /// Keys are used to open locks of a matching color
    /// </summary>
    [Serializable]
    public class Key : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 3;

        private Point _position;
        private ObjectColor _color = ObjectColor.Yellow;
        private Image _img;

        public Key()
        {
            Init(new Point(0, 0));
        }

        public Key(Point position)
        {
            Init(position);
        }

        private void Init(Point position)
        {
            //base.mazeObjectType = MazeObjectType.Key;
            LoadDefaultImage();
            _position = position;
            renderOffset.Y = 8;
            renderOffset.X = 8;
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

        [DescriptionAttribute("The color of the key. The key will only open doors with the same color.")]
        public ObjectColor KeyColor
        {
            get { return _color; }
            set { _color = value; }
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
                _img = Resource.ReplaceColor(_img, Color.Yellow, MazeFactory.GetObjectColor(_color));
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadDefaultImage()
        {
            _img = Resource.GetResourceImage("images.objects.key_obj.ico");
        }
    }
}
