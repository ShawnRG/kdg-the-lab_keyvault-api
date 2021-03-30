using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using KeyVaultSecretGet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Secrets
{
    public class KeyVaultSecretGetTest
    {
        private SecretClient _client;
        private string _secretName;
        private KeyVaultSecretRetriever _keyVaultSecretGet;
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
            
            _keyVaultSecretGet = new KeyVaultSecretRetriever(_client, _loggerFactory.CreateLogger<KeyVaultSecretRetriever>());

            
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
        public void GetSecretFromVault_ReturnsString()
        {
            _keyVaultSecretGet.GetSecretFromVault(_secretName).Should().Be("secretStored!");
        }
        
        [Test]
        public void GetFaultySecretFromVault_ReturnsString()
        {
            Action action = () =>  _keyVaultSecretGet.GetSecretFromVault("sqlConnectione");
            action.Should().Throw<NullReferenceException>()
                .WithMessage("Sorry something went wrong, for further information contact your vault administrator");
        }
    }
}