using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{

    /// <summary>
    /// The signed velocity class contains both X and Y components of velocity
    /// and is used exclusively for IonCannons
    /// </summary>
    [Serializable]
    public class SignedVelocity
    {
        public sbyte X { get; set; }
        public sbyte Y { get; set; }
    }
}
