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



}
