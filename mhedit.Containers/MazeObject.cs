using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace mhedit.Containers
{
    [DefaultPropertyAttribute( "Name" )]
    [Serializable]
    public abstract class MazeObject : ChangeTrackingBase, IName
    {
        private static readonly Point _snapSize = new Point( 64, 64 );

        private readonly Guid _id = Guid.NewGuid();
        private string name;
        private Point _position = Point.Empty;
        private readonly int _maxObjects;
        private Image _image;
        private readonly Point _staticLsb;
        private Point _renderOffset;

        private bool _selected = false;

        private bool _isValid = true;
        private List<string> _validationErrors = new List<string>();

        protected MazeObject( int maxObjects, Image image )
            : this( maxObjects, image, new Point(), new Point() )
        {}

        protected MazeObject( int maxObjects, Image image, Point staticLsb, Point renderOffset )
        {
            this._maxObjects = maxObjects;

            this._image = image;

            this._staticLsb = staticLsb;

            this._renderOffset = renderOffset;
        }

        [BrowsableAttribute( false )]
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        [BrowsableAttribute( true )]
        [DescriptionAttribute( "The name of this maze object." )]
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

        [DescriptionAttribute( "Maximum number of this Maze Object allowed in the maze." )]
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

        /// <summary>
        /// Some objects are stored with low resolution position information. For these
        /// objects the LSB of the 16bit position value is a fixed (Static) value, which
        /// means a fixed location within each maze stamp.
        /// </summary>
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
                return this.Selected | this.IsHighlighted ?
                    this.AddSelectedDecoration( (Image)this._image.Clone() ) : this._image;
            }
            protected set { this._image = value; }
        }

        /// <summary>
        /// Since XY games draw graphics with a beam, these objects have a random 
        /// starting point defined by where the drawing starts. This starting point
        /// is defined by the Object's Position property. This starting point
        /// isn't typically the center of the image being drawn. Therefore this offset
        /// is used to place the Objects image on the maze as if it were drawn there.
        /// </summary>
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

#if DEBUG
#else
        [BrowsableAttribute( false )]
#endif
        [TypeConverter( typeof( TypeConverters.VectorPositionTypeConverter ) )]
        [ReadOnly( true )]
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

        /// <summary>
        /// In some cases there are maze objects whom are connected to other maze
        /// objects by behavior. For Example TripPads and their associated Pyroids.
        /// The IsHighlighted property allows the user to visualize this connection
        /// on the screen.
        /// </summary>
        [BrowsableAttribute( false )]
        [XmlIgnore]
        protected virtual bool IsHighlighted
        {
            get { return false; }
        }

        public abstract byte[] ToBytes();

        public abstract byte[] ToBytes( object obj );

        public virtual Point GetAdjustedPosition( Point point )
        {
            Point finalPosition = new Point();

            /// Modify the current mouse position to make it relative to the MazeGrid (by
            /// removing the Padding around the Maze) since that's what the ROMs expect. The
            /// Rendering code will handle padding for the UI display.
            /// Don't shift by half image size because this could cause the MazeObject to
            /// be placed into an adjacent maze stamp. (The user expects it to be dropped
            /// into the stamp where the pointer is pointing). If we start to render the 
            /// Image while being dragged this might change.
            finalPosition.X = point.X - DataConverter.PADDING;
            finalPosition.Y = point.Y - DataConverter.PADDING;

            /// Get origin position (Upper Left) of the current Snap Grid. This isn't a visible
            /// grid, but defines the coarse grid that the object is allowed to be placed on. 
            finalPosition.X -= ( finalPosition.X % this.SnapSize.X );
            finalPosition.Y -= ( finalPosition.Y % this.SnapSize.Y );

            /// Handle objects which are stored with low resolution position information. These
            /// objects have a fixed or Static LSB, which means a fixed location within each
            /// maze stamp. Must divide the LSB data by our scaling factor.
            finalPosition.X += this.StaticLSB.X / DataConverter.PositionScaleFactor;
            finalPosition.Y += this.StaticLSB.Y / DataConverter.PositionScaleFactor;

            return finalPosition;
        }

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

            Pen redPen = this.IsHighlighted ?
                new Pen( Color.Aquamarine, 1 ) :
                new Pen( Color.Orange, 1 );

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
