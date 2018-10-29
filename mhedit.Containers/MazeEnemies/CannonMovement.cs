using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.Containers
{
    public enum CannonMovementType
    {
        Return,
        Position,
        Move,
        Pause
    }

    public enum CannonGunPosition
    {
        TopRight = 0,
        MiddleRight,
        BottomRight,
        Down,
        TopLeft,
        MiddleLeft,
        BottomLeft
    }

    public enum CannonGunSpeed
    {
        Slow = 0,
        Medium,
        Fast
    }

}
