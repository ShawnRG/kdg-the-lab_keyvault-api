using System.Collections.Generic;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Logging;

namespace KeyVaultCertificateStore
{
    public class KeyVaultCertificateAdder
    {
        private readonly ILogger<KeyVaultCertificateAdder> _logger;
        private readonly CertificateClient _client;

        public KeyVaultCertificateAdder(CertificateClient client, ILogger<KeyVaultCertificateAdder> logger)
        {
            _client = client;
            _logger = logger;
        }

        public KeyVaultCertificateWithPolicy Add(string name, bool? enabled = true, Dictionary<string, string> tags = default)
        {
            _logger.LogInformation("Start create certificate");
            return _client.StartCreateCertificate(name, CertificatePolicy.Default, enabled, tags).WaitForCompletionAsync().Result.Value;
        }
    }
}
