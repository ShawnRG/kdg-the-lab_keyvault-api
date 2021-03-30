namespace KeyVaultSecretStore.Controllers
{
    public class KeyVaultSecretAddEvent
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool NewVersion { get; set; } = default;
    }
}