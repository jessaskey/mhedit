using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using mhedit.Containers.Validation;

namespace mhedit.Containers.MazeObjects
{

    public enum HiddenLevels
    {
        _25_Fallout = 25,
        _26_Star,
        _27_Diamond,
        _28_Particle
    }

    [ Serializable ]
    public class HiddenLevelToken : MazeObject
    {
        private static readonly Point _snapSize = new Point( 1, 1 );
        private HiddenLevels _hiddenLevel;
        private int _returnLevel = 1;
        private int _visibleDistance;

        public HiddenLevelToken()
            : this( HiddenLevels._25_Fallout )
        { }

        private HiddenLevelToken( HiddenLevels level )
            : base( Constants.MAXOBJECTS_TOKEN,
                ImageFactory.Create( level ),
                Point.Empty,
                new Point( 8, 8 ) )
        {
            this._hiddenLevel = level;
        }

        [ Browsable( false ) ]
        public override Point SnapSize
        {
            get { return _snapSize; }
        }

        [ CategoryAttribute( "Custom" ) ]
        [ DescriptionAttribute(
            "Selects the Hidden Level that Rex will visit with this Token. Can only have one for each Hidden Level." ) ]
        public HiddenLevels TargetLevel
        {
            get { return this._hiddenLevel; }
            set
            {
                if ( this._hiddenLevel != value )
                {
                    /// Must change Image first then property so any UX updates get proper
                    /// image.
                    this.Image = ImageFactory.Create( value );

                    this.SetField( ref this._hiddenLevel, value );
                }
            }
        }

        [ CategoryAttribute( "Custom" ) ]
        [ DescriptionAttribute(
            "The Level that Rex will return to after completion of the hidden level." ) ]
        [ Validation( typeof( RangeRule<int> ),
            Options = "Minimum=1;Maximum=21" ) ]
        public int ReturnLevel
        {
            get { return this._returnLevel; }
            set { this.SetField( ref this._returnLevel, value ); }
        }

        [CategoryAttribute( "Custom" )]
        [DescriptionAttribute( "How close Rex needs to be to the token before it's visible." )]
        [Validation( typeof( RangeRule<int> ),
            Options = "Minimum=0;Maximum=255" )]
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

        private class ImageFactory
        {
            public static Image Create( HiddenLevels hiddenLevel )
            {
                Image image = null;

                switch ( hiddenLevel )
                {
                    case HiddenLevels._25_Fallout:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_a_obj.png" );
                        break;

                    case HiddenLevels._26_Star:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_b_obj.png" );
                        break;

                    case HiddenLevels._27_Diamond:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_c_obj.png" );
                        break;

                    case HiddenLevels._28_Particle:
                        image = ResourceFactory.GetResourceImage(
                            "mhedit.Containers.Images.Objects.token_d_obj.png" );
                        break;
                }

                return image;
            }
        }
    }

}
