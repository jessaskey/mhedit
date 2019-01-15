using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.Containers
{

    [Serializable]
    public class CannonMovement
    {
        public CannonMovementType MovementType { get; set; }
        public SignedVelocity Velocity { get; set; }
        public CannonGunPosition GunPosition { get; set; }
        public int WaitFrames { get; set; }
        public CannonGunSpeed GunSpeed { get; set; }
        public byte FireSpeed { get; set; }
    }

    [Serializable]
    public enum CannonMovementType : int
    {
        Return,
        Position,
        Move,
        Pause
    }

    [Serializable]
    public enum CannonGunPosition : int
    {
        TopRight = 0,
        MiddleRight,
        BottomRight,
        Down,
        TopLeft,
        MiddleLeft,
        BottomLeft
    }

    [Serializable]
    public enum CannonGunSpeed : int
    {
        Slow = 0,
        Medium,
        Fast
    }

}
