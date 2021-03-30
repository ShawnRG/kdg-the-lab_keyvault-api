namespace KeyVaultSecretDelete
{
    public class KeyVaultSecretDeleteEvent
    {
        public string Name { get; set; }
        public bool PurgeImmediately { get; set; } = default;
    }
}