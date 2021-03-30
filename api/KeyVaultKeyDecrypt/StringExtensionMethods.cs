using System;
using System.Text;

namespace KeyVaultKeyDecrypt
{
    public static class StringExtensionMethods
    {
        public static byte[] GetBytes(this string s)
        {
            return Convert.FromBase64String(s);
        }

        public static string GetString(this byte[] b)
        {
            return Encoding.Unicode.GetString(b);
        }


    }
}