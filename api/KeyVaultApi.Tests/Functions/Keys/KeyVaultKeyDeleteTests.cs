using System;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using FluentAssertions;
using KeyVaultKeyDelete;
using KeyVaultKeyDelete.Controllers;
using KeyVaultKeyStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using KeyVaultOptions = KeyvaultApi.Testing.KeyVaultOptions;

namespace KeyvaultApi.Tests.Functions.Keys
{
    public class KeyVaultKeyDeleteTests
    {
        private string _keyName;
        private KeyClient _client;
        private ILoggerFactory _loggerFactory;

        [SetUp]
        public void SetUp()
        {
            
            var serviceProvider = new ServiceCollection().AddLogging().BuildServiceProvider();
            var options = serviceProvider.GetService<IOptions<KeyVaultOptions>>().Value;

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            _client = new KeyClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            
            _keyName = Guid.NewGuid().ToString();

        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var deleted = _client.StartDeleteKey(_keyName).WaitForCompletionAsync().Result.Value;

            }
            catch
            {
                //ignored
            }

            try
            {
                _client.PurgeDeletedKey(_keyName);

            }
            catch
            {
                //ignored
            }
        }

        [Test]
        public void DeleteKeyFromVault_NoPurge_ReturnsDeletedKey()
        {
            var storeLogger = _loggerFactory.CreateLogger<KeyVaultKeyStorer>();
            var storer = new KeyVaultKeyStorer(_client, storeLogger);

            var deleteLogger = _loggerFactory.CreateLogger<KeyVaultKeyDeleter>();
            var deleter = new KeyVaultKeyDeleter(_client, deleteLogger);

            storer.Store(_keyName, KeyType.Ec);
            
            deleter.Delete(new KeyVaultKeyDeleteEvent
            {
                Name = _keyName,
                Purge = false
            });

            _client.GetDeletedKey(_keyName).Value.Should().NotBeNull();

        }

        [Test]
        public void DeleteKeyFromVault_Purge_CompletelyRemovedKey()
        {
            var storeLogger = _loggerFactory.CreateLogger<KeyVaultKeyStorer>();
            var storer = new KeyVaultKeyStorer(_client, storeLogger);

            var deleteLogger = _loggerFactory.CreateLogger<KeyVaultKeyDeleter>();
            var deleter = new KeyVaultKeyDeleter(_client, deleteLogger);

            storer.Store(_keyName, KeyType.Ec);
            
            deleter.Delete(new KeyVaultKeyDeleteEvent
            {
                Name = _keyName,
                Purge = true
            });

            Action action = () => _client.GetKey(_keyName);
            action.Should().ThrowExactly<RequestFailedException>().Where(e =>
                e.Message.Contains($"A key with (name/id) {_keyName} was not found in this key vault.",
                    StringComparison.InvariantCultureIgnoreCase));
            action = () => _client.GetDeletedKey(_keyName);

            action.Should().ThrowExactly<RequestFailedException>();
            
         
        }
    }
}