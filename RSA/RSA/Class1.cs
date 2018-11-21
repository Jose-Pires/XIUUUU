using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RSA
{
    public class RSA
    {
        private CspParameters Cp;

        //! Caso não esteja a funcionar, mudar para public
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

        // Encripta uma mensagem 
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

        // Encripta uma mensagem usando a nossa chave publica
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

        // Decifra uma mensagem
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

        // Adiciona uma entidade ao container
        public void AddEntintyPublicKey(string EtName, List<byte[]> PublicKey)
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            RSAParameters RSAKeyInfo = Rsa.ExportParameters(true);
        }

        //Vai buscar a chave publica de uma entidade ao container
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

        public void DeleteKeyFromContainer(string EtName)
        {
            Cp = new CspParameters { KeyContainerName = EtName };

            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            Rsa.PersistKeyInCsp = false;
            Rsa.Clear();
        }

    }
}
