using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    [Serializable]
    public abstract class MazeObject : TrackEditsBase
    {
        private static readonly Point _snapSize = new Point( 64, 64 );

        private readonly Guid _id = Guid.NewGuid();
        private string name;
        private Point _position = Point.Empty;
        private readonly int _maxObjects;
        private Image _image;
        private readonly Point _staticLsb;
        private Point _renderOffset;
        private readonly Point _dragDropFix;

        private bool _selected = false;

        private bool _isValid = true;
        private List<string> _validationErrors = new List<string>();

        protected MazeObject( int maxObjects, Image image )
            : this( maxObjects, image, new Point(), new Point() )
        {}

        protected MazeObject( int maxObjects, Image image, Point staticLsb, Point renderOffset )
            : this( maxObjects, image, staticLsb, renderOffset, new Point() )
        {}

        protected MazeObject( int maxObjects, Image image, Point staticLsb, Point renderOffset, Point dragFix )
        {
            this._maxObjects = maxObjects;

            this._image = image;

            this._staticLsb = staticLsb;

            this._renderOffset = renderOffset;

            this._dragDropFix = dragFix;
        }

        [BrowsableAttribute( false )]
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get { return name; }
            set { this.SetField( ref this.name, value ); }
        }

        [CategoryAttribute( "Location" )]
        [DescriptionAttribute( "The start location of the object in the maze." )]
        public virtual Point Position
        {
            get { return _position; }
            set { this.SetField( ref this._position, value ); }
        }

        [DescriptionAttribute( "Maximum number of reactoids allowed in this maze." )]
        public int MaxObjects
        {
            get { return this._maxObjects; }
        }

        [BrowsableAttribute( false )]
        public Size Size
        {
            get { return this.Image.Size; }
        }

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public virtual Point SnapSize
        {
            get { return _snapSize; }
        }

        [BrowsableAttribute( false )]
        public Point DragDropFix
        {
            get { return _dragDropFix; }
        }

        [BrowsableAttribute( false )]
        [IgnoreDataMemberAttribute]
        public Point StaticLSB
        {
            get { return _staticLsb; }
        }

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public Image Image
        {
            get
            {
                return this._selected ?
                    this.AddSelectedDecoration( this._image ) : this._image;
            }
            protected set { this._image = value; }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public Point RenderOffset 
        {
            get { return _renderOffset; }
            protected set { this._renderOffset = value; }
        }

        [BrowsableAttribute(false)]
        public Point RenderPosition
        {
            get
            {
                return new Point(Position.X - _renderOffset.X, Position.Y - _renderOffset.Y);
            }
        }

        [TypeConverter(typeof(TypeConverters.VectorPositionTypeConverter))]
        [ReadOnly(true)]
        public Point VectorPosition
        {
            get
            {
                return DataConverter.ConvertPixelsToVector(Position);
            }
        }

        [CategoryAttribute("Validation")]
        [ReadOnly(true)]
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        [CategoryAttribute("Validation")]
        [ReadOnly(true)]
        [XmlIgnore]
        public List<string> ValidationErrors 
        {
            get
            {
                return _validationErrors;
            }
            set
            {
                _validationErrors = value;
            }
        }

        [BrowsableAttribute(false)]
        [XmlIgnore]
        public bool Selected
        {
            get { return this._selected; }
            set { this._selected = value; }
        }

        public abstract byte[] ToBytes();

        public abstract byte[] ToBytes( object obj );

        public void LoadPosition(byte bytePosition)
        {
            Tuple<short, short> oxoidVector = DataConverter.BytePackedToVector(bytePosition, this.StaticLSB);
            Position = DataConverter.ConvertVectorToPixels(oxoidVector);
        }

        public void LoadPosition(byte[] longPosition)
        {
            Position = DataConverter.ConvertVectorToPixels(DataConverter.ByteArrayLongToPoint(longPosition));
        }

        protected virtual Image AddSelectedDecoration( Image img )
        {
            //draw little brackets in each corner
            Graphics g = Graphics.FromImage( img );
            Pen redPen = new Pen( Color.Orange, 1 );
            //top left
            g.DrawLine( redPen, 0, 0, 0, 6 );
            g.DrawLine( redPen, 0, 0, 6, 0 );
            //top right
            g.DrawLine( redPen, img.Width - 1, 0, img.Width - 1, 6 );
            g.DrawLine( redPen, img.Width - 1, 0, img.Width - 7, 0 );
            //bottom left
            g.DrawLine( redPen, 0, img.Height - 1, 0, img.Height - 7 );
            g.DrawLine( redPen, 0, img.Height - 1, 6, img.Height - 1 );
            //bottom right
            g.DrawLine( redPen, img.Width - 1, img.Height - 1, img.Width - 1, img.Height - 7 );
            g.DrawLine( redPen, img.Width - 1, img.Height - 1, img.Width - 7, img.Height - 1 );
            return img;
        }
    }
}
