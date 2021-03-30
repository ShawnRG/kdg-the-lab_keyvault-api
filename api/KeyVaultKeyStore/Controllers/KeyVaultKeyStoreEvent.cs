using System.Text.Json.Serialization;
using System.Threading;
using Azure.Security.KeyVault.Keys;

namespace KeyVaultKeyStore
{
    public class KeyVaultKeyStoreEvent
    {
        public string Name { get; set; }

        [JsonConverter(typeof(KeyTypeJsonConverter))]
        public KeyType KeyType { get; set; }
        
        public CreateKeyOptions KeyOptions { get; set; }
        
    }
}
