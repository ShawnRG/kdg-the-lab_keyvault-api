using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultSecretStore.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultSecretStoreController : ControllerBase
    {
        private readonly KeyVaultSecretAdder _adder;

        public KeyVaultSecretStoreController(KeyVaultSecretAdder adder)
        {
            _adder = adder;
        }

        [HttpPost]
        public IActionResult Post(KeyVaultSecretAddEvent @event)
        {
            try
            {
                if (@event.NewVersion)
                {
                    return Ok(_adder.StoreUpdatedSecretToVault(@event.Name, @event.Value));
                } 
                return Created("",_adder.StoreSecretToVault(@event.Name, @event.Value));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotFound(e.Message);
            }
        }
    }
}