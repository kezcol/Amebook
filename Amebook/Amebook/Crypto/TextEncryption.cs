using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Amebook.Models;

namespace Amebook.Crypto
{
    public static class TextEncryption
    {
        public static byte[] AesEncrypt(String text, byte[] Key, byte[] IV)
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

        static byte[] AesKeyEncrypt(byte[] Key, byte[] IV, RSAParameters KeyInfo)
        {
            byte[] encryptedData;
            byte[] aesKey = new byte[Key.Length + IV.Length];
            Console.WriteLine(Key.Length + " " + IV.Length);
            System.Buffer.BlockCopy(Key, 0, aesKey, 0, Key.Length);
            System.Buffer.BlockCopy(IV, 0, aesKey, Key.Length, IV.Length);
            using (RSACryptoServiceProvider rsaAlg = new RSACryptoServiceProvider())
            {
                rsaAlg.ImportParameters(KeyInfo);
                encryptedData = rsaAlg.Encrypt(aesKey, true);
            }
            return encryptedData;
        }
        static byte[][] DecryptAesKey(byte[] encryptedData, RSAParameters KeyInfo)
        {
            byte[][] aesKeys = new byte[2][];
            aesKeys[0] = new byte[32];
            aesKeys[1] = new byte[16];
            using (RSACryptoServiceProvider rsaAlg = new RSACryptoServiceProvider())
            {
                rsaAlg.ImportParameters(KeyInfo);
                byte[] tmp = rsaAlg.Decrypt(encryptedData, true);
                System.Buffer.BlockCopy(tmp, 0, aesKeys[0], 0, 32);
                System.Buffer.BlockCopy(tmp, 32, aesKeys[1], 0, 16);

            }


            return aesKeys;
        }

        public static Post EncryptionPost(Post post, string content, Account account)
        {
            try
            {

                //tworzy klucz AES i macierz Inicjującą
                using (Aes myAes = Aes.Create())
                {
                    //tworzy nowa pare kluczy RSA
                    using (RSACryptoServiceProvider myRsa = new RSACryptoServiceProvider())
                    {
                        //Szyfrujemy text i zapisujemy do bazy 
                        byte[] encrypted_data = AesEncrypt(content, myAes.Key, myAes.IV);
                        //Szyfrujemy klucz i zapisujemy do bazy
                        RSAParameters RSAKeyInfo = new RSAParameters();
                        RSAKeyInfo = myRsa.ExportParameters(false);
                        RSAKeyInfo.Modulus = account.PublicKey;
                        RSAKeyInfo.Exponent = account.Exponent;
                        myRsa.ImportParameters(RSAKeyInfo);
                        byte[] encrypted_key = AesKeyEncrypt(myAes.Key, myAes.IV, myRsa.ExportParameters(false));
                        post.Content = encrypted_data;
                        post.Key = encrypted_key;
                        return post;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public static string DecryptionPost(Post post, Account account)
        {
            try
            {

                //byte[] bytePrivateKey = System.Text.Encoding.Default.GetBytes(privateKey);
                //tworzy klucz AES i macierz Inicjującą
                    //tworzy nowa pare kluczy RSA
                    using (RSACryptoServiceProvider myRsa = new RSACryptoServiceProvider())
                    {
                        // Deszyfrowanie
                        RSAParameters RSAKeyInfo = new RSAParameters();
                        RSAKeyInfo = myRsa.ExportParameters(true);
                        RSAKeyInfo.Modulus = account._Modulus;
                        RSAKeyInfo.D = account._D;
                        RSAKeyInfo.DP = account._DP;
                        RSAKeyInfo.DQ = account._DQ;
                        RSAKeyInfo.Exponent = account._Exponent;
                        RSAKeyInfo.InverseQ = account._InvereQ;
                        RSAKeyInfo.P = account._P;
                        RSAKeyInfo.Q = account._Q;
                        myRsa.ImportParameters(RSAKeyInfo);
                        byte[][] AesKeys = DecryptAesKey(post.Key, RSAKeyInfo);
                        string plaintext = AesDecrypt(post.Content, AesKeys[0], AesKeys[1]);

                        return plaintext;
                    }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        static void test()
        {
            try
            {
                string text = "Here is some data to encrypt, lol";

                //tworzy klucz AES i macierz Inicjującą
                using (Aes myAes = Aes.Create())
                {
                    //tworzy nowa pare kluczy RSA
                    using (RSACryptoServiceProvider myRsa = new RSACryptoServiceProvider())
                    {
                        //Szyfrujemy text i zapisujemy do bazy 
                        byte[] encrypted_data = AesEncrypt(text, myAes.Key, myAes.IV);
                        //Szyfrujemy klucz i zapisujemy do bazy
                        byte[] encrypted_key = AesKeyEncrypt(myAes.Key, myAes.IV, myRsa.ExportParameters(false));

                        // Deszyfrowanie

                        byte[][] AesKeys = DecryptAesKey(encrypted_key, myRsa.ExportParameters(true));
                        string plaintext = AesDecrypt(encrypted_data, AesKeys[0], AesKeys[1]);
                        Console.WriteLine(plaintext);
                    }

                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}