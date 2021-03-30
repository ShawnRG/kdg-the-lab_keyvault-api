using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultCredentialRotator.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultCredentialRotatorController : ControllerBase
    {

        private readonly ILogger<KeyVaultCredentialRotatorController> _logger;
        private readonly KeyVaultCredentialRotator _rotator;

        public KeyVaultCredentialRotatorController(ILogger<KeyVaultCredentialRotatorController> logger, KeyVaultCredentialRotator rotator)
        {
            _logger = logger;
            _rotator = rotator;
        }

        [HttpPost]
        public IActionResult Rotate([FromQuery] int days)
        {
            _rotator.Rotate(days);
            return Ok();
        }
    }
}
