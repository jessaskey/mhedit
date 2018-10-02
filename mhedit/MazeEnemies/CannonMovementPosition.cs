using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    [Serializable]
    class CannonMovementPosition : iCannonMovement
    {
        public CannonGunPosition Position { get; set; }
        public CannonGunSpeed Speed { get; set; }
        public byte FireShot { get; set; }

        public override string ToString()
        {
            return "Position: " + Position.ToString() + " Speed: " + Speed.ToString() + " Fire: " + FireShot.ToString();
        }

        #region iCannonMovement

        public CannonMovementType GetMovementType()
        {
            return CannonMovementType.Position;
        }

        public SignedVelocity GetVelocity()
        {
            return null;
        }

        public CannonGunPosition GetGunPosition()
        {
            return Position;
        }

        public int GetWaitFrames()
        {
            return 0;
        }

        public CannonGunSpeed GetSpeed()
        {
            return Speed;
        }

        public byte GetFireStatus()
        {
            return FireShot;
        }

        #endregion
    }
}
