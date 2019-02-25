using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeEnemies
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Maxoid : MazeObject
    {
        private static readonly Point _snapSize = new Point( 4, 4 );

        private Velocity _velocity = new Velocity();
        private int _triggerDistance;
        private MaxSpeed _speed;

        public Maxoid()
            : base( 8,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.roboid_obj.png" ),
                    Point.Empty,
                    new Point( 8, 8 ) )
        {}

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how the object moves within the maze and at what speed.")]
        [TypeConverter(typeof(TypeConverters.VelocityTypeConverter))]
        public Velocity Velocity
        {
            get { return _velocity; }
            set { this.SetField( ref this._velocity, value ); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how fast Max moves after Rex.")]
        public MaxSpeed Speed
        {
            get { return _speed; }
            set { this.SetField( ref this._speed, value ); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how many maze squares between Max and Rex before Max will start persuit.")]
        public int TriggerDistance
        {
            get { return _triggerDistance; }
            set { this.SetField( ref this._triggerDistance, value ); }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(this.Position)));
            int speedDistance =  ((byte)(((int)_speed)<<4)&0x30) +((byte)(_triggerDistance&0x0F));
            bytes.Add((byte)speedDistance);
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
