using System;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    [Serializable]
    public class SimpleVelocity : ChangeTrackingBase
    {
        private int _x;
        private int _y;

        public int X
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

        public int Y
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

        //public override string ToString()
        //{
        //    return $"Velocity  X:{this.X}, Y:{this.Y}";
        //}
    }
}
