using System;

namespace mhedit.Containers
{

    /// <summary>
    /// The signed velocity class contains both X and Y components of velocity
    /// and is used exclusively for IonCannons
    /// </summary>
    [Serializable]
    public struct SignedVelocity
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
                this._x = value;
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
                this._y = value;
            }
        }

    }
}
