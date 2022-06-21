using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using mhedit.Containers.Validation;

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
        private int _hitsToKill = 3;
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
        [DescriptionAttribute("How many hits does it take to kill a Maxoid.")]
        [Validation(typeof(RangeRule<int>),
            Options = "Minimum=0;Maximum=4")]
        public int HitsToKill
        {
            get { return _hitsToKill; }
            set { this.SetField(ref this._hitsToKill, value); }
        }

        [CategoryAttribute("Location")]
        [DescriptionAttribute("Defines how many maze squares between Max and Rex before Max" +
                              " will start pursuit. Zero indicates active on maze start.")]
        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=0;Maximum=21" )]
        public int TriggerDistance
        {
            get { return _triggerDistance; }
            set { this.SetField( ref this._triggerDistance, value ); }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(this.Position)));
            int hitsSpeedDistance = (~((byte)(((int)_hitsToKill-1)) << 6) & 0xC0) + ((byte)(((int)_speed)<<4)&0x30) +((byte)(_triggerDistance&0x0F));
            bytes.Add((byte)hitsSpeedDistance);
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
