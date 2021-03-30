using System;
using System.Text;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using FluentAssertions;
using KeyVaultKeyEncrypt;
using KeyVaultKeyEncrypt.Controllers;
using KeyvaultKeyGet;
using KeyvaultKeyGet.Controllers;
using KeyVaultKeyStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Keys
{
    public class KeyVaultKeyEncryptTests
    {
        private string KeyName;
        private const string Data = "SomeDataToEncrypt";
        private ILoggerFactory _loggerFactory = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>();
        private KeyClient _client;
        private KeyVaultKey Key;
        private KeyVaultKeyStorer _storer;
        private KeyVaultKeyEncryptor _encryptor;


        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            KeyName = Guid.NewGuid().ToString();
            
            _client = new KeyClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            Key = _client.CreateKey(KeyName, KeyType.Rsa, new CreateRsaKeyOptions(KeyName)).Value;
            _storer = new KeyVaultKeyStorer(_client, _loggerFactory.CreateLogger<KeyVaultKeyStorer>());

            _encryptor = new KeyVaultKeyEncryptor(_loggerFactory.CreateLogger<KeyVaultKeyEncryptor>(),
                new CryptographyClientFactory(new DefaultAzureCredential()), _client);


        }

        [TearDown]
        public void TearDown()
        {
            var deletedKey = _client.StartDeleteKey(KeyName).WaitForCompletionAsync().Result.Value;
            _client.PurgeDeletedKey(deletedKey.Name);
        }
        [Test]
        public void Encrypt_ReturnsCipherText()
        {
            var cypherText = _encryptor.Encrypt(Key.Name, Data, "RSA1_5");
            cypherText.Should().NotBeNull();
        }
    }
}