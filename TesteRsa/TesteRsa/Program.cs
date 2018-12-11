using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TesteRsa
{
    class Program
    {
        static void Main(string[] args)
        {
            string hello = "hello world";

            RSA Pedro = new RSA("Pedro");
            RSA Sabenca = new RSA("Sabenca");

            Pedro.AddEntintyPublicKey("Sabenca", Sabenca.PublicKey);

            byte[] helloe = Pedro.EncryptToByte(hello, "Sabenca");
            Console.Write(Sabenca.DecryptToString(helloe));
            Console.Read();
        }
    }
}

public class RSA
{

    // Usadas para cifrar os nomes guardados no ficheiro
    static readonly string PasswordHash = "P@@Sw0rd";
    static readonly string SaltKey = "S@LT&KEY";
    static readonly string VIKey = "@1B2c3D4e5F6g7H8";

    // Container onde estão guardadas as keys
    private CspParameters Cp;

    private string Path { get; set; }

    //Entidades ás quais a entidade tem acesso (chave publica)
    public List<string> EntidadesGuardadas { get; set; }


    public RSAParameters PublicKey { get; set; }
    private RSAParameters PrivateKey { get; set; }

    #region Construtor
    public RSA(string EtName)
    {

        //Path = EtName + "RSAEntities.txt";

        Cp = new CspParameters { KeyContainerName = EtName };
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
        EntidadesGuardadas = new List<string>();
        EntidadesGuardadas.Add(EtName);

        //TODO Caso seja necessário files
        //if (File.Exists(Path))
        //{
        //    string[] entidades = File.ReadAllLines(Path);

        //    //EntidadesGuardadas.AddRange(entidades.ToList());
        //    foreach (string entidade in entidades)
        //    {
        //        EntidadesGuardadas.Add(DecryptName(entidade));
        //    }
        //}

        PublicKey = Rsa.ExportParameters(false);
        PrivateKey = Rsa.ExportParameters(true);

    }
    #endregion

    #region encrypt
    /// <summary>
    /// Encripta uma mensagem usando uma chave publica
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EtName"></param>
    /// <returns byte[]></returns>
    public byte[] EncryptToByte(string message, string EtName = null)
    {
        if (String.IsNullOrEmpty(EtName))
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PublicKey);
            return Rsa.Encrypt(messageByte, true);
        }
        else if (EntidadesGuardadas.Contains(EtName))
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

    public byte[] EncryptToByte(string message, byte[] Exponent, byte[] Modulus)
    {

        byte[] messageByte = Encoding.UTF8.GetBytes(message);
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        RSAParameters parameters = new RSAParameters
        {
            Exponent = Exponent,
            Modulus = Modulus
        };
        Rsa.ImportParameters(parameters);
        return Rsa.Encrypt(messageByte, true);
    }

    public string EncryptToString(string message, byte[] Exponent, byte[] Modulus)
    {

        byte[] messageByte = Encoding.UTF8.GetBytes(message);
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        RSAParameters parameters = new RSAParameters
        {
            Exponent = Exponent,
            Modulus = Modulus
        };
        Rsa.ImportParameters(parameters);
        return Encoding.UTF8.GetString(Rsa.Encrypt(messageByte, true));
    }

    /// <summary>
    ///  Encripta uma mensagem usando uma chave publica
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EtName"></param>
    /// <returns byte[]></returns>
    public byte[] EncryptToByte(byte[] message, string EtName = null)
    {
        if (String.IsNullOrEmpty(EtName))
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PublicKey);
            return Rsa.Encrypt(message, true);
        }
        else if (EntidadesGuardadas.Contains(EtName))
        {
            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

            return Rsa.Encrypt(message, true);
        }
        else
        {
            Console.WriteLine("Não tens a chave publica da entidade " + EtName);
            return null;
        }
    }

    /// <summary>
    ///  Encripta uma mensagem usando uma chave publica
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EtName"></param>
    /// <returns string></returns>
    public string EncryptToString(string message, string EtName = null)
    {
        if (String.IsNullOrEmpty(EtName))
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(message);

            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PublicKey);
            return Convert.ToBase64String(Rsa.Encrypt(messageByte, true));
        }
        else if (EntidadesGuardadas.Contains(EtName))
        {
            byte[] messageByte = Encoding.UTF8.GetBytes(message);

            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

            return Encoding.UTF8.GetString(Rsa.Encrypt(messageByte, true));
        }
        else
        {
            Console.WriteLine("Não tens a chave publica da entidade " + EtName);
            return null;
        }
    }

    /// <summary>
    ///  Encripta uma mensagem usando uma chave publica
    /// </summary>
    /// <param name="message"></param>
    /// <param name="EtName"></param>
    /// <returns string></returns>
    public string EncryptToString(byte[] message, string EtName = null)
    {
        if (String.IsNullOrEmpty(EtName))
        {
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
            Rsa.ImportParameters(PublicKey);
            return Convert.ToBase64String(Rsa.Encrypt(message, true));
        }
        else if (EntidadesGuardadas.Contains(EtName))
        {

            Cp = new CspParameters { KeyContainerName = EtName };
            RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);

            return Encoding.UTF8.GetString(Rsa.Encrypt(message, true));
        }
        else
        {
            Console.WriteLine("Não tens a chave publica da entidade " + EtName);
            return null;
        }
    }
    #endregion

    #region decrypt
    /// <summary>
    /// Decifra uma mensagem usando a chave privada
    /// </summary>
    /// <param name="message"></param>
    /// <returns byte[]></returns>
    public byte[] DecryptToByte(byte[] message)
    {
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        Rsa.ImportParameters(PrivateKey);
        return Rsa.Decrypt(message, true);
    }

    /// <summary>
    /// Decifra uma mensagem usando a chave privada
    /// </summary>
    /// <param name="message"></param>
    /// <returns string></returns>
    public string DecryptToString(byte[] message)
    {
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        Rsa.ImportParameters(PrivateKey);
        return Encoding.UTF8.GetString(Rsa.Decrypt(message, true));
    }

    /// <summary>
    /// Decifra uma mensagem usando a chave privada
    /// </summary>
    /// <param name="message"></param>
    /// <returns byte[]></returns>
    public byte[] DecryptToByte(string message)
    {
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        Rsa.ImportParameters(PrivateKey);
        return Rsa.Decrypt(Encoding.UTF8.GetBytes(message), true);
    }

    /// <summary>
    /// Decifra uma mensagem usando a chave privada
    /// </summary>
    /// <param name="message"></param>
    /// <returns string></returns>
    public string DecryptToString(string message)
    {
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider();
        Rsa.ImportParameters(PrivateKey);
        return Encoding.UTF8.GetString(Rsa.Decrypt(Encoding.UTF8.GetBytes(message), true));
    }

    #endregion

    #region Adicionar Chave Publica
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

        if (!EntidadesGuardadas.Contains(EtName))
        {
            EntidadesGuardadas.Add(EtName);
        }

        //if (!File.Exists(Path))
        //{
        //    File.WriteAllText(Path, EncryptName(EtName) + "\n");
        //    EntidadesGuardadas.Add(EtName);
        //}
        //else if (File.Exists(Path))
        //{
        //    if (!EntidadesGuardadas.Contains(EtName))
        //    {
        //        File.AppendAllText(Path, EncryptName(EtName) + "\n");
        //        EntidadesGuardadas.Add(EtName);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Entidade já se encontra guardada");
        //    }
        //}
    }

    public void AddEntintyPublicKey(string EtName, byte[] Exponent, byte[] Modulus)
    {
        Cp = new CspParameters { KeyContainerName = EtName };
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
        RSAParameters aux = new RSAParameters
        {
            Exponent = Exponent,
            Modulus = Modulus
        };

        Rsa.ImportParameters(aux);

        if (!EntidadesGuardadas.Contains(EtName))
        {
            EntidadesGuardadas.Add(EtName);
        }

        //if (!File.Exists(Path))
        //{
        //    File.WriteAllText(Path, EncryptName(EtName) + "\n");
        //    EntidadesGuardadas.Add(EtName);
        //}
        //else if (File.Exists(Path))
        //{
        //    if (!EntidadesGuardadas.Contains(EtName))
        //    {
        //        File.AppendAllText(Path, EncryptName(EtName) + "\n");
        //        EntidadesGuardadas.Add(EtName);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Entidade já se encontra guardada");
        //    }
        //}
    }
    #endregion

    #region Mostrar Chave Publica de uma Entidade
    /// <summary>
    /// vai buscar a chave publica de uma entidade ao container
    /// </summary>
    /// <param name="etname"></param>
    public void ShowEntityPublicKey(string EtName)
    {
        if (EntidadesGuardadas.Contains(EtName))
        {
            while (true)
            {
                Console.Write("\n1 - Numero\n2 - Hex\n");
                string escolha = Console.ReadLine();

                if (escolha == "1")
                {
                    ShowPublicNumber(EtName);
                    break;
                }
                else if (escolha == "2")
                {
                    ShowPublicStr(EtName);
                    break;
                }
                else
                {
                    Console.WriteLine("Comando errado");
                }
            }
        }
        else
        {
            Console.WriteLine("Não tem a chave publica desta entidade");
        }
    }

    private void ShowPublicNumber(string EtName)
    {
        Cp = new CspParameters { KeyContainerName = EtName };
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
        RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);
        Console.WriteLine("\n" + EtName + "\n");
        Console.WriteLine("Expoente :");

        Console.Write(" [ ");
        foreach (byte number in RSAKeyInfo.Exponent)
        {
            Console.Write(number + " ");
        }
        Console.Write(" ]\n");

        Console.WriteLine("Modulo :");
        Console.Write(" [ ");
        foreach (byte number in RSAKeyInfo.Modulus)
        {
            Console.Write(number + " ");
        }

        Console.Write(" ]\n");
    }

    private void ShowPublicStr(string EtName)
    {
        Cp = new CspParameters { KeyContainerName = EtName };
        RSACryptoServiceProvider Rsa = new RSACryptoServiceProvider(Cp);
        RSAParameters RSAKeyInfo = Rsa.ExportParameters(false);
        Console.WriteLine("\n" + EtName + "\n");
        Console.WriteLine("Expoente :");
        Console.Write(EtName + " [ " + BitConverter.ToString(RSAKeyInfo.Exponent) + " ]\n");
        Console.WriteLine("Modulo :");
        Console.Write(EtName + " [ " + BitConverter.ToString(RSAKeyInfo.Modulus) + " ]\n");
    }

    #endregion

    #region Mostrar Chave Privada
    /// <summary>
    /// vai buscar a chave publica de uma entidade ao container
    /// </summary>
    /// <param name="etname"></param>
    public void ShowPrivateKey()
    {
        while (true)
        {
            Console.Write("\n1 - Numero\n2 - Hex\n");
            string escolha = Console.ReadLine();

            if (escolha == "1")
            {
                Console.WriteLine("D :");

                Console.Write(" [ ");
                foreach (byte number in PrivateKey.D)
                {
                    Console.Write(number + " ");
                }
                Console.Write(" ]\n");
                break;
            }
            else if (escolha == "2")
            {
                Console.WriteLine("D :");
                Console.Write(" [ " + BitConverter.ToString(PrivateKey.D) + " ]\n");
                break;
            }
            else
            {
                Console.WriteLine("Comando errado");
            }
        }
    }
    #endregion
    #region Apagar Do Container
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

            //Caso seja necessário usar ficheiros
            //string[] Entidades = File.ReadAllLines(Path);
            //File.Delete(Path);
            //Entidades = Entidades.Where(val => DecryptName(val) != EtName).ToArray();
            //File.WriteAllLines(Path, Entidades);
            EntidadesGuardadas = EntidadesGuardadas.Where(val => val != EtName).ToList();
        }
    }

    public void DeleteAllFromContainer()
    {
        foreach (string Entidade in EntidadesGuardadas)
        {
            DeleteKeyFromContainer(Entidade);
        }
    }
    #endregion

    #region EncriptarNomes
    public static string EncryptName(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
        var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

        byte[] cipherTextBytes;

        using (var memoryStream = new MemoryStream())
        {
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                cipherTextBytes = memoryStream.ToArray();
                cryptoStream.Close();
            }
            memoryStream.Close();
        }
        return Convert.ToBase64String(cipherTextBytes);
    }

    public static string DecryptName(string encryptedText)
    {
        byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
        byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
        var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

        var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];

        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
    }
    #endregion
}
