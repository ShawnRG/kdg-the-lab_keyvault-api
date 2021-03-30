using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretRotate
{
    public class KeyVaultSecretRotator
    {
        private const string CredentialIdTag = "CredentialId";
        private const string DataSourceTag = "DataSource";
        private const string ValidityPeriodDaysTag = "ValidityPeriodDays";
        
        private readonly ILogger<KeyVaultSecretRotator> _logger;
        private readonly SecretClient _client;

        public KeyVaultSecretRotator(ILogger<KeyVaultSecretRotator> logger, SecretClient client)
        {
            _logger = logger;
            _client = client;
        }
        
        public void Rotate(string secretName)
        {
            KeyVaultSecret secret= _client.GetSecret(secretName);
            _logger.LogInformation("Secret Info Retrieved");
            
            //Retrieve Secret Info
            var userId = secret.Properties.Tags.ContainsKey(CredentialIdTag) ? secret.Properties.Tags[CredentialIdTag] : "";
            var dataSource = secret.Properties.Tags.ContainsKey(DataSourceTag) ? secret.Properties.Tags[DataSourceTag] : "";
            _logger.LogInformation("Provider Address: {DataSource}", dataSource);
            _logger.LogInformation("Credential Id: {UserId}", userId);

            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userId,
                Password = secret.Value
            }.ConnectionString;
            
            _logger.LogInformation("Checking service connection...");
            CheckServiceConnection(secret, connectionString);
            _logger.LogInformation("Service connection validated");

            var newPassword = CreateRandomPassword();
            _logger.LogInformation("New password generated");
            
            CreateNewSecretVersion(secret, newPassword);
            _logger.LogInformation("New Secret version generated");
            
            UpdateServicePassword(secret, connectionString, newPassword);
            _logger.LogInformation("Updated service password");
            
            _logger.LogInformation("Secret rotated successfully");
        }

        private void UpdateServicePassword(KeyVaultSecret secret, string connectionString, string newPassword)
        {
            var userId = secret.Properties.Tags.ContainsKey(CredentialIdTag) ? secret.Properties.Tags[CredentialIdTag] : "";
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand($"ALTER LOGIN {userId} WITH Password='{newPassword}';", connection);
            command.ExecuteNonQuery();
        }

        private void CreateNewSecretVersion(KeyVaultSecret secret, string newPassword)
        {
            var credentialId = secret.Properties.Tags.ContainsKey(CredentialIdTag) ? secret.Properties.Tags[CredentialIdTag] : "";
            var dataSource = secret.Properties.Tags.ContainsKey(DataSourceTag) ? secret.Properties.Tags[DataSourceTag] : "";
            var validityPeriodDays = secret.Properties.Tags.ContainsKey(ValidityPeriodDaysTag) ? secret.Properties.Tags[ValidityPeriodDaysTag] : "60";
            
            var newSecret = new KeyVaultSecret(secret.Name, newPassword);
            newSecret.Properties.Tags.Add(CredentialIdTag, credentialId);
            newSecret.Properties.Tags.Add(DataSourceTag, dataSource);
            newSecret.Properties.Tags.Add(ValidityPeriodDaysTag, validityPeriodDays);
            newSecret.Properties.ExpiresOn = DateTime.UtcNow.AddDays(int.Parse(validityPeriodDays));
            _client.SetSecret(newSecret);
        }
        
        private static string CreateRandomPassword()
        {
            const int length = 60;
            
            var randomBytes = new byte[length];
            RNGCryptoServiceProvider rngCrypt = new RNGCryptoServiceProvider();
            rngCrypt.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        
        private static void CheckServiceConnection(KeyVaultSecret secret, string connectionString)
        {
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
        }
    }
}
