using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    [Serializable]
    class CannonMovementAngle : iCannonMovement
    {
        public CannonGunAngle Angle { get; set; }
        public CannonGunAngleSpeed Speed { get; set; }
        public byte FireShot { get; set; }

        public override string ToString()
        {
            return "Angle: " + Angle.ToString() + " Speed: " + Speed.ToString() + " Fire: " + FireShot.ToString();
        }

        #region iCannonMovement

        public CannonMovementType GetMovementType()
        {
            return CannonMovementType.Angle;
        }

        public SignedVelocity GetVelocity()
        {
            return null;
        }

        public CannonGunAngle GetGunAngle()
        {
            return Angle;
        }

        public int GetWaitFrames()
        {
            return 0;
        }

        public CannonGunAngleSpeed GetAngleSpeed()
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
