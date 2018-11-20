
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit
{
    public static class MHPController
    {
        public static MHEditServiceReference.MHEditClient GetClient()
        {
            MHEditServiceReference.MHEditClient client = new MHEditServiceReference.MHEditClient();
#if DEBUG
            client.Endpoint.Address = new System.ServiceModel.EndpointAddress("http://localhost:52484/MHEdit.svc");
#endif
            return client;
        }

        public static string Ping()
        {
            MHEditServiceReference.MHEditClient _client = GetClient();
            return _client.Ping();
        }
        public static MHEditServiceReference.SecurityToken Login(string username, string password)
        {
            MHEditServiceReference.MHEditClient _client = GetClient();
            MHEditServiceReference.ClientResponseOfSecurityToken6aJH8QNC response = _client.Login(username, password);
            if (response.IsSuccessful)
            {
                return response.Payload;
            }
            return null;
        }

        public static bool UploadMazeDefinition(MHEditServiceReference.SecurityToken token, byte[] mazeDefinition, byte[] screenshot)
        {
            MHEditServiceReference.MHEditClient _client = GetClient();
            MHEditServiceReference.ClientResponseOfboolean result =_client.SubmitMaze(token, mazeDefinition, screenshot);
            return result.Payload;
        }

        public static List<string> GetMazes()
        {
            MHEditServiceReference.MHEditClient _client = GetClient();
            var result = _client.GetMazes("date", "", "");
            return null;
        }
    }
}
