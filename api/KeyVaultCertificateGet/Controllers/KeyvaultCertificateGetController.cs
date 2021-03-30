using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Certificates;
using KeyVaultCertificateGet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultCertificateGet.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultCertificateGetController : ControllerBase
    {
        private readonly KeyVaultCertificateRetriever _retriever;
        public KeyVaultCertificateGetController(ILogger<KeyVaultCertificateGetController> logger, KeyVaultCertificateRetriever retriever)
        {
            _retriever = retriever;
            var environmentVariable = Environment.GetEnvironmentVariable("keyvaultHostname");
            logger.LogInformation(
                $"Initiating controller with hostname <{environmentVariable}>");
        }

        [HttpPost]
        public IActionResult Get([FromQuery]string name) => Ok(_retriever.Get(name).Cer);
        
    }
}
