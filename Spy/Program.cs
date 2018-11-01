using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Spy
{
    class Program
    {
        static Server server;
        static void Main(string[] args)
        {
            Console.WriteLine("Use control + C to exit\n");
            server = new Server(IPAddress.Parse(args[0]), int.Parse(args[1]));
            server.ClientConnected += Server_ClientConnected;
        }

        static void Server_ClientConnected(byte[] packet, byte[] m, TcpClient socket, EventArgs e)
        {
            byte[] dataLength = new byte[4];
            Array.Copy(packet, dataLength, 4);
            if (BitConverter.ToInt32(dataLength) < 32) {
                return;
            }

            byte[] hmac = new byte[32];
            Array.Copy(m, hmac, 32);
            byte[] data = new byte[m.Length - 32];
            Array.Copy(m, 32, data, 0, m.Length - 32);
            string str = Encoding.ASCII.GetString(data);
            IPAddress ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;
            PrintBasic("New packet received from " + ip);
            Console.WriteLine(Hex.Dump(packet));
            Console.WriteLine("");

            PrintBasic("The packet has " + data.Length + " bytes.");
            PrintBasic("Decoding packet...");
            PrintBasic("Found HMAC data");
            Console.WriteLine(Hex.Dump(hmac));
            Console.WriteLine("");
            PrintBasic("Trying to convert HMAC");
            Console.WriteLine("HMAC: " + ByteArrayToString(hmac));
            Console.WriteLine("");
            PrintBasic("Found DATA");
            Console.WriteLine(Hex.Dump(data));
            Console.WriteLine("");
            PrintBasic("Trying to convert DATA");
            Console.WriteLine("DATA: " + Encoding.ASCII.GetString(data));
        }
        static void PrintBasic(string msg) {
            var color = Console.ForegroundColor;
            Console.Write("["); Console.ForegroundColor = ConsoleColor.Yellow; Console.Write("SPY"); Console.ForegroundColor = color; Console.Write("] - ");
            Console.WriteLine(msg);
            Console.WriteLine("");
        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
