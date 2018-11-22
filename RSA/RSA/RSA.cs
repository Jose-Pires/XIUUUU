using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;

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
        private static string path = @"C:\Users\batpe\Desktop\SI\Projeto\XIUUUU\Teste\Teste\RSAEntities.txt";

        public List<string> EntidadesGuardadas { get; set; }
        public RSAParameters PublicKey { get; set; }
        private RSAParameters PrivateKey { get; set; }

        public RSA(string EtName)
        {

            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
            EntidadesGuardadas = new List<string>();

            //TODO Ver se existe maneira mais compacta de fazer isto
            if (File.Exists(path))
            {

                string[] entidades = File.ReadAllLines(path);

                foreach (string entidade in entidades)
                {
                    EntidadesGuardadas.Add(entidade);
                }
            }


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
            if (EntidadesGuardadas.Contains(EtName))
            {
                byte[] messageByte = Encoding.UTF8.GetBytes(message);
                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

                return Rsa.Encrypt(messageByte, true);
            }
            else
            {
                Console.WriteLine("Não tens a chave publica da entidade " + EtName);
                return null;
            }
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
            if (!File.Exists(path))
            {
                File.WriteAllText(path, EtName);
                EntidadesGuardadas.Add(EtName);
            }
            else if (File.Exists(path))
            {
                if (!EntidadesGuardadas.Contains(EtName))
                {
                    File.AppendAllText(path, EtName + "\n");
                    EntidadesGuardadas.Add(EtName);
                }
            }
        }

        /// <summary>
        /// vai buscar a chave publica de uma entidade ao container
        /// </summary>
        /// <param name="etname"></param>
        /// <returns list<byte[]> ></returns>
        public void ShowEntityPublicKey(string EtName)
        {
            if (EntidadesGuardadas.Contains(EtName))
            {
                Cp = new CspParameters { KeyContainerName = EtName };
                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
                RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);
                Console.Write(EtName + " [ ");
                foreach (byte number in RSAKeyInfo.Modulus)
                {
                    Console.Write(number + " ");
                }
                Console.Write(" ]\n");
            }
        }

        /// <summary>
        /// Apaga a key de uma entidade do container
        /// </summary>
        /// <param name="EtName"></param>
        public void DeleteKeyFromContainer(string EtName)
        {
            if (EntidadesGuardadas.Contains(EtName))
            {
                Cp = new CspParameters { KeyContainerName = EtName };

                RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp)
                {
                    PersistKeyInCsp = false
                };
                Rsa.Clear();
                string[] Entidades = File.ReadAllLines(path);
                File.Delete(path);
                Entidades = Entidades.Where(val => val != EtName).ToArray();
                File.WriteAllLines(path, Entidades);
                EntidadesGuardadas = EntidadesGuardadas.Where(val => val != EtName).ToList();
            }
        }

    }
}
