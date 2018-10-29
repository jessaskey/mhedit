using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.Containers.MazeEnemies
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

        public CannonGunPosition GetGunPosition()
        {
            return CannonGunPosition.MiddleRight;
        }

        public int GetWaitFrames()
        {
            return 0;
        }

        public CannonGunSpeed GetSpeed()
        {
            return CannonGunSpeed.Slow;
        }

        public byte GetFireSpeed()
        {
            return 0;
        }

        #endregion
    }
}
