using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultCertificateStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultCertificateStoreController : ControllerBase
    {
        private readonly ILogger<KeyVaultCertificateStoreController> _logger;
        private readonly KeyVaultCertificateAdder _certificateAdder;

        public KeyVaultCertificateStoreController(ILogger<KeyVaultCertificateStoreController> logger, KeyVaultCertificateAdder certificateAdder)
        {
            _logger = logger;
            _certificateAdder = certificateAdder;
        }

        [HttpPost]
        public IActionResult Post(KeyVaultCertificateAddEvent @event)
        {
            _logger.LogInformation("Processing event: {Event}", @event.ToString());
            var addedCertificate = _certificateAdder.Add(@event.Name, @event.Enabled, @event.Tags);
            return Created("", addedCertificate);
        }
        
    }
}
