using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit
{
    /// <summary>
    /// Ghost of the XSD GameProfile class, this will go away when we merge code, but for now, I 
    /// will code against this so we can code in Parallel.
    /// </summary>
    public class GameProfile
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public GameProfile()
        {
            Id = Guid.NewGuid();
        }
    }
}
