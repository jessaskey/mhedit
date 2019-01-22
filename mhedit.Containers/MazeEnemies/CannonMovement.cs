using mhedit.Containers.MazeEnemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace mhedit.Containers
{

    [Serializable]
    [XmlInclude(typeof(CannonMovementPosition))]
    [XmlInclude(typeof(CannonMovementPause))]
    [XmlInclude(typeof(CannonMovementMove))]
    [XmlInclude(typeof(CannonMovementReturn))]
    [XmlInclude(typeof(CannonGunPosition))]
    [XmlInclude(typeof(CannonGunSpeed))]
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
