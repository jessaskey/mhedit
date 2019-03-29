using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
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
        private TripPyroidDirection _direction = TripPyroidDirection.Right;

        public TripPadPyroid()
            : this( PyroidStyle.Double )
        { }

        private TripPadPyroid( PyroidStyle pyroidStyle )
            : base( Constants.MAXOBJECTS_TRIPPADPYROID,
                ImageFactory.Create( pyroidStyle ),
                    new Point( 0x40, 0x00 ),
                    new Point( 8, 32 ) )
        { }

        /// <summary>
        /// This property is basically hidden but allows us to track the associated
        /// objects.
        /// </summary>
        [BrowsableAttribute( false )]
        [XmlIgnore]
        public TripPad TripPad
        {
            get { return _tripPad; }
            internal set { _tripPad = value; }
        }

        [BrowsableAttribute( false )]
        [XmlIgnore]
        protected override bool IsHighlighted
        {
            get { return this._tripPad.Selected; }
        }

        [ CategoryAttribute( "Location" ) ]
        [ DescriptionAttribute( "Defines the X velocity of the pyroid launched." ) ]
        public TripPyroidDirection Direction
        {
            get { return this._direction; }
            set { this.SetField( ref this._direction, value ); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines the X velocity of the pyroid launched.")]
        public TripPyroidSpeedIndex SpeedIndex
        {
            get { return this._speedIndex; }
            set { this.SetField( ref this._speedIndex, value ); }
        }

        [DescriptionAttribute("Defines if the launched pyroid is a single or double Pyroid.")]
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
