using System.Collections.Generic;

namespace KeyVaultCertificateStore.Controllers
{
    public class KeyVaultCertificateAddEvent
    {
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public Dictionary<string, string> Tags { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Enabled)}: {Enabled}, {nameof(Tags)}: {Tags}";
        }
    }
}
