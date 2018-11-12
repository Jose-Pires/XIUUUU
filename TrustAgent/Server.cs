using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

using static TrustAgent.SHA256hmac;

namespace TrustAgent
{
    public class Server
    {

        #region "Variables"

        Thread thread;
        bool stop;

        public EventArgs e;

        public Hashtable clientHandlers = new Hashtable();

        #endregion

        #region "Events"

        public event ClientCommunicationHandler MessageReceived;
        public event ClientConnectionHandler ClientConnected;

        #endregion

        #region "Delegates"

        public delegate void ClientCommunicationHandler(byte[] m, ClientHandler clientHandler);
        public delegate void ClientConnectionHandler(byte[] m, TcpClient socket, EventArgs e);

        #endregion

        public Server(IPAddress ip, int port)
        {
            thread = new Thread(() => StartListening(ip, port));
            thread.Start();
        }

        void StartListening(IPAddress ip, int port)
        {
            TcpListener serverSocket = new TcpListener(ip, port);
            TcpClient clientSocket = default(TcpClient);

            int counter = 0;
            serverSocket.Start();

            while (!stop)
            {
                if (serverSocket.Pending())
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();


                    byte[] dataLength = new byte[4];
                    NetworkStream stream = clientSocket.GetStream();
                    stream.Read(dataLength, 0, 4);

                    byte[] packet = new byte[BitConverter.ToInt32(dataLength) + 4];
                    stream.Read(packet, 0, BitConverter.ToInt32(dataLength) + 4);

                    byte[] data = new byte[BitConverter.ToInt32(dataLength)];
                    Array.Copy(packet, 4, data, 0, BitConverter.ToInt32(dataLength));

                    ClientConnected(data, clientSocket, e);

                }
            }

            BroadcastShutdownToClients();
            if (clientSocket != null)
                clientSocket.Close();
            serverSocket.Stop();
            foreach (DictionaryEntry item in clientHandlers)
                ((ClientHandler)item.Value).Disconnect();
        }

        /// <summary>
        /// Accepts a client a saves all the data in a Hashtable
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="socket">Socket.</param>
        /// <param name="key">Key.</param>
        /// <param name="enableSpy">If set to <c>true</c> enable spy.</param>
        /// <param name="spyIP">Spy ip.</param>
        /// <param name="spyPort">Spy port.</param>
        public void AcceptClient(string entity, TcpClient socket, byte[] key, bool enableSpy = false, string spyIP = "", int spyPort = 11223)
        {

            SendMessage(new ServerCommand { Command = ServerOperations.Connected.Value, EnableSpy = enableSpy, SpyIP = spyIP, SpyPort = spyPort },
                        PacketType.ServerCommand, key, socket);

            ClientHandler client = new ClientHandler(entity, socket);
            client.MessageReceived += Client_MessageReceived;

            clientHandlers.Add(entity, client);
        }

        public void DennyClient(string entity, ServerMessages message, TcpClient socket, byte[] key) {
            SendMessage(new ServerCommand { Command = ServerOperations.ConnectionRefused.Value, Message = message.Value },
                        PacketType.ServerCommand, key, socket);
        }

        /// <summary>
        /// Handles a message received from a client
        /// </summary>
        /// <param name="handler">Handler.</param>
        /// <param name="m">M.</param>
        void Client_MessageReceived(ClientHandler handler, byte[] m)
        {
            MessageReceived(m, handler);
        }

        /// <summary>
        /// Sends a message to a specific client (socket)
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="type">Type.</param>
        /// <param name="key">Key.</param>
        /// <param name="socket">Socket.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void SendMessage<T>(T message, PacketType type, byte[] key, TcpClient socket)
        {
            new Thread(delegate ()
            {
                NetworkStream stream = socket.GetStream();
                byte[] packet = BuildPacket(message, key, type);
                stream.Write(packet, 0, packet.Length);
                stream.Flush();
                if (Program.enableSpy)
                    Program.spy.SendPacket(packet);
            }).Start();
        }

        /// <summary>
        /// Sends a signal to all connected clients that the server is shutting down
        /// </summary>
        public void BroadcastShutdownToClients() {
            int threads = clientHandlers.Count;
            CountdownEvent countdown = new CountdownEvent(threads);

            foreach (DictionaryEntry item in clientHandlers) {
                ClientHandler handler = (ClientHandler)item.Value; 
                string entity = handler.Entity;
                byte[] packet = BuildPacket(new ServerCommand { Command = ServerOperations.ServerShutdown.Value },
                                            Program.database.RetreiveEntityKey(entity), PacketType.ServerCommand);
                new Thread(delegate ()
                {
                    NetworkStream stream = handler.Socket.GetStream();
                    stream.Write(packet, 0, packet.Length);
                    stream.Flush();
                    if (Program.enableSpy)
                        Program.spy.SendPacket(packet);
                    countdown.Signal();
                }).Start();
            }
            countdown.Wait();
        }

        /// <summary>
        /// Initializes the shutdown process
        /// </summary>
        public void Shutdown() { stop = true; }

        #region "Network Helpers"

        /// <summary>
        /// Builds a packet to be sent to the client
        /// A packet follows the following diagram
        /// +-----------------------------------------------------------+
        /// | PACKET SIZE | PACKET TYPE | HMAC (SHA256) |    MESSAGE    |
        /// |    INT 32   |    INT 32   |  BYTE ARRAY   |    STRING     |
        /// |   4 BYTES   |   4 BYTES   |   32 BYTES    | VARIABLE SIZE |
        /// +-----------------------------------------------------------+
        /// </summary>
        /// <returns>The packet.</returns>
        /// <param name="message">Message.</param>
        /// <param name="key">Key.</param>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static byte[] BuildPacket<T>(T message, byte[] key, PacketType type)
        {
            string json = JsonConvert.SerializeObject(message);
            byte[] packetType = BitConverter.GetBytes(type.Value);
            byte[] data = Encoding.ASCII.GetBytes(json);
            byte[] hmac = ComputeHMAC(data, key);
            byte[] size = BitConverter.GetBytes(hmac.Length + data.Length);
            byte[] packet = new byte[data.Length + hmac.Length + 4];
            Array.Copy(size, packet, 4);
            Array.Copy(packetType, 0, packet, 4, 4);
            Array.Copy(hmac, 0, packet, 8, hmac.Length);
            Array.Copy(data, 0, packet, 8 + hmac.Length, data.Length);
            return packet;
        }

        /// <summary>
        /// Deconstructs the packet.
        /// </summary>
        /// <param name="packet">Packet.</param>
        /// <param name="hmac">Hmac.</param>
        /// <param name="message">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void DeconstructPacket<T>(byte[] packet, out byte[] hmac, out T message)
        {
            hmac = new byte[32];
            message = default(T);

            Array.Copy(packet, 4, hmac, 0, 32);
            byte[] msg = new byte[packet.Length - 36];
            Array.Copy(packet, 36, msg, 0, packet.Length - 36);
            message = JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(msg));
        }

        /// <summary>
        /// Decodes the type of the packet.
        /// </summary>
        /// <returns>The packet type.</returns>
        /// <param name="packet">Packet.</param>
        public static PacketType DecodePacketType(byte[] packet)
        {
            byte[] packetType = new byte[4];
            Array.Copy(packet, packetType, 4);
            return new PacketType(BitConverter.ToInt32(packetType));
        }

        #endregion

    }
}
