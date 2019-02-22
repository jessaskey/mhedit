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
    public class Pyroid : MazeObject
    {
        private static readonly Point _snapSize = new Point( 4, 4 );

        private SignedVelocity _velocity = new SignedVelocity();
        private SignedVelocity _incrementingVelocity = new SignedVelocity();

        public Pyroid()
            : base( 16,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.pyroid_obj.png" ),
                    Point.Empty,
                    new Point( 8, 8 ) )
        {
            this._velocity.PropertyChanged += this.ForwardIsDirtyPropertyChanged;

            this._incrementingVelocity.PropertyChanged += this.ForwardIsDirtyPropertyChanged;
        }

        [XmlIgnore]
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty | 
                    this._velocity.IsDirty | 
                    this._incrementingVelocity.IsDirty;
            }
            set
            {
                base.IsDirty =
                    this._velocity.IsDirty =
                    this._incrementingVelocity.IsDirty = value;
            }
        }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute( "Location" )]
        [DescriptionAttribute( "Defines how the object moves within the maze and at what speed." )]
        [TypeConverter( typeof( TypeConverters.SignedVelocityTypeConverter ) )]
        public SignedVelocity Velocity
        {
            get { return _velocity; }
        }

        [CategoryAttribute( "Location" )]
        [DescriptionAttribute( "Defines the additional velocity added at each difficulty level. Generally leave this at zero." )]
        [TypeConverter( typeof( TypeConverters.SignedVelocityTypeConverter ) )]
        public SignedVelocity IncrementingVelocity
        {
            get { return _incrementingVelocity; }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(this.Position)));

            if (_incrementingVelocity.X != 0)
            {
                if (_incrementingVelocity.X > 0)
                {
                    bytes.Add((byte)(0x80 | _incrementingVelocity.X));
                }
                else
                {
                    bytes.Add((byte)(0x70 | (_incrementingVelocity.X & 0xF)));
                }
            }
            bytes.Add((byte)_velocity.X);

            if (_incrementingVelocity.Y != 0)
            {
                if (_incrementingVelocity.Y > 0)
                {
                    bytes.Add((byte)(0x80 | _incrementingVelocity.Y));
                }
                else
                {
                    bytes.Add((byte)(0x70 | (_incrementingVelocity.Y & 0xF)));
                }
            }
            bytes.Add((byte)_velocity.Y);

            return bytes.ToArray();
        }

        [BrowsableAttribute(false)]
        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
