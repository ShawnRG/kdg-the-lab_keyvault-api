using System;
using System.Text.Json;
using Azure.Security.KeyVault.Keys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultKeyStoreController :  ControllerBase
    {
        private readonly KeyVaultKeyStorer _keyStorer;
        private readonly ILogger<KeyVaultKeyStoreController> _logger;

        public KeyVaultKeyStoreController(KeyVaultKeyStorer keyStorer, ILogger<KeyVaultKeyStoreController> logger)
        {
            _keyStorer = keyStorer;
            _logger = logger;
        }


        [HttpPost]
        public IActionResult Post(KeyVaultKeyStoreEvent @event)
        {
            var storedKey = _keyStorer.Store(@event.Name, @event.KeyType, @event.KeyOptions);
            _logger.LogInformation("Created key with parameters: {Name}, {KeyType}, {KeyOptions}", @event.Name, @event.KeyType, @event.KeyOptions);
            return Created("", storedKey);
        }
    }
    
    
}