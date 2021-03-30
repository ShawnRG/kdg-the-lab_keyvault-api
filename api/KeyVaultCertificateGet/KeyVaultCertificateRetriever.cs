using System;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;

namespace KeyVaultCertificateGet
{
    public class KeyVaultCertificateRetriever
    {
        private readonly CertificateClient _client;

        public KeyVaultCertificateRetriever(CertificateClient client) => _client = client;

        public KeyVaultCertificateWithPolicy Get(string name) => _client.GetCertificate(name);
    }
}
