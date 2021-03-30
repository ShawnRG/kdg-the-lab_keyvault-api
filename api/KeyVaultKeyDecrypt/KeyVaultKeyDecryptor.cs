using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyDecrypt
{
    public class KeyVaultKeyDecryptor
    {
        private readonly ILogger<KeyVaultKeyDecryptor> _logger;
        private readonly CryptographyClientFactory _clientFactory;
        private readonly KeyClient _keyClient;

        public KeyVaultKeyDecryptor(ILogger<KeyVaultKeyDecryptor> logger, CryptographyClientFactory clientFactory, KeyClient keyClient)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _keyClient = keyClient;
        }

        public DecryptResult Decrypt(string keyName, string encryptedData , string algorithm)
        {
            _logger.LogInformation("Getting Key with name: {KeyName}", keyName);
            var key = _keyClient.GetKey(keyName).Value;

            var cryptoClient = _clientFactory.CreateCryptographyClient(key.Id.ToString());
            
            _logger.LogInformation("Decrypting data with keyid {Id}",key.Id.ToString());

            return cryptoClient.Decrypt(new EncryptionAlgorithm(algorithm ?? "RSA1_5"), encryptedData.GetBytes());
        }
    }
}