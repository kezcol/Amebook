using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Amebook.Crypto
{
    public class TextEncryption
    {
        // TODO
        static byte[] AesEncrypt(String text, byte[] Key, byte[] IV)
        {
            byte[] cipher;
            if (text == null || text.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.IV = IV;
                aesAlg.Key = Key;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        cipher = msEncrypt.ToArray();
                    }
                }
            }
            return cipher;
        }
        static string AesDecrypt(byte[] encryptedMsg, byte[] Key, byte[] IV)
        {
            string text = null;
            if (encryptedMsg == null || encryptedMsg.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.IV = IV;
                aesAlg.Key = Key;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(encryptedMsg))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            text = srDecrypt.ReadToEnd();
                        }

                    }
                }
            }
            return text;
        }
        void AesTest()
        {
            try
            {
                string text = "Here is some data to encrypt, lol";
                using (Aes myAes = Aes.Create())
                {
                    byte[] encrypted_data = AesEncrypt(text, myAes.Key, myAes.IV);
                    string plaintext = AesDecrypt(encrypted_data, myAes.Key, myAes.IV);
                    //Console.WriteLine(plaintext);
                }
              //  Console.ReadKey();
            }
            catch (Exception e)
            {
               // Console.WriteLine(e.Message);
            }
        }
    }
}