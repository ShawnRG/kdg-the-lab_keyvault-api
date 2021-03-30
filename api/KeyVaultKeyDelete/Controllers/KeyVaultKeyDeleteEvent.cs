namespace KeyVaultKeyDelete.Controllers
{
    public class KeyVaultKeyDeleteEvent
    {
        public string Name { get; set; }

        public bool Purge { get; set; }
    }
}