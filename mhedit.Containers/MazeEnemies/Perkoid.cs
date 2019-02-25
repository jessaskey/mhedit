using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// The Perkoid class implements the common robots in the maze. Perkoids have a direction and
    /// velocity properties.
    /// </summary>
    [Serializable]
    public class Perkoid : MazeObject
    {
        private static readonly Point _snapSize = new Point( 4, 4 );

        private SignedVelocity _velocity = new SignedVelocity();
        private SignedVelocity _incrementingVelocity = new SignedVelocity();

        public Perkoid()
            : base( 10,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.perkoid_obj.png" ),
                    Point.Empty,
                    new Point( 16, 16 ) )
        {}

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how the object moves within the maze and at what speed.")]
        [TypeConverter(typeof(TypeConverters.SignedVelocityTypeConverter))]
        public SignedVelocity Velocity
        {
            get { return _velocity; }
            set { this.SetField( ref this._velocity, value ); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines the additional velocity added at each difficulty level. Generally leave this at zero.")]
        [TypeConverter(typeof(TypeConverters.SignedVelocityTypeConverter))]
        public SignedVelocity IncrementingVelocity
        {
            get { return _incrementingVelocity; }
            set { this.SetField( ref this._incrementingVelocity, value ); }
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
