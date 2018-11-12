using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TrustAgent
{

    public class Client
    {
        public bool IsListening { get; set; }
        TcpClient clientSocket;
        NetworkStream serverStream;

        public void Connect(string ip, int port, byte[] data)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(ip, port);
            serverStream = clientSocket.GetStream();
            serverStream.Write(data, 0, data.Length);
            serverStream.Flush();
        }

        void SendDisconnectMessage()
        {
            byte[] outStream = Encoding.ASCII.GetBytes("client_disconnect");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        public void SendPacket(byte[] b) {
            serverStream = clientSocket.GetStream();
            serverStream.Write(b, 0, b.Length);
            serverStream.Flush();
        }

    }
}