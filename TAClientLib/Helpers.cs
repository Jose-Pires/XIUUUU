using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TAClientLib
{
    public static class Helpers
    {

        #region "Converters"

        /// <summary>
        /// Converts an object to a byte array
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="obj">The object to be converted.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                bf.Serialize(ms, obj);
                ms.Flush();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts an byte array to a object
        /// </summary>
        /// <returns>The obejct.</returns>
        /// <param name="data">Byte array.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                ms.Position = 0;
                object obj = bf.Deserialize(ms);
                ms.Flush();
                return (T)obj;
            }
        }

        /// <summary>
        /// Converts an byte array to an string in hex format
        /// </summary>
        /// <returns>The array to string.</returns>
        /// <param name="ba">Ba.</param>
        public static string FromByteArrayToHex(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Converts a string with a hex value to a byte array
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="hex">String formatted as hex</param>
        public static byte[] FromHexToByteArray(this String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        #endregion

        /// <summary>
        /// Checks if the array contains an object
        /// This is an extension of the class Array
        /// </summary>
        /// <returns><c>true</c>, if array containsthe object, <c>false</c> otherwise.</returns>
        /// <param name="arr">Array on witch to search.</param>
        /// <param name="obj">Object to search.</param>
        public static bool Contains(this Array arr, object obj)
        {
            foreach (object _obj in arr)
            {
                if (_obj.Equals(obj))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the local IP Addresses.
        /// </summary>
        /// <returns>A list with the local IP Addresses.</returns>
        public static List<IPAddress> GetLocalIPAddress()
        {
            var _host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ips = new List<IPAddress>();
            foreach (IPAddress ip in _host.AddressList)
            {
                if ((ip.AddressFamily == AddressFamily.InterNetwork) && (ip.ToString() != "127.0.0.1"))
                    ips.Add(ip);
            }
            return ips;
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
        /// Replaces the current line with the provided text.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public static void ReplaceWith(string text)
        {
            var c = new string(' ', Console.BufferWidth - text.Length);
            if (Console.BufferWidth < text.Length)
                Console.Write("\r" + text);
            else
                Console.Write("\r" + c);
        }

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <returns>The number.</returns>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static int GenerateSeed(int min = 100000, int max = 999999)
        {
            var rnd = new Random();
            return rnd.Next(min, max);
        }

        /// <summary>
        /// Generates a time stamp with the format HHmmss
        /// </summary>
        /// <returns>The timestamp.</returns>
        /// <param name="value">Value.</param>
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HHmmss");
        }

        /// <summary>
        /// Converts the input of Y or y to boolean this is an extension of string
        /// </summary>
        /// <returns><c>true</c>, if y or Y was entered, <c>false</c> otherwise.</returns>
        /// <param name="input">Input.</param>
        public static bool ToBool(this String input)
        {
            return input == "y" || input == "Y";
        }

    }
}
