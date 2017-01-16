using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
{        
    /// <summary>
    /// The Hand may be placed on any maze. If it is not disabled by the player, it will automatically 
    /// turn off the reactoid if it is triggered.
    /// </summary>
    [Serializable]
    public class Hand : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 1;

        private Point _position;
        private Image _img;

        public Hand()
        {
            Init(new Point(0, 0));
        }

        public Hand(Point position)
        {
            Init(position);
        }

        private void Init(Point position)
        {
            //base.mazeObjectType = MazeObjectType.Hand;
            LoadDefaultImage();
            renderOffset.X = -8;
            renderOffset.Y = 24;
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
            _img = Resource.GetResourceImage("images.objects.hand_obj.ico");
        }
    }
}
