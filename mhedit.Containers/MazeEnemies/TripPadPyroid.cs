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
        private int _velocity;
        private PyroidStyle _pyroidStyle = PyroidStyle.Double;

        public TripPadPyroid()
            : base( 7,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.pyroidr_obj.png" ) )
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
            set { _tripPad = value; }
        }

        [BrowsableAttribute( false )]
        [XmlIgnore]
        protected override bool IsHighlighted
        {
            get { return this._tripPad.Selected; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines the X velocity of the pyroid launched.")]
        public int Velocity
        {
            get { return _velocity; }
            set { this.SetField( ref this._velocity, value ); }
        }

        [DescriptionAttribute("Defines if the launched pyroid is a single or double Pyroid.")]
        public PyroidStyle PyroidStyle
        {
            get { return _pyroidStyle; }
            set { this.SetField( ref this._pyroidStyle, value ); }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            byte[] position = DataConverter.PointToByteArrayShort(new Point(this.Position.X, this.Position.Y + 64));
            if (_pyroidStyle == PyroidStyle.Single)
            {
                position[0] |= 0x80;
            }
            bytes.AddRange(position);

            byte velocity = (byte)Math.Abs(_velocity);
            if (_velocity < 0)
            {
                velocity |= 0x80;
            }
            bytes.Add(velocity);
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
