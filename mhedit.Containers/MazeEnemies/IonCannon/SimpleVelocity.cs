using System;

namespace mhedit.Containers.MazeEnemies.IonCannon
{
    [Serializable]
    public class SimpleVelocity
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
                if ( value > 64 || value < -64 )
                {
                    throw new ArgumentOutOfRangeException( nameof( X ),
                        value, "Must be -64 < value < 64." );
                }

                _x = value;
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
                if ( value > 64 || value < -64 )
                {
                    throw new ArgumentOutOfRangeException( nameof( Y ),
                        value, "Must be -64 < value < 64." );
                }

                _y = value;
            }
        }

        //public override string ToString()
        //{
        //    return $"Velocity  X:{this.X}, Y:{this.Y}";
        //}
    }
}
