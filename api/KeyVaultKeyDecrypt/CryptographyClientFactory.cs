using System;
using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace KeyVaultKeyDecrypt
{
    public class CryptographyClientFactory
    {

        private readonly TokenCredential _credential;

        public CryptographyClientFactory(TokenCredential credential)
        {
            _credential = credential;
        }

        public CryptographyClient CreateCryptographyClient(string keyUri)
        {
            return new CryptographyClient(new Uri(keyUri), _credential);
        }
    }
}