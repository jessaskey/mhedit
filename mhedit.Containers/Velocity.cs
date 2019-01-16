using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    /// <summary>
    /// The velocity class contains both X and Y components of velocity
    /// </summary>
    [Serializable]
    public class Velocity
    {
        public byte X { get; set; }
        public byte Y { get; set; }
    }
}
