using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    public class ObjectEncoding
    {

        public List<byte> Bytes { get; set; }
        public String Comment { get; set; }

        public ObjectEncoding()
        {
            Bytes = new List<byte>();
        }

    }
}
