using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text;

namespace mhedit.MazeEnemies
{
    /// <summary>
    /// The Cannon class shows the Ion Cannon in the maze.
    /// </summary>
    [Serializable]
    public class Cannon : MazeObject, ISerializable
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 4;

        private Point _position;
        private Image _img;
        private List<iCannonMovement> _movements = null;

        public Cannon()
        {
            _movements = new List<iCannonMovement>();
            LoadDefaultImage();
            renderOffset.X = 32;
            renderOffset.Y = 32;
        }

        [BrowsableAttribute(false)]
        public override Size Size
        {
            get { return _img.Size; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("The location of the object in the maze.")]
        public override Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [DescriptionAttribute("Maximum number of cannon's allowed in this maze.")]
        public override int MaxObjects
        {
            get { return _MAXOBJECTS; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The movement script for the cannon")]
        [EditorAttribute(typeof(CannonEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<iCannonMovement> Movements
        {
            get { return _movements; }
            set { _movements = value; }
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
            _img = Resource.GetResourceImage("images.objects.cannon_obj.ico");
        }

        #region ISerializable

        //Deserialization constructor.
        public Cannon(SerializationInfo info, StreamingContext ctxt)
        {
            _movements = (List<iCannonMovement>)info.GetValue("Movements", typeof(List<iCannonMovement>));
            _position = (Point)info.GetValue("Position", typeof(Point));
        }
                
        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Movements", _movements);
            info.AddValue("Position", _position);
        }

        #endregion
    }
}
