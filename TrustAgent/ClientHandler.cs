using System;
using System.Collections;
using System.Net.Sockets;
using System.Threading;

namespace TrustAgent
{
    public class ClientHandler
    {
        public event ClientMessage MessageReceived;
        public delegate void ClientMessage(ClientHandler handler, byte[] m);
        public EventArgs e;

        public TcpClient Socket { get; }
        public string Entity { get; }

        readonly Thread thread;

        public ClientHandler(string entity, TcpClient socket)
        {
            Entity = entity;
            Socket = socket;

            thread = new Thread(StartListening)
            {
                IsBackground = true
            };
            thread.Start();
        }

        void StartListening()
        {
            int requestCount = 0;
            requestCount = 0;

            while (true)
            {
                try
                {
                    requestCount = requestCount + 1;

                    byte[] dataLength = new byte[4];
                    NetworkStream stream = Socket.GetStream();
                    stream.Read(dataLength, 0, 4);

                    byte[] packet = new byte[BitConverter.ToInt32(dataLength) + 4];
                    stream.Read(packet, 0, BitConverter.ToInt32(dataLength) + 4);

                    byte[] data = new byte[BitConverter.ToInt32(dataLength)];
                    Array.Copy(packet, 4, data, 0, BitConverter.ToInt32(dataLength));

                    MessageReceived(this, data);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void Disconnect() {
            Socket.Close();
            Socket.Dispose();
            thread.Abort();
        }
    }
}
