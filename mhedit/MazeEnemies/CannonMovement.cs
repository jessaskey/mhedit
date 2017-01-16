using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit
{
    public enum CannonMovementType
    {
        Return,
        Angle,
        Move,
        Pause
    }

    public enum CannonGunAngle
    {
        TopRight = 0,
        Right,
        BottomRight,
        Down,
        BottomLeft,
        Left,
        TopLeft,
        Up
    }

    public enum CannonGunAngleSpeed
    {
        Slow = 0,
        Medium,
        Fast
    }

}
