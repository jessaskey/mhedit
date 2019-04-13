using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    [Serializable]
    public class ExceptionSummary
    {
        public ExceptionSummary() { }
        public ExceptionSummary(Exception ex, string username, string programVersion, string romVersion)
        {
            Message = ex.Message;
            Source = ex.Source;
            StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
            {
                InnerException = new ExceptionSummary(ex.InnerException, username, programVersion, romVersion);
            }
            Username = username;
            ProgramVersion = programVersion;
            RomVersion = romVersion;
        }

        public String Message { get; set; }
        public String Source { get; set; }
        public String StackTrace { get; set; }
        public ExceptionSummary InnerException { get; set; }
        public String Username { get; set; }
        public String ProgramVersion { get; set; }
        public String RomVersion { get; set; }
    }
}
