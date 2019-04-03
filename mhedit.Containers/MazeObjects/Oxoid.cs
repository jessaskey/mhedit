using System;
using System.ComponentModel;
using System.Drawing;

namespace mhedit.Containers.MazeObjects
{

    [Serializable]
    public class Oxoid : MazeObject
    {
        private const string ImageResource = "mhedit.Containers.Images.Objects.oxoid_obj.png";
        private OxoidType _oxoidType = OxoidType.Fixed;

        public Oxoid()
            : base( Constants.MAXOBJECTS_OXOID,
                    ResourceFactory.GetResourceImage( ImageResource ),
                    new Point( 0x90, 0x40 ),
                    new Point( 4, 6 ) )
        { }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("Defines whether the oxoid is of fixed point value or if the point value increases with each oxoid collected.")]
        public OxoidType OxoidType
        {
            get { return _oxoidType; }
            set
            {
                if ( this._oxoidType != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image =
                        ResourceFactory.ReplaceColor(
                            ResourceFactory.GetResourceImage( ImageResource ),
                            Color.Fuchsia,
                            value == OxoidType.Fixed ? Color.Fuchsia : Color.Yellow );

                    this.SetField( ref this._oxoidType, value );
                }
            }
        }

        public override byte[] ToBytes()
        {
            return DataConverter.PointToByteArrayPacked(this.Position);
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }
    }
}
