using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    [Serializable]
    public class CannonMovementPause : iCannonMovement
    {
        public int WaitFrames { get; set; }

        public override string ToString()
        {
            return "Pause: " + WaitFrames.ToString();
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
            return WaitFrames;
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
