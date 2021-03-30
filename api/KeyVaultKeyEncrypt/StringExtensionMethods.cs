using System.Text;

namespace KeyVaultKeyEncrypt
{
    public static class StringExtensionMethods
    {
        public static byte[] JsonDataToByteArray(this string s)
        {
            return Encoding.Unicode.GetBytes(s);
        }
        
        public static string ByteArrayToString(this byte[] s)
        {
            return Encoding.Unicode.GetString(s);
        }
    }
}