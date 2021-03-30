using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using KeyVaultSecretStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;


namespace KeyvaultApi.Tests.Functions.Secrets
{
    public class KeyVaultSecretStoreTest
    {
        private KeyVaultSecretAdder _adder;
        private SecretClient _client;
        private ILoggerFactory _loggerFactory;
        private const string SecretName = "StoreTest";

        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            _client = new SecretClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            
            _adder = new KeyVaultSecretAdder(_client, _loggerFactory.CreateLogger<KeyVaultSecretAdder>());
        }
        
        
        [TearDown]
        public void TearDown()
        {
            try
            {
                var deletion = _client.StartDeleteSecret(SecretName).WaitForCompletionAsync().Result.Value;
            }
            catch
            {
                // ignored
            }

            try
            {
                _client.PurgeDeletedSecret(SecretName);
            }
            catch
            {
                // ignored
            }
        }

        [Test]
        public void StoreSecretToVault_ReturnsString()
        {
            _adder.StoreSecretToVault(SecretName,"secretStored!").Should().Be("The secret has been successfully added.");
        }
        
        [Test]
        public void StoreFaultySecretToVault_ReturnsString()
        {
           Action action = () => _adder.StoreSecretToVault(null,null).Should().Be("Sorry something went wrong, for further information contact your vault administrator");
           action.Should().Throw<NullReferenceException>()
               .WithMessage("Sorry something went wrong, for further information contact your vault administrator");
        }
    }
}