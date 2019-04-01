using System;

namespace mhedit.Containers
{

    /// <summary>
    /// The signed velocity class contains both X and Y components of velocity
    /// and is used exclusively for IonCannons
    /// </summary>
    [Serializable]
    public class SignedVelocity : ChangeTrackingBase
    {
        private sbyte _x;
        private readonly Range<int> _xRange;
        private sbyte _y;
        private readonly Range<int> _yRange;

        public SignedVelocity( Range<int> xRange, Range<int> yRange )
        {
            this._xRange = xRange;

            this._yRange = yRange;
        }

        public sbyte X
        {
            get
            {
                return _x;
            }
            set
            {
                if ( !this._xRange.ContainsValue( value ) )
                {
                    throw new ArgumentOutOfRangeException( nameof( X ), value,
                        $"Must be {this._xRange.Minimum} <= value <= {this._xRange.Minimum}." );
                }

                this.SetField( ref this._x, value );
            }
        }

        public sbyte Y
        {
            get
            {
                return _y;
            }
            set
            {
                if ( !this._yRange.ContainsValue( value ) )
                {
                    throw new ArgumentOutOfRangeException( nameof( Y ), value,
                        $"Must be {this._yRange.Minimum} <= value <= {this._yRange.Minimum}." );
                }

                this.SetField( ref this._y, value );
            }
        }
    }
}
