

using System;
using System.Diagnostics;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using FluentAssertions;
using KeyvaultKeyGet;
using KeyvaultKeyGet.Controllers;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Keys
{
    public class KeyvaultKeyGetTests
    {
        private const string KeyName = "JustAKey";
        private KeyClient _client;
        private KeyVaultKeyRetriever _vaultKeyRetriever;
        private KeyVaultKeyGetController _controller;

        
        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            _client = new KeyClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            _client.CreateKey(KeyName, KeyType.Ec, new CreateEcKeyOptions(KeyName, true));
            _vaultKeyRetriever = new KeyVaultKeyRetriever(_client);

        }

        [TearDown]
        public void TearDown()
        {
            var deletedKey = _client.StartDeleteKey(KeyName).WaitForCompletionAsync().Result.Value;
            _client.PurgeDeletedKey(deletedKey.Name);
        }

        [Test]
        public void Get_ReturnsKey()
        {
            var key = _vaultKeyRetriever.Get(KeyName);
            key.Should().NotBeNull();
            key.Name.Should().Be(KeyName);
            key.KeyType.Should().Be(KeyType.Ec);

        }

    }
}