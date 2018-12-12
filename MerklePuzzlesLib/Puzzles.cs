using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MerklePuzzlesLib
{
    public class Puzzles
    {
        #region "aes"

        public byte[] Encript(byte[] mensagem, byte[] key)
        {
            byte[] encryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256 };
                aes.Key = key;
                aes.Mode = CipherMode.ECB;

                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(mensagem, 0, mensagem.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }

            return encryptedBytes;
        }



        public byte[] Decript(byte[] mensagem, byte[] key)
        {
            byte[] encryptedBytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256 };
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                try
                {
                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(mensagem, 0, mensagem.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
                catch (Exception ex)
                {
                    return Encoding.UTF8.GetBytes("fodas");
                }
            }

            return encryptedBytes;
        }

        #endregion

        #region "decimal to binary"

        public string DecimalParaBinario(string numero)
        {

            string valor = "";

            int dividendo = Convert.ToInt32(numero);

            if (dividendo == 0 || dividendo == 1)
            {

                return Convert.ToString(dividendo);

            }

            else
            {

                while (dividendo > 0)
                {

                    valor += Convert.ToString(dividendo % 2);

                    dividendo = dividendo / 2;

                }
                string inverted = InverterString(valor);

                return InverterString(valor);

            }

        }



        public string InverterString(string str)
        {
            int tamanho = str.Length;

            char[] caracteres = new char[tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                caracteres[i] = str[tamanho - 1 - i];
            }

            return new string(caracteres);
        }

        #endregion

        #region "Gerar lista de Chaves"
        public List<byte[]> GerarListaDeChaves()
        {

            List<byte[]> listaPi = new List<byte[]>();

            Random randNum = new Random();
            for (int i = 0; i < 256; i++)
            {
                int aleatorio = randNum.Next(0, 255);

                String str = aleatorio.ToString();

                var x = DecimalParaBinario(str);

                x = x.PadLeft(8, '0');

                var y = Encoding.UTF8.GetBytes(x);
                listaPi.Add(Encoding.UTF8.GetBytes("000000000000000000000000" + x));

                Console.WriteLine("chave " + i + " : " + listaPi[i]);
            }

            return listaPi;
        }
        #endregion


        #region "Gerar lista de Mensagens"

        public List<byte[]> GerarListaDeMensagens()
        {
            List<byte[]> listaMes = new List<byte[]>();

            Random randNum = new Random();



            for (int i = 0; i < 256; i++)
            {

                Random r = new Random();
                int xi = r.Next(45, 300);

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider { KeySize = 256 };
                aes.GenerateKey();
                string puzzle = "puzzle" + xi;
                string ki = Convert.ToBase64String(aes.Key);
                string mensagem = (puzzle + "|" + ki);

                listaMes.Add(Encoding.UTF8.GetBytes(mensagem));

                Console.WriteLine("mensagem " + i + ": " + listaMes[i]);
            }
            return listaMes;
        }


        #endregion


        #region "cifrar mensagens"
        public List<byte[]> CifrarListaMensagens(List<byte[]> listaMensagens, List<byte[]> listaChaves)
        {
            List<byte[]> listaMensagensCifradas = new List<byte[]>();

            for (int i = 0; i < 256; i++)
            {
                listaMensagensCifradas.Add(Encript(listaMensagens[i], listaChaves[i]));

                Console.WriteLine("cifrada " + i + ": " + listaMensagensCifradas[i]);
            }
            return listaMensagensCifradas;
        }

        #endregion


        #region "decifrar mensagens"
        public List<byte[]> DecifrarListaMensagens(List<byte[]> listaMensagensCifradas, List<byte[]> listaChaves)
        {
            List<byte[]> listaMensagensDecifradas = new List<byte[]>();

            for (int i = 0; i < 256; i++)
            {
                listaMensagensDecifradas.Add(Decript(listaMensagensCifradas[i], listaChaves[i]));

                Console.WriteLine("decifrada " + i + ": " + listaMensagensDecifradas[i]);
            }

            return listaMensagensDecifradas;
        }

        #endregion

        #region "escolher mensagem cifrada aleatoria"

        public byte[] EscolherMensagemCifradaAleatoria(List<byte[]> listaMensagensCifradas)
        {
            Random randNum = new Random();
            int aleatorio = randNum.Next(0, 255);
            Console.WriteLine(aleatorio);
            byte[] mensagemEscolhida = listaMensagensCifradas[aleatorio];
            return mensagemEscolhida;
        }

        #endregion

        #region "decifrar mensagem (força bruta)"

        public string DecifrarMensagemEscolhida(byte[] cifrada)
        {
            string dec = string.Empty;
            byte[] aux = null;
            string chave;


            for (int i = 0; i < 256; i++)
            {

                string y = DecimalParaBinario(i.ToString());
                y = y.PadLeft(8, '0');

                chave = ("000000000000000000000000" + y);

                var x = Encoding.UTF8.GetBytes(chave);
                aux = Decript(cifrada, Encoding.UTF8.GetBytes(chave));
                dec = Encoding.UTF8.GetString(aux);
                if (dec.Contains("puzzle"))
                {
                    break;
                }
            }
            Console.WriteLine("CONSEGUI");
            return dec;
        }
        #endregion
    }
}
