using System;

namespace GameEditor.Atari.MajorHavoc.Maze.Enemies.IonCannon
{
    public sealed partial class Move
    {
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
                    if ( value > 128 || value < -128 )
                    {
                        throw new ArgumentOutOfRangeException( nameof( X ),
                            value, "Must be -128 < value < 128." );
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
                    if ( value > 128 || value < -128 )
                    {
                        throw new ArgumentOutOfRangeException( nameof( Y ),
                            value, "Must be -128 < value < 128." );
                    }

                    _y = value;
                }
            }
        }
    }
}
