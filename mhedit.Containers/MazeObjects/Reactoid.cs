using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    [Serializable]
    public class Reactoid : MazeObject
    {
        private static readonly Point _snapSize = new Point( 4, 4 );

        private int _timer = 30;

        public Reactoid()
            : base( 1,
                    ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.reactoid_obj.png" ),
                    Point.Empty,
                    new Point( 15, 24 ) )
        { }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The amount of time allowed to exit the maze upon triggering the reactoid.")]
        public int Timer
        {
            get { return _timer; }
            set { this.SetField( ref this._timer, value ); }
        }

        public override byte[] ToBytes(object obj)
        {
            List<byte> bytes = new List<byte>();
            if (obj is Point)
            {
                //Position
                bytes.AddRange(DataConverter.PointToByteArrayLong(DataConverter.ConvertPixelsToVector(this.Position)));
            }
            else if (obj is int)
            {
                //Decimal Mode here requires extra conversion for Timer value
                bytes.Add((byte)Convert.ToInt16(("0x" + _timer.ToString()), 16));
            }
            else
            {

            }
            return bytes.ToArray();
        }

        public override byte[] ToBytes()
        {
            throw new Exception("Reactoid must be serialized in parts. Use other ToBytes(object) method.");
        }
    }
}
