using System;
using System.Threading;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using FluentAssertions;
using KeyVaultCertificateGet;
using NUnit.Framework;

namespace KeyvaultApi.Tests.Functions.Certificates
{
    public class KeyvaultCertificateGetTests
    {
        private const string CertificateName = "TestCase1";
        private CertificateClient _client;
        private KeyVaultCertificateRetriever _retriever;
        
        
        [SetUp]
        public void SetUp()
        {
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");

            _client = new CertificateClient(new Uri(hostname ?? throw new ArgumentException()), new DefaultAzureCredential());

            _client.StartCreateCertificate(CertificateName, CertificatePolicy.Default, true);

            _retriever = new KeyVaultCertificateRetriever(_client);
        }

        [TearDown]
        public void TearDown()
        {
            var deletedCertificate = _client.StartDeleteCertificate(CertificateName).WaitForCompletionAsync().Result.Value;
            _client.PurgeDeletedCertificate(deletedCertificate.Name);
        }

        [Test]
        public void Get_ReturnsCertificate()
        {
            var certificate = _retriever.Get(CertificateName);
            certificate.Should().NotBeNull();
            certificate.Name.Should().Be(CertificateName);
            certificate.Policy.Enabled.Should().BeTrue();
        }
    }
}
