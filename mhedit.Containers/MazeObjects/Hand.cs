using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
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
            LoadDefaultImage();
            renderOffset.X = -8;
            renderOffset.Y = 24;
            staticLsb = new Point(0x3c, 0x01);
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
        public override byte[] ToBytes()
        {
            throw new Exception("Hand must be serialized with Rectoid position passed.");
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Point)
            {
                Point reactoidPosition = (Point)obj;
                byte[] handLocation = Context.PointToByteArrayShort(_position);
                bytes.AddRange(handLocation);
                byte[] reactoidLocation = Context.PointToByteArrayShort(reactoidPosition);
                int xAccordians = Math.Abs(reactoidLocation[0] - handLocation[0]);
                int yAccordians = Math.Abs(handLocation[1] - reactoidLocation[1]);
                bytes.AddRange(new byte[] { (byte)((xAccordians * 2) + 1), (byte)(yAccordians * 2), 0x3F, 0x0B, 0x1F, 0x05, 0x03 });
            }
            else
            {
                throw new Exception("Parameter passed must be a Reactoid.Position object.");
            }
            return bytes.ToArray();
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
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.hand_obj.ico");
        }
    }
}
