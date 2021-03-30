using System;
using System.Text.Json.Serialization;

namespace KeyVaultSecretRotate.Controllers
{
    public class KeyVaultRotateEvent
    {
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Topic)}: {Topic}, {nameof(Subject)}: {Subject}, {nameof(EventType)}: {EventType}, {nameof(EventTime)}: {EventTime}, {nameof(EventData)}: {EventData}, {nameof(DataVersion)}: {DataVersion}, {nameof(MetadataVersion)}: {MetadataVersion}";
        }

        [JsonPropertyName("id")]
        public string Id { get; set; } 

        [JsonPropertyName("topic")]
        public string Topic { get; set; } 

        [JsonPropertyName("subject")]
        public string Subject { get; set; } 

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } 

        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; } 

        [JsonPropertyName("data")]
        public Data EventData { get; set; } 

        [JsonPropertyName("dataVersion")]
        public string DataVersion { get; set; } 

        [JsonPropertyName("metadataVersion")]
        public string MetadataVersion { get; set; } 
        
        public class Data    {
            
            [JsonPropertyName("Id")]
            public string Id { get; set; } 

            [JsonPropertyName("vaultName")]
            public string VaultName { get; set; } 

            [JsonPropertyName("objectType")]
            public string ObjectType { get; set; } 

            [JsonPropertyName("objectName")]
            public string ObjectName { get; set; } 

            [JsonPropertyName("version")]
            public string Version { get; set; } 

            [JsonPropertyName("nbf")]
            public string Nbf { get; set; } 

            [JsonPropertyName("exp")]
            public string Exp { get; set; }

            public override string ToString()
            {
                return $"{nameof(Id)}: {Id}, {nameof(VaultName)}: {VaultName}, {nameof(ObjectType)}: {ObjectType}, {nameof(ObjectName)}: {ObjectName}, {nameof(Version)}: {Version}, {nameof(Nbf)}: {Nbf}, {nameof(Exp)}: {Exp}";
            }
        }
        
        
    }
}
