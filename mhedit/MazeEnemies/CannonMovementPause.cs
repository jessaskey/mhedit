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

        public CannonGunPosition GetGunPosition()
        {
            return CannonGunPosition.MiddleRight;
        }

        public int GetWaitFrames()
        {
            return WaitFrames;
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
