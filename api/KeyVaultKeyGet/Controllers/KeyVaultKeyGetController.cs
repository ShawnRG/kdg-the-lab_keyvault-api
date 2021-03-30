using System;
using Azure.Security.KeyVault.Keys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyvaultKeyGet.Controllers
{
    
    [ApiController]
    [Route("/")]
    public class KeyVaultKeyGetController : ControllerBase

    {
        private readonly KeyVaultKeyRetriever _vaultKeyRetriever;

        public KeyVaultKeyGetController(ILogger<KeyVaultKeyGetController> logger,KeyVaultKeyRetriever vaultKeyRetriever)
        {
            _vaultKeyRetriever = vaultKeyRetriever;
            var hostname = Environment.GetEnvironmentVariable("keyvaultHostname");
            logger.LogInformation(
                $"Initiating KeyvaultGetController with hostname <{hostname}>");
        }

        [HttpPost]
        public IActionResult Post([FromQuery] string name)
        {
            return Ok(_vaultKeyRetriever.Get(name));
        }
    }
}