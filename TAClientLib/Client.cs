using System;
using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace TAClientLib
{

    public class ServerCommandEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }
        public string OriginalHMAC { get; set; }
        public string ComputedHMAC { get; set; }
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
        public bool IsListening { get; set; }

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
        public delegate void EntitiesListResponseHandler(EntitiesList e);

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
            byte[] outStream = Encoding.ASCII.GetBytes("client_disconnect");
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
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


                byte[] hmac = new byte[32];
                Array.Copy(bytesFrom, hmac, 32);
                byte[] data = new byte[bytesFrom.Length - 32];
                Array.Copy(bytesFrom, 32, data, 0, bytesFrom.Length - 32);
                string str = Encoding.ASCII.GetString(data);

                byte[] computedHmac = Helpers.Encode(data, key);

                if (Helpers.ByteArrayToString(hmac) != Helpers.ByteArrayToString(computedHmac)) {
                    InvalidHMAC(
                        new ServerCommandEventArgs
                        {
                            Message = "",
                            OriginalHMAC = Helpers.ByteArrayToString(hmac),
                            ComputedHMAC = Helpers.ByteArrayToString(computedHmac),
                            EnableSpy = false,
                            SpyIP = "",
                            SpyPort = -1
                        });
                    return;
                }

                bool valid = TryParseJSON(str, out NetworkPacket packet, out ServerCommand cmd, out EntitiesList entitiesList);

                if (valid)
                {
                    if (packet != null)
                    {

                    }
                    else if (cmd != null)
                    {
                        switch (cmd.Command)
                        {
                            case "connected":
                                Connected(
                                    new ServerCommandEventArgs
                                    {
                                        Message = "",
                                        EnableSpy = cmd.EnableSpy,
                                        SpyIP = cmd.SpyIP,
                                        SpyPort = cmd.SpyPort,
                                        OriginalHMAC = Helpers.ByteArrayToString(hmac),
                                        ComputedHMAC = Helpers.ByteArrayToString(computedHmac)
                                    });
                                break;
                            case "notfound":
                                Rejected(
                                    new ServerCommandEventArgs
                                    {
                                        Message = "Entity not found",
                                        EnableSpy = cmd.EnableSpy,
                                        SpyIP = cmd.SpyIP,
                                        SpyPort = cmd.SpyPort,
                                        OriginalHMAC = Helpers.ByteArrayToString(hmac),
                                        ComputedHMAC = Helpers.ByteArrayToString(computedHmac)
                                    });
                                break;
                            case "invalidkey":
                                Rejected(
                                    new ServerCommandEventArgs
                                    {
                                        Message = "Invalid key or HMAC missmatch on server",
                                        EnableSpy = cmd.EnableSpy,
                                        SpyIP = cmd.SpyIP,
                                        SpyPort = cmd.SpyPort,
                                        OriginalHMAC = Helpers.ByteArrayToString(hmac),
                                        ComputedHMAC = Helpers.ByteArrayToString(computedHmac)
                                    });
                                break;
                            case "alreadyconnected":
                                Rejected(
                                    new ServerCommandEventArgs
                                    {
                                        Message = "Entity conntected from other computer or secondary instance",
                                        EnableSpy = cmd.EnableSpy,
                                        SpyIP = cmd.SpyIP,
                                        SpyPort = cmd.SpyPort,
                                        OriginalHMAC = Helpers.ByteArrayToString(hmac),
                                        ComputedHMAC = Helpers.ByteArrayToString(computedHmac)
                                    });
                                break;
                        }
                    }
                    else if (entitiesList != null) {

                    }
                }
            }
        }

        public void Disconnect(bool userDisconnect = false)
        {
            if (userDisconnect)
                SendDisconnectMessage();
            //clientSocket.Close();
            cThread.Abort();
        }

        bool TryParseJSON(string jsonResponse, out NetworkPacket packet, out ServerCommand command, out EntitiesList entitiesList)
        {

            packet = null;
            command = null;
            entitiesList = null;

            if (jsonResponse.Contains("Entity") &&
                jsonResponse.Contains("Operation") &&
                jsonResponse.Contains("Message"))
            {
                packet = JsonConvert.DeserializeObject<NetworkPacket>(jsonResponse);
                return true;
            }
            if (jsonResponse.Contains("Command") &&
                jsonResponse.Contains("EnableSpy") &&
                jsonResponse.Contains("SpyIP") &&
                jsonResponse.Contains("SpyPort")) {
                command = JsonConvert.DeserializeObject<ServerCommand>(jsonResponse);
                return true;
            }
            if (jsonResponse.Contains("Entities")) {
                entitiesList = JsonConvert.DeserializeObject<EntitiesList>(jsonResponse);
            }
            return false;

        }

    }
}
