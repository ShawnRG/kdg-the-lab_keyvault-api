using System;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using FluentAssertions;
using KeyVaultKeyStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Keys
{
    public class KeyVaultKeyStoreTests
    {
        private const string KeyName = "JustAnotherKey";
        private KeyClient _client;
        private KeyVaultKeyStorer _storer;
        private ILoggerFactory _loggerFactory = new ServiceCollection().AddLogging().BuildServiceProvider().GetService<ILoggerFactory>();


        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            
            _client = new KeyClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            _storer = new KeyVaultKeyStorer(_client, _loggerFactory.CreateLogger<KeyVaultKeyStorer>());
        }

        [TearDown]
        public void TearDown()
        {
            var deletedKey = _client.StartDeleteKey(KeyName).WaitForCompletionAsync().Result.Value;
            _client.PurgeDeletedKey(deletedKey.Name);
        }

        [Test]
        public void Store_StoreKey()
        {
            var storedKey = _storer.Store(KeyName, KeyType.Ec);
            storedKey.Should().NotBeNull();
            storedKey.Name.Should().Be(KeyName);

        }
        
    }
}