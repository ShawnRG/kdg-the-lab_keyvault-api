using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretRotate.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultSecretRotateController : ControllerBase
    {
        private readonly ILogger<KeyVaultSecretRotateController> _logger;
        private readonly KeyVaultSecretRotator _rotator;
        
        public KeyVaultSecretRotateController(ILogger<KeyVaultSecretRotateController> logger, KeyVaultSecretRotator rotator)
        {
            _logger = logger;
            _rotator = rotator;
        }
        
        [HttpPost]
        public IActionResult Post([FromBody] List<KeyVaultRotateEvent> @event)
        {
            try
            {
                foreach (var keyVaultRotateEvent in @event.Where(keyVaultRotateEvent => keyVaultRotateEvent.EventData?.ObjectType.Equals("Secret") ?? false))
                {
                    _logger.LogInformation("Processing event {Event}", keyVaultRotateEvent);
                    _rotator.Rotate(keyVaultRotateEvent.EventData.ObjectName);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            
            
        }
    }
}