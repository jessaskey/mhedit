using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{
    /// <summary>
    /// OneWay signs only allow the player to pass through in the direction specified.
    /// </summary>
    [Serializable]
    public class OneWay : MazeObject
    {
        private static readonly Point _snapSize = new Point( 4, 4 );

        private OneWayDirection _direction = OneWayDirection.Right;

        public OneWay()
            : this( OneWayDirection.Right )
        { }

        private OneWay( OneWayDirection direction )
            : base( 4,
                    ImageFactory.Create( direction ),
                    new Point( 0x80, 0x80 ),
                    new Point( 32, 32 ) )
        { }

        [BrowsableAttribute( false )]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute("The Direction of the gate.")]
        public OneWayDirection Direction
        {
            get { return _direction; }
            set
            {
                if ( this._direction != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ImageFactory.Create( value );

                    this.SetField( ref this._direction, value );
                }
            }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(DataConverter.PointToByteArrayPacked(new Point(this.Position.X, this.Position.Y + 64)));
            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private class ImageFactory
        {
            public static Image Create( OneWayDirection arrowDirection )
            {
                Image image = null;

                switch ( arrowDirection )
                {
                    case OneWayDirection.Right:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.oneway_obj.png" );
                        //rotation is okay
                        break;
                    case OneWayDirection.Left:
                        image = ResourceFactory.GetResourceImage( "mhedit.Containers.Images.Objects.oneway_l_obj.png" );
                        break;
                }

                return image;
            }
        }
    }
}
