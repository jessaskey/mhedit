using System;

namespace mhedit.Containers
{
    /// <summary>
    /// The velocity class contains both X and Y components of velocity
    /// </summary>
    [Serializable]
    public class Velocity : TrackEditsBase
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
                this.SetField( ref this._x, value );
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
                this.SetField( ref this._y, value );
            }
        }

    }
}
