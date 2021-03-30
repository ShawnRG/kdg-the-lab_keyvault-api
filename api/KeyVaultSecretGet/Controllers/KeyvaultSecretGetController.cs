using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretGet.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultSecretGetController : ControllerBase
    {
        private readonly KeyVaultSecretRetriever _retriever;
        
        public KeyVaultSecretGetController(KeyVaultSecretRetriever retriever)
        {
            _retriever = retriever;
        }
        
        [HttpPost]
        public IActionResult Get([FromQuery] string name)
        {
            try
            {
                return Ok(_retriever.GetSecretFromVault(name));
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
            
        }
    }
}