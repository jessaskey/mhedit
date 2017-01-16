using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    [Serializable]
    public class CannonMovementMove : iCannonMovement
    {
        [TypeConverter(typeof(TypeConverters.SignedVelocityTypeConverter))]
        public SignedVelocity Velocity { get; set; }

        public int WaitFrames { get; set; }

        public CannonMovementMove()
        {
            Velocity = new SignedVelocity();
        }

        public override string ToString()
        {
            return "Move: " + Velocity.X.ToString() + "," + Velocity.Y.ToString();
        }

        #region iCannonMovement

        public CannonMovementType GetMovementType()
        {
            return CannonMovementType.Move;
        }

        public SignedVelocity GetVelocity()
        {
            return Velocity;
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
