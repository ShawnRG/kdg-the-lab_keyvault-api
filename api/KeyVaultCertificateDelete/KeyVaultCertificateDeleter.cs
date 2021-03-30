using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Logging;

namespace KeyVaultCertificateDelete
{
    public class KeyVaultCertificateDeleter
    {
        private readonly CertificateClient _client;
        private readonly ILogger<KeyVaultCertificateDeleter> _logger;

        public KeyVaultCertificateDeleter(CertificateClient client, ILogger<KeyVaultCertificateDeleter> logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Delete(KeyVaultCertificateDeleteEvent @event)
        {
            _logger.LogInformation($"Deleting certificate with name <{@event.Name}>");
            var operation = _client.StartDeleteCertificate(@event.Name).WaitForCompletionAsync().Result.Value;

            if (!@event.Purge) return;
            Purge(@event);
        }

        private void Purge(KeyVaultCertificateDeleteEvent @event)
        {
            _logger.LogInformation($"Purging certificate with name <{@event.Name}>");
            _client.PurgeDeletedCertificate(@event.Name);
        }
    }
}
