using System;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using FluentAssertions;
using KeyVaultCertificateStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Certificates
{
    public class KeyvaultCertificateStoreTests
    {
        private const string CertificateName = "AddTestCase1";
        private CertificateClient _client;
        private ILoggerFactory _loggerFactory;
        private KeyVaultCertificateAdder _adder;
        
        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");

            _client = new CertificateClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider();

            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            _adder = new KeyVaultCertificateAdder(_client, _loggerFactory.CreateLogger<KeyVaultCertificateAdder>());
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var deletedCertificate = _client.StartDeleteCertificate(CertificateName).WaitForCompletionAsync().Result.Value;
                _client.PurgeDeletedCertificate(deletedCertificate.Name);
            }
            catch
            {
                // ignored
            }
        }

        [Test]
        public void Add_AddsCertificate()
        {
            _adder.Add(CertificateName);

            var certificate = _client.GetCertificate(CertificateName).Value;
            certificate.Should().NotBeNull();
            certificate.Name.Should().Be(CertificateName);
        }

    }
}
