using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    public class ExceptionSummary
    {
        public String Message { get; set; }
        public String Source { get; set; }
        public String StackTrace { get; set; }
        public ExceptionSummary InnerException { get; set; }

        public String Username { get; set; }
        public String ProgramVersion { get; set; }
    }
}
