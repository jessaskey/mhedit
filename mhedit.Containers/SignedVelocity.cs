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
        private sbyte _y;

        public sbyte X
        {
            get
            {
                return _x;
            }
            set
            {
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
                this.SetField( ref this._y, value );
            }
        }

#region Overrides of Object

        public override string ToString()
        {
            return $"{this._x},{this._y}";
        }

#endregion
    }
}
