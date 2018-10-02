﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mhedit.MazeEnemies
{
    public interface iCannonMovement
    {

        CannonMovementType GetMovementType();
        SignedVelocity GetVelocity();
        CannonGunPosition GetGunPosition();
        int GetWaitFrames();
        CannonGunSpeed GetSpeed();
        byte GetFireStatus();
    }
}
