using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.MazeObjects
{
    /// <summary>
    /// The EscapePodOption defines whether a player *must* use the escape pod to leave the maze
    /// or whether they may use the normal maze exits as well.
    /// </summary>
    [Serializable]
    public enum EscapePodOption
    {
        /// <summary>
        /// If Optional is selected, then the player may optionally use the Escape Pod to leave the 
        /// maze.
        /// </summary>
        Optional = 1,
        /// <summary>
        /// If Required is selected, then the maze doors do not open when the reactoid is triggered 
        /// and the player must use the escape pod to leave the maze safely.
        /// </summary>
        Required = 2
    }

    /// <summary>
    /// Escape pod may be placed on Type 2 mazes only. It allows the player an alternate 
    /// way to get out of the maze after touching the reactor.
    /// </summary>
    [Serializable]
    public class EscapePod : MazeObject
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 1;

        private Point _position;
        private Image _img;
         private EscapePodOption _option = EscapePodOption.Optional;

        public EscapePod()
        {
            LoadDefaultImage();
            _position = new Point(1184, 352);
            staticLsb = new Point(0x00, 0x80);
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [CategoryAttribute("Location")]
        [ReadOnly(true)]
        [DescriptionAttribute("The start location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set {  }
        }

        [DescriptionAttribute("Maximum number of reactoids allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [DescriptionAttribute("Sets whether the player must use the escape pod to exit the maze or if they may also exit the maze through the main maze doors.")]
        public EscapePodOption Option
        {
            get { return _option; }
            set { _option = value; }
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
            _img = Resource.GetResourceImage("images.objects.pod_obj.ico");
        }
    }
}
