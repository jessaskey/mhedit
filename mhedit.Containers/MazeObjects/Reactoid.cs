using mhedit.Containers.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{
    [Serializable]
    public class Reactoid : MazeObject
    {
        /// <summary>
        /// The Width of the Reactoid in Atari Vector units.
        /// </summary>
        public const int VectorWidth = 110;
        private const string ImageResource = "mhedit.Containers.Images.Objects.reactoid_obj.png";
        private const string ImageResourceExtraLarge = "mhedit.Containers.Images.Objects.reactoid_extra_obj.png";

        private static readonly Point _snapSize = new Point( 1, 1 );
        private bool _isMegaReactoid = false;
        private int _timer = 30;

        public Reactoid()
            : base( Constants.MAXOBJECTS_REACTOID,
                    ResourceFactory.GetResourceImage(ImageResource),
                    Point.Empty,
                    new Point( 15, 20 ) )
        { }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The amount of time allowed to exit the maze upon triggering the reactoid.")]
        [Validation(typeof(RangeRule<int>),
            Options = "Minimum=-1;Maximum=79")]
        public int Timer
        {
            get { return _timer; }
            set { this.SetField( ref this._timer, value ); }
        }

        [DescriptionAttribute("Determines if the Reactor is extra large size.")]
        public bool MegaReactoid
        {
            get { return this._isMegaReactoid; }
            set {
                //some sizing hacks to show the reactor on the screen correctly, the position that goes into the ROM's needs to be the same as the 
                //regular sized reactoid for several reasons unfortunately
                if (!MegaReactoid && value)
                {
                    //from normal to mega
                    base.RenderOffset = new Point(RenderOffset.X + 60, RenderOffset.Y + 80);
                }
                else if(MegaReactoid && !value)
                {
                    //from mega to normal
                    base.RenderOffset = new Point(RenderOffset.X - 60, RenderOffset.Y - 80);
                }
                this.Image = this.GetReactoidImage(value);
                this.SetField(ref this._isMegaReactoid, value);
            }
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
            else if (obj is bool)
            {

            }
            return bytes.ToArray();
        }

        public override byte[] ToBytes()
        {
            throw new Exception("Reactoid must be serialized in parts. Use other ToBytes(object) method.");
        }

        private Image GetReactoidImage(bool isExtraLarge)
        {
            Image image = ResourceFactory.GetResourceImage(ImageResource);
            if (isExtraLarge)
            {
                image = ResourceFactory.GetResourceImage(ImageResourceExtraLarge);
            }
            return image;
        }
    }
}
