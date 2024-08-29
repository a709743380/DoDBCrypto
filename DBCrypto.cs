using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DoDBCrypto
{
    public class DBCrypto
    {
        public static string AESEncrypt(string plainText, string key, string iv)
        {
            bool isEncrypt = true;

            byte[] Key = Encoding.UTF8.GetBytes(key);
            byte[] IV = Encoding.UTF8.GetBytes(iv);
            byte[] data = switchEncryptInput(isEncrypt, plainText);
            byte[] byteArray = aesProcess(isEncrypt, data, Key, IV);

            return switchEncryptResult(isEncrypt, byteArray);

        }
        public static string AESDecrypt(string plainText, string key, string iv)
        {
            bool isEncrypt = false;

            byte[] Key = Encoding.UTF8.GetBytes(key);
            byte[] IV = Encoding.UTF8.GetBytes(iv);
            byte[] data = switchEncryptInput(isEncrypt, plainText);
            byte[] byteArray = aesProcess(isEncrypt, data, Key, IV);

            return switchEncryptResult(isEncrypt, byteArray);

        }
        public static string PwdHashEncrypt(string plainText, string key, string iv)
        {
            return HashSha256( key + plainText + iv);
        }

        private static string HashSha256(string content, int stringType = 0)
        {
            byte[] input = Encoding.UTF8.GetBytes(content);
            byte[] output = null;

            using (var provider = new SHA256CryptoServiceProvider())
            {
                output = provider.ComputeHash(input);
            }

            if (stringType == 0)
            {
                return BitConverter.ToString(output).Replace("-", string.Empty);
            }
            else
            {
                return Convert.ToBase64String(output);
            }
        }

        private static byte[] aesProcess(bool isEncrypt, byte[] str, byte[] key, byte[] iv)
        {
            byte[] result = null;

            using (var aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform cryptoTransform = null;
                if (isEncrypt)
                {
                    cryptoTransform = aes.CreateEncryptor();
                }
                else
                {
                    cryptoTransform = aes.CreateDecryptor();
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cs.Write(str, 0, str.Length);
                        cs.FlushFinalBlock();
                        result = ms.ToArray();
                    }
                }

                return result;
            }
        }
        private static byte[] switchEncryptInput(bool isEncrypt, string str)
        {
            byte[] result = null;

            if (!string.IsNullOrWhiteSpace(str))
            {
                if (isEncrypt)
                {
                    result = Encoding.UTF8.GetBytes(str);
                }
                else
                {
                    result = Convert.FromBase64String(str);
                }
            }

            return result;
        }
        private static string switchEncryptResult(bool isEncrypt, byte[] byteArray)
        {
            string result = null;

            if (byteArray != null && byteArray.Length > 0)
            {
                if (isEncrypt)
                {
                    result = Convert.ToBase64String(byteArray);
                }
                else
                {
                    result = Encoding.UTF8.GetString(byteArray);
                }
            }

            return result;
        }
    }
}
