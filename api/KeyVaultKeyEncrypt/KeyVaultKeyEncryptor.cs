using System.Text;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyEncrypt
{
    public class KeyVaultKeyEncryptor
    {
        private readonly ILogger<KeyVaultKeyEncryptor> _logger;
        private readonly CryptographyClientFactory _factory;
        private readonly KeyClient _client;

        public KeyVaultKeyEncryptor(ILogger<KeyVaultKeyEncryptor> logger, CryptographyClientFactory factory, KeyClient client)
        {
            _logger = logger;
            _factory = factory;
            _client = client;
        }

        public EncryptResult Encrypt(string keyName, string data, string algorithm )
        {
            _logger.LogInformation("Getting Key with name: {keyName}", keyName);
            var key = _client.GetKey(keyName).Value;
            
            
            _logger.LogInformation("Encrypting data with keyid {Id}",key.Id.ToString());
            var client = _factory.CreateCryptographyClient(key.Id.ToString());
            return client.Encrypt(new EncryptionAlgorithm(algorithm ?? "RSA1_5"), data.JsonDataToByteArray());
        }
    }
}