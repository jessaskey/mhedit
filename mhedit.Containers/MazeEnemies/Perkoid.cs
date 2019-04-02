using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// The Perkoid class implements the common robots in the maze. Perkoids have a direction and
    /// velocity properties.
    /// </summary>
    [Serializable]
    public class Perkoid : MazeObject
    {
        private static readonly Point _snapSize = new Point( 1, 1 );

        private SignedVelocity _velocity;
        private SignedVelocity _incrementingVelocity;

        public Perkoid()
            : base( Constants.MAXOBJECTS_PERKOID,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.perkoid_obj.png" ),
                    Point.Empty,
                    new Point( 16, 20 ) )
        {
            this.Velocity = new SignedVelocity(
                new Range<int>() { Maximum = 32, Minimum = -1 },
                new Range<int>() { Maximum = 32, Minimum = -32 } );

            this.IncrementingVelocity = new SignedVelocity(
                new Range<int>() { Maximum = 16, Minimum = 0 },
                new Range<int>() { Maximum = 16, Minimum = -16 } );
        }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Location")]
        [ReadOnly( true )]
        [DescriptionAttribute( "Defines the vector that object takes in the maze. For Left Facing Zero Velocity use -1 X Velocity" )]
        [TypeConverter(typeof(TypeConverters.SignedVelocityTypeConverter))]
        public SignedVelocity Velocity
        {
            get { return _velocity; }
            set
            {
                this.SetField( ref this._velocity, value );

                this._velocity.PropertyChanged += this.ForwardPropertyChanged;
            }
        }

        [CategoryAttribute("Location")]
        [ReadOnly( true )]
        [DescriptionAttribute( "Defines the additional velocity added at each difficulty level. " +
                               "Note that the sign is forced to match the Velocity. Generally leave this at zero." )]
        [TypeConverter(typeof(TypeConverters.SignedVelocityTypeConverter))]
        public SignedVelocity IncrementingVelocity
        {
            get { return _incrementingVelocity; }
            set
            {
                this.SetField( ref this._incrementingVelocity, value );

                this._incrementingVelocity.PropertyChanged += this.ForwardPropertyChanged;
            }
        }

        #region Implementation of IChangeTracking

        [BrowsableAttribute( false )]
        [XmlIgnore]
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged |
                    this._velocity.IsChanged |
                    this._incrementingVelocity.IsChanged;
            }
        }

        public override void AcceptChanges()
        {
            /// clear composite member first.
            this._velocity.AcceptChanges();

            this._incrementingVelocity.AcceptChanges();

            base.AcceptChanges();
        }

        #endregion

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
