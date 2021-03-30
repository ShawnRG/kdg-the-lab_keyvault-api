using System;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretDelete
{
    public class KeyVaultSecretDeleter
    {
        private readonly SecretClient _client;
        private readonly ILogger<KeyVaultSecretDeleter> _logger;


        public KeyVaultSecretDeleter(SecretClient client, ILogger<KeyVaultSecretDeleter> logger)
        {
            _client = client;
            _logger = logger;
        }

        public string DeleteSecretFromVault(string secretName, bool purgeImmediately)
        {
            if (purgeImmediately)
            {
                return DeleteAndPurgeSecret(secretName);
            }
            
            var checkAlreadyDeleted = CheckIfAlreadyInDeletion(secretName);
            if (checkAlreadyDeleted != null)
            {
                _logger.LogInformation("SECRET: {SecretName} has already been deleted and will be purged on: {CheckAlreadyDeleted}",secretName,checkAlreadyDeleted);
                return $"This secret has already been deleted and will be purged on: {checkAlreadyDeleted}";
            }

            try
            {
                var deletion = _client.StartDeleteSecret(secretName).WaitForCompletionAsync().Result;
                _logger.LogInformation("Deleting: SECRET with Name: {SecretName}",secretName);
                    return "The secret's delete operation has successfully started.";
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"ERROR: Deleting SECRET with name: {SecretName} threw an Exception: {ExceptionType}",secretName,e.GetType());
                throw new NullReferenceException("Sorry something went wrong, for further information contact your vault administrator");
            }
        }

        private string DeleteAndPurgeSecret(string secretName)
        {
            try
            {
                var deletedSecret = _client.StartDeleteSecret(secretName).WaitForCompletionAsync().Result.Value;
                _client.PurgeDeletedSecret(secretName);
                _logger.LogInformation("Deleting and Purging: SECRET with Name: {SecretName}",secretName);
                return "The secret has been deleted and purged.";
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"ERROR: Deleting and purging SECRET with name: {SecretName} threw an Exception: {ExceptionType}",secretName,e.GetType());
                throw new NullReferenceException("Sorry something went wrong, for further information contact your vault administrator");
            }
        }

        private DateTimeOffset? CheckIfAlreadyInDeletion(string secretName)
        {
            try
            {
                var response = _client.GetDeletedSecret(secretName).Value;

                return response?.ScheduledPurgeDate;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e,"ERROR: in checking ALREADY DELETION of SECRET with name: {SecretName} threw an Exception: {ExceptionType}",secretName,e.GetType());
                return null;
            }
        }
    }
}