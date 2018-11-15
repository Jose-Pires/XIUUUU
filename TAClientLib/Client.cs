using System;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using static TAClientLib.SHA256hmac;

namespace TAClientLib
{

    public class ServerCommandEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }
        public byte[] OriginalHMAC { get; set; }
        public byte[] ComputedHMAC { get; set; }
    }

    public enum ConnectionErrors {
        RejectedNotFound,
        RejectedInvalidKey,
        RejectedAlreadyConnected,
        HMACMissMatch,
        ConnectionFailed,
        NoError
    }

    /// <summary>
    /// Client class that will connect and "talk" with the trust agent
    /// </summary>
    internal class Client
    {
        public const string DEFAULT_KEY = "a77acc4e2b2586f8b58795573c5227661f93877abcc3878ba6c6179022e10f4e";
        public bool IsListening { get; set; }
        public byte[] Key { get; }

        TcpClient clientSocket;
        Thread cThread;
        NetworkStream serverStream;
        readonly byte[] key;

        public event ServerCommandHandler Connected;
        public event ServerCommandHandler Rejected;
        public event ServerCommandHandler InvalidHMAC;
        public event ServerCommandHandler ConnectionFailed;
        public delegate void ServerCommandHandler(ServerCommandEventArgs e);
        public event EntitiesListResponseHandler EntityListReceived;
        public delegate void EntitiesListResponseHandler(EntityClass e);

        internal Client(byte[] key) {
            this.key = key;
        }

        public void Connect(string ip, int port, byte[] data)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(ip, port);
            serverStream = clientSocket.GetStream();
            serverStream.Write(data, 0, data.Length);
            serverStream.Flush();

            cThread = new Thread(GetMessage);
            cThread.Start();
        }

        public void SendRequest(byte[] message) {
            serverStream.Write(message, 0, message.Length);
            serverStream.Flush();
        }

        void SendDisconnectMessage()
        {
            //TODO: Send propper shutdown message
        }

        void GetMessage()
        {
            while (true)
            {

                serverStream = clientSocket.GetStream();
                byte[] dataLength = new byte[4];
                serverStream.Read(dataLength, 0, 4);
                int dLength = BitConverter.ToInt32(dataLength, 0);
                byte[] bytesFrom = new byte[dLength];
                serverStream.Read(bytesFrom, 0, dLength);

                PacketType packetType = DecodePacketType(bytesFrom);

                switch (packetType.Value) {
                    case 1:
                        ProcessServerCommand(bytesFrom);
                        break;
                    case 3:
                        ProcessServerResponse(bytesFrom);
                        break;
                }

            }
        }

        #region "Packet Processors"

        void ProcessServerCommand(byte[] data) {
            DeconstructPacket(data, out byte[] hmac, out ServerCommand serverCommand, out byte[] raw);

            bool valid_comm = CompareHMAC(hmac, ComputeHMAC(raw, key));

            if (valid_comm) {
                if (serverCommand.Command == ServerOperations.Connected.Value) {
                    Connected(
                        new ServerCommandEventArgs
                        {
                            SpyIP = serverCommand.SpyIP,
                            SpyPort = serverCommand.SpyPort,
                            OriginalHMAC = hmac,
                            ComputedHMAC = ComputeHMAC(raw, key),
                            EnableSpy = serverCommand.EnableSpy,
                            Message = serverCommand.Message
                        });
                } else if (serverCommand.Command == ServerOperations.ConnectionRefused.Value) {
                    Rejected(new ServerCommandEventArgs
                    {
                        OriginalHMAC = hmac,
                        ComputedHMAC = ComputeHMAC(raw, key),
                        Message = serverCommand.Message
                    });
                } else if (serverCommand.Command == ServerOperations.ServerShutdown.Value) {
                    //TODO: Handle server shutdown
                }
            } else {
                bool valid_default = CompareHMAC(hmac, ComputeHMAC(raw, DEFAULT_KEY.FromHexToByteArray()));
                if (valid_default) {
                    if (serverCommand.Command == ServerOperations.ConnectionRefused.Value) {
                        if (serverCommand.Message == ServerMessages.InvalidKey.Value) {
                            Rejected(new ServerCommandEventArgs
                            {
                                Message = "The key used to comunicate with the server seems invalid or you were victim of MITM Attack",
                                ComputedHMAC = ComputeHMAC(raw, DEFAULT_KEY.FromHexToByteArray()),
                                OriginalHMAC = hmac
                            });
                        }
                    }
                } else {
                    InvalidHMAC(new ServerCommandEventArgs
                    {
                        Message = "The HMACs does not match",
                        OriginalHMAC = hmac,
                        ComputedHMAC = ComputeHMAC(raw, key)
                    });
                }
            }
        }

        void ProcessServerResponse(byte[] data) {
            byte[] _data = new byte[data.Length - 32];
            Array.Copy(data, 32, _data, 0, data.Length - 32);
            DeconstructPacket(data, out byte[] hmac, out ServerCommand serverCommand, out byte[] raw);

            bool valid_comm = CompareHMAC(hmac, ComputeHMAC(_data, key));
        }

        #endregion

        public void Disconnect(bool userDisconnect = false)
        {
            if (userDisconnect)
                SendDisconnectMessage();
            //clientSocket.Close();
            cThread.Abort();
        }

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
            byte[] size = BitConverter.GetBytes(hmac.Length + data.Length + 4);
            byte[] packet = new byte[data.Length + hmac.Length + 8];
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
        public static void DeconstructPacket<T>(byte[] packet, out byte[] hmac, out T message, out byte[] raw)
        {
            hmac = new byte[32];
            message = default(T);
            Array.Copy(packet, 4, hmac, 0, 32);
            byte[] msg = new byte[packet.Length - 36];
            Array.Copy(packet, 36, msg, 0, packet.Length - 36);
            raw = msg;
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
            return new PacketType(BitConverter.ToInt32(packetType,0));
        }

        #endregion


    }
}
