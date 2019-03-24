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
        private static readonly Point _snapSize = new Point( 1, 1 );

        private int _triggerDistance;
        private MaxSpeed _speed;

        public Maxoid()
            : base( Constants.MAXOBJECTS_MAXOID,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.roboid_obj.png" ),
                    Point.Empty,
                    new Point( 16, 16 ) )
        {}

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how fast Max moves after Rex.")]
        public MaxSpeed Speed
        {
            get { return _speed; }
            set { this.SetField( ref this._speed, value ); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how many maze squares between Max and Rex before Max will start pursuit.")]
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
