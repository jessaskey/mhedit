using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeObjects
{

    [Serializable]
    public enum TokenStyle
    {
        Fallout = 0,
        Star = 1,
        Diamond = 2,
        Particle = 3
    }

    [ Serializable ]
    public class HiddenLevelToken : MazeObject
    {
        private static readonly Point _snapSize = new Point( 1, 1 );
        private TokenStyle _tokenStyle;
        private int _targetLevel = 25;
        private int _returnLevel = 1;
        private int _visibleDistance;

        public HiddenLevelToken()
            : this( TokenStyle.Fallout )
        { }

        private HiddenLevelToken( TokenStyle style )
            : base( Constants.MAXOBJECTS_TOKEN,
                ImageFactory.Create( style ),
                Point.Empty,
                new Point( 8, 8 ) )
        {
            this._tokenStyle = style;
        }

        [ Browsable( false ) ]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }


        [CategoryAttribute("Custom")]
        [DescriptionAttribute(
            "Selects the Hidden Level that Rex will visit with this Token.")]
        public TokenStyle TokenStyle
        {
            get { return this._tokenStyle; }
            set
            {
                if (this._tokenStyle != value)
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ImageFactory.Create(value);
                    this.SetField(ref this._tokenStyle, value);
                }
            }
        }

        [CategoryAttribute("Custom")]
        [DescriptionAttribute(
            "Selects the Hidden Level that Rex will visit with this Token.")]
        [Validation(typeof(RangeRule<int>),
            Options = "Minimum=1;Maximum=28")]
        public int TargetLevel
        {
            get { return this._targetLevel; }
            set { this.SetField(ref this._targetLevel, value); }
        }

        [ CategoryAttribute( "Custom" ) ]
        [ DescriptionAttribute(
            "The Level that Rex will return to after completion of the hidden level." ) ]
        [ Validation( typeof( RangeRule<int> ),
            Options = "Minimum=1;Maximum=28" ) ]
        public int ReturnLevel
        {
            get { return this._returnLevel; }
            set { this.SetField( ref this._returnLevel, value ); }
        }

        [CategoryAttribute( "Custom" )]
        [DescriptionAttribute( "How close Rex needs to be to the token before it's visible." )]
        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=0;Maximum=10" )]
        public int VisibleDistance
        {
            get { return this._visibleDistance; }
            set { this.SetField( ref this._visibleDistance, value ); }
        }

        public override byte[] ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(
                DataConverter.PointToByteArrayLong(
                    DataConverter.ConvertPixelsToVector( this.Position ) ) );

            bytes.Add( (byte)( this.TargetLevel - 1 ) );

            bytes.Add( (byte)( this.ReturnLevel - 1 ) );

            bytes.Add( (byte)( this.VisibleDistance ) );

            return bytes.ToArray();
        }

        [ BrowsableAttribute( false ) ]
        public override byte[] ToBytes( object obj )
        {
            return ToBytes();
        }

        public static byte[] EmptyBytes
        {
            get
            {
                return new byte[] { 0xFF, 0, 0, 0, 0, 0, 0, 0 };
            }
        }

        private class ImageFactory
        {
            public static Image Create( TokenStyle hiddenLevel )
            {
                Image image = null;

                switch ( hiddenLevel )
                {
                    case TokenStyle.Fallout:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_a_obj.png" );
                        break;

                    case TokenStyle.Star:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_b_obj.png" );
                        break;

                    case TokenStyle.Diamond:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_c_obj.png" );
                        break;

                    case TokenStyle.Particle:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_d_obj.png" );
                        break;
                }

                return image;
            }
        }
    }

}
