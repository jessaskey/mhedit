using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace mhedit.Containers.MazeObjects
{

    [Serializable]
    public class Transporter : MazeObject
    {
        private const string ImageResource = "mhedit.Containers.Images.Objects.transporter_obj.png";
        private TransporterDirection _direction;
        private List<bool> _transportability = new List<bool>();
        private ObjectColor _color;
        private bool _isBroken = false;
        private bool _isHidden = false;

        public Transporter()
            : base( 8,
                    ResourceFactory.GetResourceImage( ImageResource ),
                    new Point( 0x80, 0x80 ),
                    new Point( 24, 32 ) )
        {
            this.Color = ObjectColor.Yellow;

            this.Direction = TransporterDirection.Left;
        }

        public override Point GetAdjustedPosition( Point point )
        {
            Point adjusted = base.GetAdjustedPosition( point );

            /// Make a special adjustment for drag/drop operations to make the drop 
            /// behavior/location logical from the Users perspective. This is due 
            /// to the Image being the same size as a maze stamp AND the image needs
            /// to be displayed between 2 maze stamps.
            /// Thus, make adjustments based upon the cursor being in the lower or
            /// upper range of a maze stamp
            adjusted.Y +=
                ( ( point.Y - DataConverter.PADDING ) % DataConverter.CanvasGridSize ) < 32 ?
                -32 : 32;

            return adjusted;
        }

        [CategoryAttribute("Direction")]
        [DescriptionAttribute("The Direction of the transporter.")]
        public TransporterDirection Direction
        {
            get { return _direction; }
            set
            {
                if ( this._direction != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = this.GetTransporterImage( this._color, value );

                    this.RenderOffset = new Point(
                        value == TransporterDirection.Right ? 24 : 40, this.RenderOffset.Y );

                    this.SetField( ref this._direction, value );
                }
            }
        }

        //This is here for backwards compatibility but it is NOT USED
        [BrowsableAttribute(false)]
        [DescriptionAttribute("Flags setting the transportability data for moving objects in the maze.")]
        public List<bool> Transportability
        {
            get { return _transportability; }
            set { this.SetField( ref this._transportability, value ); }
        }

        [DescriptionAttribute("Marks if the transporter is shown as 'Broken' (only for Promised End")]
        public bool IsBroken
        {
            get { return _isBroken; }
            set { this.SetField( ref this._isBroken, value ); }
        }

        [DescriptionAttribute("Marks if the transporter is invisible (only for Promised End")]
        public bool IsHidden
        {
            get { return _isHidden; }
            set { this.SetField( ref this._isHidden, value ); }
        }

        [DescriptionAttribute("The color of the Transporter. Transporters are paired by color.")]
        public ObjectColor Color
        {
            get { return _color; }
            set
            {
                if ( this._color != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = this.GetTransporterImage( value, this._direction );

                    this.SetField( ref this._color, value );
                }
            }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            byte colorByte = (byte)(((byte)_color) & 0x0F);
            if (_direction == TransporterDirection.Right)
            {
                colorByte += 0x10;
            }
            if (_isBroken)
            {
                colorByte += 0x40;
            }
            if (_isHidden)
            {
                colorByte += 0x80;
            }
            bytes.Add(colorByte);
            bytes.AddRange(DataConverter.PointToByteArrayPacked(new Point(this.Position.X, this.Position.Y)));

            return bytes.ToArray();
        }

        public override byte[] ToBytes(object obj)
        {
            return ToBytes();
        }

        private Image GetTransporterImage( ObjectColor color, TransporterDirection direction )
        {
            Image image = ResourceFactory.ReplaceColor(
                ResourceFactory.GetResourceImage( ImageResource ),
                System.Drawing.Color.Yellow,
                MazeFactory.GetObjectColor( color ) );

            if ( direction == TransporterDirection.Right )
            {
                image.RotateFlip( RotateFlipType.Rotate180FlipNone );
            }

            return image;
        }
    }
}
