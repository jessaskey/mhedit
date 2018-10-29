using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace mhedit.Containers.MazeEnemies
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
