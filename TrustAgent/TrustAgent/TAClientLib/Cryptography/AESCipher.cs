/*
 * TAClientLib.AESCipher.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Contains multiple operations using AES256
 * 
 * Requires initialization: NO
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace TAClientLib
{
    public static class AESCipher
    {

        /// <summary>
        /// Encrypts data using AES256
        /// </summary>
        /// <returns>Encrypted data.</returns>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <param name="iv">IV bytes.</param>
        public static byte[] EncryptData(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts data using AES256.
        /// </summary>
        /// <returns>Decrypted data.</returns>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <param name="iv">IV bytes.</param>
        public static byte[] DecryptData(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    using (var decryptor = aes.CreateDecryptor(key, iv))
                    {
                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.Close();
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Generates an Key based on the first seed (seed1) and the selector
        /// </summary>
        /// <returns>Returns the propper Key.</returns>
        public static byte[] GenKey(int seed, int selector)
        {
            List<byte[]> keys = new List<byte[]>();
            Random rnd = new Random(seed);
            for (int i = 0; i < 50; i++)
            {
                byte[] key = new byte[32]; //KEY 256bits
                rnd.NextBytes(key);
                keys.Add(key);
            }
            return keys[selector];
        }

        /// <summary>
        /// Generates an IV based on the second seed (seed2) and the selector
        /// </summary>
        /// <returns>Returns the propper IV.</returns>
        public static byte[] GenIV(int seed, int selector)
        {
            List<byte[]> ivs = new List<byte[]>();
            Random rnd = new Random(seed);
            for (int i = 0; i < 50; i++)
            {
                byte[] iv = new byte[16]; //IV 128bits
                rnd.NextBytes(iv);
                ivs.Add(iv);
            }
            return ivs[selector];
        }
    }
}
