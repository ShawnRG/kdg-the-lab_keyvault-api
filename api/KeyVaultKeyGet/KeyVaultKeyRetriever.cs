using System;
using Azure.Security.KeyVault.Keys;

namespace KeyvaultKeyGet
{
    public class KeyVaultKeyRetriever
    {
        private readonly KeyClient _client;

        public KeyVaultKeyRetriever(KeyClient client)
        {
            _client = client;
        }

        public KeyVaultKey Get(string name)
        {
            return _client.GetKey(name);
        }
        
         
    }
}