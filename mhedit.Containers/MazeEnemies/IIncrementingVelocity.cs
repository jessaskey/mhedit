using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers.MazeEnemies
{
    public interface IIncrementingVelocity
    {
        SignedVelocity GetVelocity();
        SignedVelocity GetIncrementingVelocity();
    }
}
