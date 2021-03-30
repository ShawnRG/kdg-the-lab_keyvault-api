using System.Threading;
using Azure.Security.KeyVault.Keys;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyStore
{
    public class KeyVaultKeyStorer
    {
        private readonly KeyClient _client;
        private readonly ILogger<KeyVaultKeyStorer> _logger;

        public KeyVaultKeyStorer(KeyClient client, ILogger<KeyVaultKeyStorer> logger)
        {
            _client = client;
            _logger = logger;
        }

        public KeyVaultKey Store(string name, KeyType keyType, CreateKeyOptions keyOptions = null)
        {
            _logger.LogInformation("Storing key: {Name}", name);
            return _client.CreateKey(name, keyType, keyOptions);
        }
    }
}