using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyEncrypt.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultKeyEncryptController : ControllerBase
    {
        private readonly ILogger<KeyVaultKeyEncryptController> _logger;
        private readonly KeyVaultKeyEncryptor _encryptor;

        public KeyVaultKeyEncryptController(ILogger<KeyVaultKeyEncryptController> logger, KeyVaultKeyEncryptor encryptor)
        {
            _logger = logger;
            _encryptor = encryptor;
        }

        [HttpPost]
        public IActionResult Post(KeyVaultKeyEncryptEvent @event)
        {
            _logger.LogInformation("Controller calling KeyVaultKeyEncryptor.Encrypt with: KeyName = {KeyName}, Algorithm = {Algorithm}",@event.Name, @event.Algorithm);
            var result = _encryptor.Encrypt(@event.Name, @event.Data, @event.Algorithm);
            var cipher = Convert.ToBase64String(result.Ciphertext);
            return Ok(cipher);
        }
    }
}