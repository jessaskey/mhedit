using System;

namespace mhedit.Containers
{
    /// <summary>
    /// The velocity class contains both X and Y components of velocity
    /// </summary>
    [Serializable]
    public struct Velocity
    {
        private byte _x;
        private byte _y;

        public byte X
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

        public byte Y
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
