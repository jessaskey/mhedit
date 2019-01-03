using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// Oxoid type defines whether an oxoid point value is fixed or increases exponentially
    /// </summary>
    public enum OxoidType
    {
        /// <summary>
        /// Fixed oxoids award 500 points
        /// </summary>
        Fixed = 0,
        /// <summary>
        /// Increasing oxoid points start at 200 and increase to 400, 600, 800, 1000, 1200
        /// </summary>
        Increasing
    }

    [Serializable]
    public class Oxoid : MazeObject
    {
        private const int _SNAP_X = 64;
        private const int _SNAP_Y = 64;
        private const int _MAXOBJECTS = 20;

        private Point _position;
        private OxoidType _oxoidType = OxoidType.Fixed;
        private Image _img;

        public Oxoid()
        {
            LoadDefaultImage();
            renderOffset.X = 0;
            renderOffset.Y = 2;
            staticLsb = new Point(0x90, 0x40);
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
        [DescriptionAttribute("Defines whether the oxoid is of fixed point value or if the point value increases with each oxoid collected.")]
        public OxoidType OxoidType
        {
            get { return _oxoidType; }
            set { _oxoidType = value; }
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
                if (_oxoidType == OxoidType.Increasing)
                {
                    ResourceFactory.ReplaceColor(_img, Color.Fuchsia, Color.Yellow);
                }
                if (selected)
                {
                    _img = base.ImageSelected(_img);
                }
                return _img;
            }
        }

        private void LoadDefaultImage()
        {
            _img = ResourceFactory.GetResourceImage("mhedit.Containers.Images.Objects.oxoid_obj.ico");
        }
    }
}
