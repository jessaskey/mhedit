using mhedit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit
{
    public static class MHPController
    {
        private static MHEditServiceReference.MHEditClient _client = new MHEditServiceReference.MHEditClient();

        public static bool Login(string username, string password)
        {
            MHEditServiceReference.ClientResponseOfbase64Binary response = _client.GetEncryptionKey();
            if (response.IsSuccessful)
            {
                StringEncryption se = new StringEncryption(response.Payload);
                string encryptedUsername = se.Encrypt(username);
                string encryptedPassword = se.Encrypt(password);
                MHEditServiceReference.ClientResponseOfboolean responseLogin = _client.Login(encryptedUsername, encryptedPassword);
                if (responseLogin.IsSuccessful)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool UploadMazeDefinition(string username, string password, string mazeDefinition)
        {
            return false;
        }
    }
}
