using System;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretStore
{
    public class KeyVaultSecretAdder
    {
        private readonly SecretClient _client;
        private readonly ILogger<KeyVaultSecretAdder> _logger;


        public KeyVaultSecretAdder(SecretClient client, ILogger<KeyVaultSecretAdder> logger)
        {
            _client = client;
            _logger = logger;
        }

        public string StoreSecretToVault(string secretName, string secretValue)
        {
         
            
            try
            {
                if (LookUpSecret(secretName))
                {
                    throw new NullReferenceException("Sorry the secret you're trying to add already exists.");
                }

                _client.SetSecret(secretName,secretValue);
                
                _logger.LogInformation("Storing: SECRET for User: "+ Environment.GetEnvironmentVariable("AZURE_CLIENT_ID") + " With Name: "+ secretName +" and Value: "+ secretValue);
                return "The secret has been successfully added.";
            }
            catch (Exception e)
            {
                _logger.LogCritical("ERROR: Storing SECRET with Exception" +  e.Message);
                throw new NullReferenceException("Sorry something went wrong, for further information contact your vault administrator");
            }
            
           
        }

        private bool LookUpSecret(string secretName)
        {
            try
            {
                KeyVaultSecret  lookupSecret = _client.GetSecret(secretName).Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string StoreUpdatedSecretToVault(string secretName, string secretValue)
        {
            try
            {
                if (!LookUpSecret(secretName))
                {
                    throw new InvalidOperationException("Sorry can't update a secret that doesn't exist yet.");
                }
                
                _client.SetSecret(secretName,secretValue);
                
                _logger.LogInformation("Updating: SECRET With Name: {SecretName} and Value: {SecretValue}",secretName,secretValue);
                return "The secret has been successfully updated.";
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"ERROR: Storing SECRET with Exception: {ExceptionType}",e.GetType());
                throw new NullReferenceException("Sorry something went wrong, for further information contact your vault administrator");
            }
        }
    }
}