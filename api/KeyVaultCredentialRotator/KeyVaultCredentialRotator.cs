using System;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using CloudNative.CloudEvents;
using Microsoft.Extensions.Logging;

namespace KeyVaultCredentialRotator
{
    public class KeyVaultCredentialRotator
    {
        private readonly ILogger<KeyVaultCredentialRotator> _logger;
        private readonly SecretClient _secretClient;
        private readonly KeyClient _keyClient;

        public KeyVaultCredentialRotator(KeyClient keyClient, SecretClient secretClient, ILogger<KeyVaultCredentialRotator> logger)
        {
            _keyClient = keyClient;
            _secretClient = secretClient;
            _logger = logger;
        }

        public void Rotate(int days)
        {
            _logger.LogInformation("Starting scheduled credential rotation");

            _logger.LogInformation("Retrieving properties for secrets that are expiring within {Days} days", days);
            _secretClient.GetPropertiesOfSecrets().Where(properties =>
                (properties.ExpiresOn - DateTimeOffset.Now).GetValueOrDefault(TimeSpan.MaxValue).TotalDays <= days).Select(properties => properties.Name).ToList().ForEach(
                name =>
                {
                    try
                    {
                        SendCloudEventForCertificate(name);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Could not successfully rotate secret {Name}", name);
                    }
                });
            
            _logger.LogInformation("Rotating complete");
            
        }

        private async void SendCloudEventForCertificate(string name)
        {
            _logger.LogInformation("Sending cloud event for secret {Name}", name);
            var cloudEvent = new CloudEvent("Microsoft.KeyVault.SecretExpired",
                new Uri("https://labrats.shawnross.dev/async-function/keyvault-credential-rotator"))
            {
                DataContentType = new ContentType(MediaTypeNames.Application.Json),
                Data = new CertificateExpiryEvent
                {
                    ObjectType = "Secret",
                    ObjectName = name
                }
            };

            _logger.LogInformation("Creating content type");
            var content = new CloudEventContent(cloudEvent, ContentMode.Structured, new JsonEventFormatter());
            
            var httpClient = new HttpClient();
            _logger.LogInformation("Sending request");
            var result =
                await httpClient.PostAsync("https://labrats.shawnross.dev/async-function/keyvault-secret-rotate",
                    // I know this is kind of scuffed, but this is the best I can do currently with the limited timespan.
                    new StringContent("[" + await content.ReadAsStringAsync() + "]", Encoding.UTF8, "application/json"));
            _logger.LogInformation("Request {Request}", await result.RequestMessage.Content.ReadAsStringAsync());

            if (result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully rotated secret");
            }
            else
            {
                var resultString = await result.Content.ReadAsStringAsync();
                _logger.LogError("Result unsuccessful: \n{Result}", resultString);
            }
        }

        public class CertificateExpiryEvent
        {
            public string ObjectType { get; set; }
            public string ObjectName { get; set; }
        }
    }
}
