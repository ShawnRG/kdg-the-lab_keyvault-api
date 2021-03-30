using Azure.Identity;

namespace KeyVaultSecretGet
{
    public class KeyVaultOptions
    {
        public string Host { get; set; }
        public string AzureClientId { get; set; }
        public string AzureTenantId { get; set; }
        public string AzureClientSecret { get; set; }

        public ClientSecretCredential CreateCredential() =>
            new ClientSecretCredential(AzureTenantId, AzureClientId, AzureClientSecret);
    }
}
