using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;


/*
    Testado no Microsoft Windows [Version 10.0.17763.168]
 */

namespace PreSharedKeyLib
{
    public class PreSharedKeys
    {
        public class Keys
        {
            // Membro 1 (entrou primeiro)
            public int Member { get; set; }

            // Membro 1 (entrou depois)
            public int Member2 { get; set; }

            public byte[] Key { get; set; }
            public byte[] Iv { get; set; }
        }

        // Membros Conectados
        public List<string> Members = new List<string>();

        // Lista de Chaves Geradas
        private List<Keys> GenKeys = new List<Keys>();


        /// <summary>
        /// Constroi uma lista de Chaves partilhadas usando uma lista de membros iniciais
        /// </summary>
        /// <param name="Membros"></param>
        public ChavesPrePartilhadas(List<string> Membros)
        {
            foreach (string Membro in Membros)
            {
                Members.Add(Membro);

                if (Members.Count > 1)
                {
                    AddMember(Membro);
                }
            }
        }

        #region Adicionar Membro
        /// <summary>
        /// Adiciona um membro novo e respetivas chaves
        /// </summary>
        /// <param name="Membro"></param>
        public void AddMember(string Membro)
        {
            if (!Members.Contains(Membro))
            {
                Members.Add(Membro);
            }
            if (Members.Count > 1)
            {
                int j = Members.Count - 1;
                for (int i = 0; i < j; i++)
                {
                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256 };
                    aes.GenerateKey();
                    aes.GenerateIV();

                    Keys aux = new Keys
                    {
                        Member = i,
                        Member2 = j,
                        Key = aes.Key,
                        Iv = aes.IV
                    };
                    GenKeys.Add(aux);
                }
            }
        }
        #endregion

        #region Buscar Chave partilhada entre duas entidades
        /// <summary>
        /// Retorna um objeto key que representa a chave especifica entre dois membros
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="Receiver"></param>
        /// <returns Keys></returns>
        public Keys MemberKey(string Sender, string Receiver)
        {
            if (Members.Contains(Sender) && Members.Contains(Receiver))
            {
                int i = Members.FindIndex(val => val.Equals(Sender));
                int j = Members.FindIndex(val => val.Equals(Receiver));

                return (GenKeys.Where(val => i < j ? val.Member == i && val.Member2 == j : val.Member == j && val.Member2 == i).First());
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Encriptação
        /// <summary>
        /// Encripta usando aes256 e a chave 
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Sender"></param>
        /// <param name="Receiver"></param>
        /// <returns byte[]></returns>
        public byte[] Encrypt(byte[] Message, string Sender, string Receiver)
        {
            byte[] encryptedBytes = null;
            if (Members.Contains(Sender) && Members.Contains(Receiver))
            {
                int i = Members.FindIndex(val => val.Equals(Sender));
                int j = Members.FindIndex(val => val.Equals(Receiver));
                using (MemoryStream ms = new MemoryStream())
                {
                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256, BlockSize = 128 };
                    Keys aux = GenKeys.Where(val => i < j ? val.Member == i && val.Member2 == j : val.Member == j && val.Member2 == i).First();
                    aes.Key = aux.Key;
                    aes.IV = aux.Iv;

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Message, 0, Message.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }
            return encryptedBytes;
        }
        #endregion

        #region Decrypt 
        /// <summary>
        /// Decripta usando a chave dos dois membros
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Sender"></param>
        /// <param name="Receiver"></param>
        /// <returns byte[]></returns>
        public byte[] Decrypt(byte[] Message, string Sender, string Receiver)
        {
            byte[] decryptedBytes = null;
            if (Members.Contains(Sender) && Members.Contains(Receiver))
            {
                int i = Members.FindIndex(val => val.Equals(Sender));
                int j = Members.FindIndex(val => val.Equals(Receiver));
                using (MemoryStream ms = new MemoryStream())
                {
                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256, BlockSize = 128 };
                    Keys aux = GenKeys.Where(val => i < j ? val.Member == i && val.Member2 == j : val.Member == j && val.Member2 == i).First();
                    aes.Key = aux.Key;
                    aes.IV = aux.Iv;

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(Message, 0, Message.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();

                }
            }
            return decryptedBytes;
        }
    }
    #endregion
}
