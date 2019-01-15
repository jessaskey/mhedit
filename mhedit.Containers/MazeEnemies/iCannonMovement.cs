﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.Containers.MazeEnemies
{
    public interface ICannonMovement
    {
        CannonMovementType GetMovementType();
        SignedVelocity GetVelocity();
        CannonGunPosition GetGunPosition();
        int GetWaitFrames();
        CannonGunSpeed GetSpeed();
        byte GetFireSpeed();
    }
}
