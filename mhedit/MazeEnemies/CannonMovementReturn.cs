using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    [Serializable]
    public class CannonMovementReturn : iCannonMovement
    {
        public override string ToString()
        {
            return "Return to Start";
        }

        #region iCannonMovement

        public CannonMovementType GetMovementType()
        {
            return CannonMovementType.Pause;
        }

        public SignedVelocity GetVelocity()
        {
            return null;
        }

        public CannonGunAngle GetGunAngle()
        {
            return CannonGunAngle.Up;
        }

        public int GetWaitFrames()
        {
            return 0;
        }

        public CannonGunAngleSpeed GetAngleSpeed()
        {
            return CannonGunAngleSpeed.Slow;
        }

        public byte GetFireStatus()
        {
            return 0;
        }

        #endregion
    }
}
