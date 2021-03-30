using System;
using System.IO;
using System.Threading;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using FluentAssertions;
using KeyVaultCertificateDelete;
using KeyVaultCertificateStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Certificates
{
    public class KeyvaultCertificateDeleteTests
    {
        private string _certificateName;
        private CertificateClient _client;
        private ILoggerFactory _loggerFactory;

        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");

            _client = new CertificateClient(new Uri(hostname ?? throw new ArgumentException()),
                new DefaultAzureCredential());
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _certificateName = Guid.NewGuid().ToString();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var deletedCertificate = _client.StartDeleteCertificate(_certificateName).WaitForCompletionAsync().Result
                    .Value;
            }
            catch
            {
                // ignored
            }

            try
            {
                var response = _client.PurgeDeletedCertificate(_certificateName);
                Console.WriteLine(new StreamReader(response.ContentStream!).ReadToEnd());
            }
            catch
            {
                // ignored
            }
        }

        [Test]
        public void Delete_NoPurge_AddCertificateToDeletedCertificates()
        {
            var deleteLogger = _loggerFactory.CreateLogger<KeyVaultCertificateDeleter>();
            var addLogger = _loggerFactory.CreateLogger<KeyVaultCertificateAdder>();
            var adder = new KeyVaultCertificateAdder(_client, addLogger);
            adder.Add(_certificateName);
            var deleter = new KeyVaultCertificateDeleter(_client, deleteLogger);

            deleter.Delete(new KeyVaultCertificateDeleteEvent
            {
                Name = _certificateName
            });

            _client.GetDeletedCertificate(_certificateName).Value.Should().NotBeNull();
        }

        [Test]
        public void Delete_Purge_CompletelyRemovedCertificate()
        {
            var deleteLogger = _loggerFactory.CreateLogger<KeyVaultCertificateDeleter>();
            var addLogger = _loggerFactory.CreateLogger<KeyVaultCertificateAdder>();
            var adder = new KeyVaultCertificateAdder(_client, addLogger);
            adder.Add(_certificateName);
            var deleter = new KeyVaultCertificateDeleter(_client, deleteLogger);

            deleter.Delete(new KeyVaultCertificateDeleteEvent
            {
                Name = _certificateName,
                Purge = true
            });

            Action action = () => _client.GetCertificate(_certificateName);
            action.Should().ThrowExactly<RequestFailedException>().Where(e =>
                e.Message.Contains($"A certificate with (name/id) {_certificateName} was not found in this key vault.",
                    StringComparison.InvariantCultureIgnoreCase));
            action = () => _client.GetDeletedCertificate(_certificateName);

            action.Should().ThrowExactly<RequestFailedException>();
        }
    }
}
