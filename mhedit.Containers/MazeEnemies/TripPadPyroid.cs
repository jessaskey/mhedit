using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// Pyroids are the common 'spark-like' enemies in the maze. They have a speed and velocity
    /// component and freeze when the reactoid is touched.
    /// </summary>
    [Serializable]
    public class TripPadPyroid : MazeObject
    {
        private TripPad _tripPad;
        private TripPyroidSpeedIndex _speedIndex = TripPyroidSpeedIndex.Slowest;
        private PyroidStyle _pyroidStyle = PyroidStyle.Double;
        private TripPyroidDirection _direction = TripPyroidDirection.Left;

        public TripPadPyroid()
            : this( PyroidStyle.Double )
        { }

        private TripPadPyroid( PyroidStyle pyroidStyle )
            : base( Constants.MAXOBJECTS_TRIPPADPYROID,
                ImageFactory.Create( pyroidStyle ),
                    new Point( 0x40, 0x80 ), /// Direction.Right 0x80, Direction.Left 0x40
                    new Point( 8, 32 ) )
        { }

#region Overrides of MazeObject

        /// <inheritdoc />
        public override string Name
        {
            get { return base.Name; }
            set {} // Name is implicit since it's assigned to a TripPad.
        }

#endregion

        /// <summary>
        /// This property is basically hidden but allows us to track the associated
        /// objects.
        /// </summary>
        [BrowsableAttribute( false )]
        [XmlIgnore]
        public TripPad TripPad
        {
            get { return this._tripPad; }
            internal set
            {
                if (this._tripPad != null)
                {
                    this._tripPad.PropertyChanged -= this.TripPadPropertyChanged;
                }

                this._tripPad = value;

                if (this._tripPad != null)
                {
                    base.Name = $"{value?.Name}Pyroid";

                    this._tripPad.PropertyChanged += this.TripPadPropertyChanged;
                }
            }
        }

        [BrowsableAttribute( false )]
        [XmlIgnore]
        protected override bool IsHighlighted
        {
            get { return this._tripPad.Selected; }
        }

        [ CategoryAttribute( "Location" ) ]
        [ DescriptionAttribute( "Determines the direction of motion of the launched Pyroid(s)." ) ]
        public TripPyroidDirection Direction
        {
            get { return this._direction; }
            set
            {
                this.StaticLSB = new Point(
                    value == TripPyroidDirection.Right ? 0x80 : 0x40, this.StaticLSB.Y );

                Point currentPosition = this.Position;

                currentPosition.X = ( currentPosition.X & ~0x3F ) +
                                    this.StaticLSB.X / DataConverter.PositionScaleFactor;

                this.Position = currentPosition;

                this.SetField( ref this._direction, value );
            }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute( "Chooses one of several fixed speeds for the launched Pyroid(s)." )]
        public TripPyroidSpeedIndex SpeedIndex
        {
            get { return this._speedIndex; }
            set { this.SetField( ref this._speedIndex, value ); }
        }

        [DescriptionAttribute( "Determines if a single Pyroid or a pair of Pyroids (Double) are launched." )]
        public PyroidStyle PyroidStyle
        {
            get { return _pyroidStyle; }
            set
            {
                if ( this._pyroidStyle != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ImageFactory.Create( value );

                    this.SetField( ref this._pyroidStyle, value );
                }
            }
        }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being the same size as a maze stamp AND the image needs
            /// to be displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                    -32 : 32;

            return adjusted;
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            byte[] position = DataConverter.PointToByteArrayShort(new Point(this.Position.X, this.Position.Y));
            if (_pyroidStyle == PyroidStyle.Single)
            {
                position[0] |= 0x80;
            }
            bytes.AddRange(position);

            bytes.Add( (byte)( (int)this._speedIndex | (int)this._direction ) );
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private void TripPadPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Name))
            {
                base.Name = $"{this.TripPad?.Name}Pyroid";
            }
        }

        private class ImageFactory
        {
            public static Image Create( PyroidStyle pyroidStyle )
            {
                Image image = null;

                switch ( pyroidStyle )
                {
                    case PyroidStyle.Single:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.pyroidr_obj.png" );
                        break;
                    case PyroidStyle.Double:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.pyroidr_d_obj.png" );
                        break;
                }

                return image;
            }
        }
    }
}
