using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    /// <summary>
    /// The IonCannon class shows the Ion IonCannon in the maze.
    /// </summary>
    [Serializable]
    public class IonCannon : MazeObject
    {
        private const int _SNAP_X = 4;
        private const int _SNAP_Y = 4;
        private const int _MAXOBJECTS = 4;

        private Point _position;
        private Image _img;
        private IonCannonProgram _program = null;

        public IonCannon()
        {
            _program = new IonCannonProgram();
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
        [EditorAttribute(typeof(IonCannonEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlElement( "ReturnToStart", typeof( ReturnToStart ) )]
        [XmlElement( "OrientAndFire", typeof( OrientAndFire ) )]
        [XmlElement( "Move", typeof( Move ) )]
        [XmlElement( "Pause", typeof( Pause ) )]
        public IonCannonProgram Program
        {
            get { return _program; }
            set { _program = value; }
        }

        [BrowsableAttribute(false)]
        public override Point SnapSize
        {
            get { return new Point(_SNAP_X, _SNAP_Y); }
        }

        [BrowsableAttribute(false)]
        [XmlIgnoreAttribute]
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

        [BrowsableAttribute(false)]
        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(_position)));

            //now cannon commands
            this._program.GetObjectData( bytes );

            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.cannon_obj.png");
        }

    }
}
