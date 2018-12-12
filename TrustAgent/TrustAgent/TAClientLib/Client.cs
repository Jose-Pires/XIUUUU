/*
 * TAClientLib.Server.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * This class handles all server related operations on the network level
 * 
 * Requires initialization: YES
 * 
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

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
    class Client
    {
        public const string DEFAULT_KEY = "a77acc4e2b2586f8b58795573c5227661f93877abcc3878ba6c6179022e10f4e";
        public bool IsListening { get; set; }
        public byte[] Key { get; }

        public TcpClient clientSocket;
        Thread cThread;
        NetworkStream serverStream;
        readonly byte[] key;
        bool stop;

        public event ServerCommandHandler Connected;
        public event ServerCommandHandler Rejected;
        public event ServerCommandHandler InvalidHMAC;
        public event ServerCommandHandler ConnectionFailed;
        public event ServerCommandHandler ClientKicked;
        public event ServerCommandHandler InvalidTime;
        public event ServerCommandHandler InvalidComand;
        public event ServerCommandHandler EntityNotFound;
        public event ServerCommandHandler Disconnected;
        public delegate void ServerCommandHandler(ServerCommandEventArgs e);

        public event EntitiesListResponseHandler EntityListReceived;
        public delegate void EntitiesListResponseHandler(List<(string,string)> e);

        public event KeyNegotiationHandler KeyReceived;
        public delegate void KeyNegotiationHandler(byte[] key, IPAddress remoteIP, int remotePORT);

        internal Client(byte[] key) {
            this.key = key;
        }

        /// <summary>
        /// Connect the specified ip, port and sends the data of the first packet.
        /// </summary>
        /// <param name="ip">Ip.</param>
        /// <param name="port">Port.</param>
        /// <param name="data">Data.</param>
        public void Connect(string ip, int port, byte[] data)
        {
            try {
                clientSocket = new TcpClient();
                clientSocket.Connect(ip, port);
                serverStream = clientSocket.GetStream();
                serverStream.Write(data, 0, data.Length);
                serverStream.Flush();
            } catch (Exception) {
                ConnectionFailed(null);
            }


            cThread = new Thread(GetMessage);
            cThread.Start();
        }

        /// <summary>
        /// Sends a request, the packet must be pre-built
        /// </summary>
        /// <param name="message">Message.</param>
        public void SendRequest(byte[] message) {
            serverStream.Write(message, 0, message.Length);
            serverStream.Flush();
        }

        /// <summary>
        /// Sends a disconnect message
        /// </summary>
        void SendDisconnectMessage()
        {
            stop = true;
            ClientMessage payload = new ClientMessage
            {
                Operation = ClientOperations.Disconnect.Value
            };
            byte[] packet = BuildPacket(payload, key, PacketType.ClientMessage);
            SendRequest(packet);
        }

        /// <summary>
        /// Waits for and gets a server message
        /// </summary>
        void GetMessage()
        {
            while (!stop)
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
                    case 2:
                        ProcessServerResponse(bytesFrom);
                        break;
                }

            }
        }

        #region "Packet Processors"

        /// <summary>
        /// Processes a server comand
        /// </summary>
        /// <param name="data">Data.</param>
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
                    Disconnected(null);
                } else if (serverCommand.Command == ServerOperations.EntityNoLongerAvailable.Value) {
                    EntityNotFound(null);
                } else if (serverCommand.Command == ServerOperations.InvalidComand.Value) {
                    InvalidComand(null);
                } else if (serverCommand.Command == ServerOperations.InvalidHMAC.Value) {
                    InvalidHMAC(new ServerCommandEventArgs
                    {
                        OriginalHMAC = Encoding.ASCII.GetBytes(serverCommand.Message.Split('|')[0].Split('=')[1]),
                        ComputedHMAC = Encoding.ASCII.GetBytes(serverCommand.Message.Split('|')[1].Split('=')[1])
                    });
                } else if (serverCommand.Command == ServerOperations.InvalidTime.Value) {
                    InvalidTime(null);
                } else if (serverCommand.Command == ServerOperations.KickEntity.Value) {
                    ClientKicked(null);
                    stop = true;
                } else if (serverCommand.Command == ServerOperations.ResponseSuccessEntities.Value) {
                    byte[] decodedBytes = Convert.FromBase64String(serverCommand.Message);
                    string decodedJson = Encoding.Unicode.GetString(decodedBytes);
                    List<(string, string)> entities = JsonConvert.DeserializeObject<List<(string, string)>>(decodedJson);
                    EntityListReceived(entities);
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

        /// <summary>
        /// Processes a response from the server
        /// </summary>
        /// <param name="data">Data.</param>
        void ProcessServerResponse(byte[] data)
        {

            Console.WriteLine("Received Response");

            byte[] _data = new byte[data.Length - 32];
            Array.Copy(data, 32, _data, 0, data.Length - 32);
            DeconstructPacket(data, out byte[] hmac, out ClientMessage serverCommand, out byte[] raw);

            bool valid_comm = CompareHMAC(hmac, ComputeHMAC(raw, key));

            if (valid_comm)
            {

                if (serverCommand.Operation == ClientOperations.RequestKeyNegotiation.Value)
                {
                    string destEntity = serverCommand.Message.Split('|')[0];
                    string destEndpoint = serverCommand.Message.Split('|')[1];
                    string randKey = serverCommand.Message.Split('|')[2];
                    string randIV = serverCommand.Message.Split('|')[3];

                    byte[] unencryptedKey = AESCipher.DecryptData(Convert.FromBase64String(randKey), key, Convert.FromBase64String(randIV));

                    IPAddress ip = IPAddress.Parse(destEndpoint.Split(':')[0]);

                    KeyReceived(unencryptedKey, ip, int.Parse(destEndpoint.Split(':')[1]));

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

        #endregion

        /// <summary>
        /// Disconnects the user from the server (safe disconnect)
        /// </summary>
        /// <param name="userDisconnect">If set to <c>true</c> user disconnect.</param>
        public void Disconnect(bool userDisconnect = false)
        {
            stop = true;
            if (userDisconnect)
                SendDisconnectMessage();
            clientSocket.Close();
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
