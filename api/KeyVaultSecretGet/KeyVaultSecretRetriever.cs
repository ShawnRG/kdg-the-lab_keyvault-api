using System;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretGet
{
    public class KeyVaultSecretRetriever
    {
        private readonly SecretClient _client;
        private readonly ILogger<KeyVaultSecretRetriever> _logger;


        public KeyVaultSecretRetriever(SecretClient client, ILogger<KeyVaultSecretRetriever> logger)
        {
            _client = client;
            _logger = logger;
        }
        
        public string GetSecretFromVault(string secretName)
        {
            try
            {
                var response = _client.GetSecret(secretName);

                _logger.LogInformation("Retrieving: SECRET with Name: {SecretName}", secretName);
                return response.Value.Value;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"ERROR: Retrieving SECRET with name: {SecretName} threw an Exception: {ExceptionType}",secretName,e.GetType());
                throw new NullReferenceException("Sorry something went wrong, for further information contact your vault administrator");

            }
        }
    }
    
}