using System;
using Azure.Security.KeyVault.Keys;
using KeyVaultKeyDelete.Controllers;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyDelete
{
    public class KeyVaultKeyDeleter
    {
        private readonly KeyClient _client;
        private readonly ILogger<KeyVaultKeyDeleter> _logger;

        public KeyVaultKeyDeleter(KeyClient client, ILogger<KeyVaultKeyDeleter> logger)
        {
            _client = client;
            _logger = logger;
        }

        public void Delete(KeyVaultKeyDeleteEvent @event)
        {
            _logger.LogInformation("Deleting key with name {Name}", @event.Name);
            var deleteOperation = _client.StartDeleteKey(@event.Name).WaitForCompletionAsync().Result.Value;
            
            if(!@event.Purge) return;
            Purge(@event);
            
        }

        private void Purge(KeyVaultKeyDeleteEvent @event)
        {
            _logger.LogInformation("Purging key with name {Name}", @event.Name);
            _client.PurgeDeletedKey(@event.Name);
        }
    }
}