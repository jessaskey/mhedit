using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit
{
    public static class MHPController
    {
        private static MHPServiceReference.ImheditClient _client = new MHPServiceReference.ImheditClient();

        public static bool Login(string username, string password)
        {
            return _client.Login(username, password);
        }

        public static bool UploadMazeDefinition(string username, string password, string mazeDefinition)
        {
            return false;
        }
    }
}
