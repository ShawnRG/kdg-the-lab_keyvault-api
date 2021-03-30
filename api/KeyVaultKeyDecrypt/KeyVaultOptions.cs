using Azure.Core;
using Azure.Identity;

namespace KeyVaultKeyDecrypt
{
    public class KeyVaultOptions
    {
        public string Host { get; set; }
        public string AzureClientId { get; set; }
        public string AzureTenantId { get; set; }
        public string AzureClientSecret { get; set; }

        public TokenCredential CreateCredential() =>
            new ClientSecretCredential(AzureTenantId, AzureClientId, AzureClientSecret);
    }
}