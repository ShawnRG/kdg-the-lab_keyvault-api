using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyDecrypt.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultKeyDecryptController : ControllerBase
    {
        private readonly ILogger<KeyVaultKeyDecryptController> _logger;
        private readonly KeyVaultKeyDecryptor _decryptor;

        public KeyVaultKeyDecryptController(ILogger<KeyVaultKeyDecryptController> logger, KeyVaultKeyDecryptor decryptor)
        {
            _logger = logger;
            _decryptor = decryptor;
        }

        [HttpPost]
        public IActionResult Post(KeyVaultKeyDecryptEvent @event)
        {
            _logger.LogInformation("Controller calling KeyVaultKeyDecryptor.Dencrypt with: KeyName = {KeyName}, Algorithm = {Algorithm}",@event.Name, @event.Algorithm);

            var result = _decryptor.Decrypt(@event.Name, @event.Data, @event.Algorithm);

            var plain = result.Plaintext.GetString();
            
            return Ok(plain);
        }
        
        
    }
}