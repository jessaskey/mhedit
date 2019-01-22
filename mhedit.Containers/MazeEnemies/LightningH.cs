using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// The Lightning class shows the force fields in the maze. They will kill the player upon contact. 
    /// The Lightning objects have have either horizontal or vertical orientation.
    /// </summary>
    [Serializable]
    public class LightningH : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 7;

        private Point _position;
        private Image _img;

        public LightningH()
        {
            LoadDefaultImage();
            renderOffset.X = 32;
            renderOffset.Y = 64;
            base.dragDropFix.Y = 32;
            base.dragDropFix.X = 32;
            staticLsb = new Point(0x00, 0);  //offset of 128d is in vectors
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

        [DescriptionAttribute("Maximum number of lightnings allowed in this maze.")]
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
        public override byte[] ToBytes()
        {
            return DataConverter.PointToByteArrayPacked(_position);
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
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
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.lightning_obj.png");
        }
    }
}
