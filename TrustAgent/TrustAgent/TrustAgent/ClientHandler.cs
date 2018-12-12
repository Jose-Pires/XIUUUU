/*
 * TrustAgent.ClientHandler.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Handles the messages received from a single client already connected to the
 * server
 * 
 * Requires initialization: YES
 * 
 */

using System;
using System.Net.Sockets;
using System.Threading;

namespace TrustAgent
{
    public class ClientHandler
    {
        bool stop;
        public event ClientMessage MessageReceived;
        public event ClientEvent ConnectionLost;
        public delegate void ClientMessage(ClientHandler handler, byte[] m);
        public delegate void ClientEvent(ClientHandler handler);
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

        /// <summary>
        /// Starts listening for messages from the user
        /// </summary>
        void StartListening()
        {
            int requestCount = 0;
            requestCount = 0;

            while (!stop)
            {
                try
                {
                    requestCount = requestCount + 1;

                    byte[] dataLength = new byte[4];
                    NetworkStream stream = Socket.GetStream();
                    stream.Read(dataLength, 0, 4);

                    byte[] packet = new byte[BitConverter.ToInt32(dataLength)];
                    stream.Read(packet, 0, BitConverter.ToInt32(dataLength));

                    //byte[] data = new byte[ BitConverter.ToInt32(dataLength)];
                    //Array.Copy(packet, 4, data, 0, BitConverter.ToInt32(dataLength));

                    MessageReceived(this, packet);

                }
                catch (Exception)
                {
                    if (!stop) {
                        ConnectionLost(this);
                        stop = true;
                    }
                }
            }
        }

        /// <summary>
        /// Closes the socket 
        /// </summary>
        public void Disconnected() {
            stop = true;
            Socket.Close();
            Socket.Dispose();
        }

        /// <summary>
        /// Disconnects the user
        /// </summary>
        public void Disconnect() {
            stop = true;

            PacketType packetType = PacketType.ServerCommand;
            ServerCommand command = new ServerCommand
            {
                Command = ServerOperations.KickEntity.Value
            };

            NetworkStream stream = Socket.GetStream();
            byte[] packet = Server.BuildPacket(command, Program.database.RetreiveEntityKey(Entity), packetType);
            stream.Write(packet, 0, packet.Length);
            stream.Flush();
            if (Program.enableSpy)
                Program.spy.SendPacket(packet);

            Socket.Close();
            Socket.Dispose();

        }
    }
}
