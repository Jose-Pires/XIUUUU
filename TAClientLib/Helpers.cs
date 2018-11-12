using System;
using System.Security.Cryptography;
using System.Text;

namespace TAClientLib
{
    public static class Helpers
    {

        /// <summary>
        /// Converts a string with a hex value to a byte array
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="hex">String formatted as hex</param>
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Converts an byte array to an string in hex format
        /// </summary>
        /// <returns>The array to string.</returns>
        /// <param name="ba">Ba.</param>
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Checks if a string is a valid hexadecimal format
        /// </summary>
        /// <returns><c>true</c>, if hex was valid, <c>false</c> otherwise.</returns>
        /// <param name="test">String to test.</param>
        public static bool ValidHex(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// Computes the HMAC of a given byte array
        /// </summary>
        /// <returns>The HMAC in byte array.</returns>
        /// <param name="input">Byte array to encode.</param>
        /// <param name="key">Key.</param>
        public static byte[] Encode(byte[] input, byte[] key)
        {
            using (var hMACSHA256 = new HMACSHA256(key))
                return hMACSHA256.ComputeHash(input);
        }

    }
}
