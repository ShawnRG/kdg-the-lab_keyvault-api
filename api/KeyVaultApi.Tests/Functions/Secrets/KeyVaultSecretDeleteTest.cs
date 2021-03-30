using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KeyVaultSecretDelete;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KeyvaultApi.Tests.Functions.Secrets
{
    public class KeyVaultSecretDeleteTest
    {
        private KeyVaultSecretDeleter _keyVaultSecretDeleter;
        private string _secretName;
        private SecretClient _client;
        private ILoggerFactory _loggerFactory;

        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            _client = new SecretClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            
            _keyVaultSecretDeleter = new KeyVaultSecretDeleter(_client, _loggerFactory.CreateLogger<KeyVaultSecretDeleter>());

            _secretName = Guid.NewGuid().ToString();
            _client.SetSecret(_secretName, "secretStored!");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var deletion = _client.StartDeleteSecret(_secretName).WaitForCompletionAsync().Result.Value;
            }
            catch
            {
                // ignored
            }

            try
            {
                _client.PurgeDeletedSecret(_secretName);
            }
            catch
            {
                // ignored
            }
        }


        [Test]
        public void DeleteSecretFromVaultWithout_ReturnsString()
        {
            _keyVaultSecretDeleter.DeleteSecretFromVault(_secretName, false).Should()
                .Be("The secret's delete operation has successfully started.");
        }

        [Test]
        public void DeleteAlreadyDeletedSecretFromVault_ReturnsString()
        {
            _keyVaultSecretDeleter.DeleteSecretFromVault(_secretName, false).Should()
                .Be("The secret's delete operation has successfully started.");

            _keyVaultSecretDeleter.DeleteSecretFromVault(_secretName, false).Should()
                .Contain("This secret has already been deleted and will be purged on: ");
        }

        [Test]
        public void DeleteSecretFromVaultWithPurge_ReturnsString()
        {
            _keyVaultSecretDeleter.DeleteSecretFromVault(_secretName, true).Should()
                .Be("The secret has been deleted and purged.");
        }

        [Test]
        public void DeleteFaultySecretFromVault_ReturnsString()
        {
            Action action = () => _keyVaultSecretDeleter.DeleteSecretFromVault("faulty", false).Should().Be("Sorry something went wrong, for further information contact your vault administrator");
            action.Should().Throw<NullReferenceException>()
                .WithMessage("Sorry something went wrong, for further information contact your vault administrator");
        }
    }
}