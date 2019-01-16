using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    /// <summary>
    /// The vector class contains a start and end point
    /// </summary>
    [Serializable]
    public class Vector
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    }
}
