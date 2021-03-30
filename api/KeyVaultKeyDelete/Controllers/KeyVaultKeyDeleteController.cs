using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KeyVaultKeyDelete.Controllers
{
    [ApiController]
    [Route("/")]
    public class KeyVaultKeyDeleteController : ControllerBase
    {
        private readonly KeyVaultKeyDeleter _deleter;
        private readonly ILogger<KeyVaultKeyDeleteController> _logger;

        public KeyVaultKeyDeleteController(KeyVaultKeyDeleter deleter, ILogger<KeyVaultKeyDeleteController> logger)
        {
            _deleter = deleter;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post(KeyVaultKeyDeleteEvent @event)
        {
            _logger.LogInformation("Deleting key with name {Name} and purge = {Purge}", @event.Name, @event.Purge);
            _deleter.Delete(@event);
            return Accepted();
        }
    }
}