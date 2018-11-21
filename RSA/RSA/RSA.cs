using System;
using System.Collections.Generic;
using System.Security.Cryptography;

/*
 * RSA.cs
 * Developer: Pedro Batista
 * Developement stage: Completed (awaiting testing)
 * Tested on: pending
 * 
 */

namespace RSA
{
    public class RSA
    {
        private CspParameters Cp;


        public List<byte[]> PublicKey { get; set; }
        public List<byte[]> PrivateKey { get; set; }

        public RSA(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(true);

            PublicKey.Add(RSAKeyInfo.Exponent);
            PublicKey.Add(RSAKeyInfo.Modulus);

            PrivateKey.Add(RSAKeyInfo.D);
            PrivateKey.Add(RSAKeyInfo.Modulus);
        }

        /// <summary>
        /// Encripta uma mensagem 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="EntityPublicKey"></param>
        /// <returns byte[]></returns>
        public byte[] Encrypt(byte[] message, List<byte[]> EntityPublicKey)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = new RSAParameters
            {
                Exponent = EntityPublicKey[0],
                Modulus = EntityPublicKey[1]
            };

            Rsa.ImportParameters(RSAKeyInfo);
            return Rsa.Encrypt(message, true);
        }


        /// <summary>
        /// Encripta uma mensagem usando a nossa chave publica
        /// </summary>
        /// <param name="message"></param>
        /// <returns byte[]></returns>
        public byte[] Encrypt(byte[] message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = new RSAParameters
            {
                Exponent = PublicKey[0],
                Modulus = PublicKey[1]
            };

            Rsa.ImportParameters(RSAKeyInfo);
            return Rsa.Encrypt(message, true);
        }

        /// <summary>
        /// Decifra uma mensagem
        /// </summary>
        /// <param name="message"></param>
        /// <returns byte[]></returns>
        public byte[] Decrypt(byte[] message)
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = new RSAParameters
            {
                D = PrivateKey[0],
                Modulus = PrivateKey[1]
            };

            Rsa.ImportParameters(RSAKeyInfo);
            return Rsa.Decrypt(message, true);
        }

        /// <summary>
        /// Adiciona uma entidade ao container
        /// </summary>
        /// <param name="EtName"></param>
        /// <param name="PublicKey"></param>
        public void AddEntintyPublicKey(string EtName, List<byte[]> PublicKey)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(true);
        }

        /// <summary>
        /// Vai buscar a chave publica de uma entidade ao container
        /// </summary>
        /// <param name="EtName"></param>
        /// <returns List<byte[]> ></returns>
        public List<byte[]> GetEntityPublicKey(string EtName)
        {
            List<byte[]> EnPublicKey = new List<byte[]>();
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);

            EnPublicKey.Add(RSAKeyInfo.Exponent);
            EnPublicKey.Add(RSAKeyInfo.Modulus);

            return EnPublicKey;
        }

        /// <summary>
        /// Apaga a key de uma entidade do container
        /// </summary>
        /// <param name="EtName"></param>
        public void DeleteKeyFromContainer(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };

            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            Rsa.PersistKeyInCsp = false;
            Rsa.Clear();
        }

    }
}
