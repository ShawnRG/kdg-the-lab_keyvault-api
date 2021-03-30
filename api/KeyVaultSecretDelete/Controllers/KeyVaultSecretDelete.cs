using System;
using Microsoft.AspNetCore.Mvc;


namespace KeyVaultSecretDelete.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultSecretDelete : ControllerBase
    {
        private readonly KeyVaultSecretDeleter _keyVaultSecretDeleter;
        public KeyVaultSecretDelete(KeyVaultSecretDeleter keyVaultSecretDeleter)
        {
            this._keyVaultSecretDeleter = keyVaultSecretDeleter;
        }
        
        [HttpPost]
        public IActionResult Post(KeyVaultSecretDeleteEvent @event)
        {
            try
            {
                return Ok(_keyVaultSecretDeleter.DeleteSecretFromVault(@event.Name, @event.PurgeImmediately));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}