/*
 * PBKDF2.**.cs 
 * Developer: José Pires
 * Developement stage: Development
 * Tested on: Windows 10 (1803) -> Current Commit Fully Tested (Working)
 */
using System;
using System.Text;
using System.Security.Cryptography;

/*
 * Funcionalidades base do PBKDF2 feitas, falta apenas implementação do seguinte ponto: "Possibilitar a escolha de diferentes funções de hash para o PBKDF2".
 * Quantas mais iterações maior a entropia, daí o min de 250000 iterações.
 * (1*) - > https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rngcryptoserviceprovider?view=netframework-4.7.2 
 * (2*) - > https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netframework-4.7.2
*/

namespace PBKDF2
{
    class PBKDF2
    {
        /* Métodos Principais */
        /* Gera sal criptográfico. */
        public static byte[] GerarSal(int size)
        {
            if (size < 64)
                size = 64;
            byte[] sal = new byte[size];
            var rng = new RNGCryptoServiceProvider(); //Função de RNG (1*). 
            rng.GetNonZeroBytes(sal);
            return (sal);
        }
        /* Encriptação usando o Password Based Key Derivation Function 2. Recebe Password, sal e iterações.
        Para aumentar entropia basta aumentar a quantidade de iterações, tamanho do segredo 
        (O tamanho do segredo pode não ser igual ao size, depende das iterações e do tamanho do sal)*/
        public static byte[] Derivar(byte[] password, byte[] salt, int it, int size)
        {
            if (size < 64)
                size = 64;
            if (size > 160)
                size = 160;
            if (it < 250000)
                it = 250000;
            var derivedKey = new Rfc2898DeriveBytes(password, salt, it); //Função de geração pseudo-randomizada de números baseada no HMACSHA1 (2*).
            return derivedKey.GetBytes(size);
        }

        /* Métodos auxiliares */
        public static byte[] StringToByte(String input)
        {
            return Encoding.ASCII.GetBytes(input);
        }
        public static string ByteToString(byte[] input)
        {
            return System.Text.Encoding.Default.GetString(input);
        }
    }
}