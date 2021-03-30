using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultCertificateDelete.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultCertificateDeleteController : ControllerBase
    {
        private readonly ILogger<KeyVaultCertificateDeleteController> _logger;
        private readonly KeyVaultCertificateDeleter _certificateDeleter;

        public KeyVaultCertificateDeleteController(ILogger<KeyVaultCertificateDeleteController> logger, KeyVaultCertificateDeleter certificateDeleter)
        {
            _logger = logger;
            _certificateDeleter = certificateDeleter;
        }


        [HttpPost]
        public IActionResult Post(KeyVaultCertificateDeleteEvent @event)
        {
            _certificateDeleter.Delete(@event);

            return Accepted();
        }
    }
}
