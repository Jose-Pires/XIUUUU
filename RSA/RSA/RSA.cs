using System;
using System.Collections.Generic;
using System.Security.Cryptography;

/*
 * RSA.cs
 * Developer: Pedro Batista
 * Developement stage: Encryption and Decryption a funcionar, falta fazer para mostrar chaves publicas e privadas
 * Tested on: Windows 10 
 * 
 */

namespace RSA
{
    public class RSA
    {
        private CspParameters Cp;

        public RSAParameters PublicKey { get; set; }
        private RSAParameters PrivateKey { get; set; }

        public RSA(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

            PublicKey = Rsa.ExportParameters(false);
            PrivateKey = Rsa.ExportParameters(true);
        }

        /// <summary>
        /// Encripta uma mensagem 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="EntityPublicKey"></param>
        /// <returns byte[]></returns>
        public byte[] Encrypt(string message, string EtName)
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

            return Rsa.Encrypt(messageByte, true);
        }


        /// <summary>
        /// Encripta uma mensagem usando a nossa chave publica
        /// </summary>
        /// <param name="message"></param>
        /// <returns byte[]></returns>
        public byte[] Encrypt(string message)
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PublicKey);
            return Rsa.Encrypt(messageByte, true);
        }

        /// <summary>
        /// Decifra uma mensagem
        /// </summary>
        /// <param name="message"></param>
        /// <returns byte[]></returns>
        public byte[] Decrypt(byte[] message, string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(true);

            Rsa.ImportParameters(PrivateKey);
            return Rsa.Decrypt(message, true);
        }

        /// <summary>
        /// Adiciona uma entidade ao container
        /// </summary>
        /// <param name="EtName"></param>
        /// <param name="PublicKey"></param>
        public void AddEntintyPublicKey(string EtName, RSAParameters EtParameters)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            Rsa.ImportParameters(EtParameters);
        }

        ///// <summary>
        ///// Vai buscar a chave publica de uma entidade ao container
        ///// </summary>
        ///// <param name="EtName"></param>
        ///// <returns List<byte[]> ></returns>
        //public List<byte[]> GetEntityPublicKey(string EtName)
        //{
        //    List<byte[]> EnPublicKey = new List<byte[]>();
        //    Cp = new CspParameters { KeyContainerName = EtName };
        //    RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
        //    RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);

        //    EnPublicKey.Add(RSAKeyInfo.Exponent);
        //    EnPublicKey.Add(RSAKeyInfo.Modulus);

        //    return EnPublicKey;
        //}

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
