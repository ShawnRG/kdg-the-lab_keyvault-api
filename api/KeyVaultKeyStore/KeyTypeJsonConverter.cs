using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Security.KeyVault.Keys;

namespace KeyVaultKeyStore
{
    public class KeyTypeJsonConverter : JsonConverter<KeyType>
    {
        
        public override KeyType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new KeyType(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, KeyType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}