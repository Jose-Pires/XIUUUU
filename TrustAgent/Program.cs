/*
 * TrustAgent.Program.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * This class is the main program, handles the startup, and the events of the 
 * database, server and menuhandler
 * 
 * Requires initialization: NO
 * Contains:
 *     Class Level Constants: 1 Public
 *     Class Level Variables: 4 Private, 8 Public
 *     Methods:
 *         Static: 7 Public 
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;

using static TrustAgent.StandardPrints;
using static TrustAgent.Helpers;
using static TrustAgent.AESCipher;
using System.Text;

namespace TrustAgent
{
    class Program
    {

        #region "Constants"

        //Used when there is no need to use a key or it's not possible to use one,
        //This key IS NOT to build a secure/valid HMAC but rather for the system
        //to be able to generate an hmac as all functions require
        public const string DEFAULT_KEY = "a77acc4e2b2586f8b58795573c5227661f93877abcc3878ba6c6179022e10f4e";

        #endregion

        #region "Variables"

        public static bool enableDebug;
        public static bool enableSpy;
        static int serverPort = 12345;

        public static int dbSeed1;
        public static int dbSeed2;
        static string dbPath;

        static string commandPrompt;
        static string computerName;

        public static Database database;
        public static Server server;
        public static MenuManager menuManager;
        public static Client spy;

        #endregion

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            Console.Clear();
            ProcessLog(ProcessPrint.Info, "TrustAgent is initializing...");
            dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/tadb.bin";

            if (args.Length > 0)
                ProcessArgs(args);
            else {
                Seed1Validation();
                Seed2Validation();
            }

            List<IPAddress> ips = GetLocalIPAddress();
            computerName = Dns.GetHostName().Replace(".local", "").Replace(".lan", "");
            commandPrompt = string.Format("TrustAgent ({0}) > ", computerName);

            ProcessDebugMessage(string.Format("Computer name: {0} ", computerName));
            ProcessDebugMessage("Fetchig available IP's");

            if (ips.Count <= 0)
            {
                ProcessLog(ProcessPrint.Critical, "There are no available networks!");
                Environment.Exit(0);
            }

            foreach (IPAddress ip in ips)
                ProcessDebugMessage(ip.ToString());

            database = new Database(dbSeed1, dbSeed2, dbPath);

            if (File.Exists(dbPath)) {
                if (!ValidateDecryption(database.entities, 
                                       GenKey(dbSeed1, database.selector),
                                       GenIV(dbSeed2, database.selector))) {
                    File.Delete(dbPath);
                    ProcessLog(ProcessPrint.Error, "Incorrect seeds! Database deleted to prevent bruteforce!");
                    database.entities = null;
                    database.selector = 0;
                    database.fileExists = false;
                }
            }

            Console.WriteLine("");
            ProcessLog(ProcessPrint.Info, "Type help at anytime to show the help menu\n");

            server = new Server(ips.First(), serverPort);
            server.ClientConnected += Server_ClientConnected;
            server.MessageReceived += Server_MessageReceived;

            if (enableSpy) {
                //To connect for the first time to the spy
                byte[] connectedString = Encoding.ASCII.GetBytes("f");
                byte[] size = BitConverter.GetBytes(connectedString.Length);
                byte[] packet = new byte[connectedString.Length + 4];
                Array.Copy(size, 0, packet, 0, 4);
                Array.Copy(connectedString, 0, packet, 4, connectedString.Length);
                spy = new Client();
                spy.Connect(ips.First().ToString(), 11223, packet);
            }

            menuManager = new MenuManager();

        }

        /// <summary>
        /// Handles all messages received by the client (except the first one)
        /// </summary>
        /// <param name="m">Packet without the byte length</param>
        /// <param name="clientHandler">Client handler.</param>
        static void Server_MessageReceived(byte[] m, ClientHandler clientHandler)
        {
            IPAddress ip = ((IPEndPoint)clientHandler.Socket.Client.RemoteEndPoint).Address;

            PacketType packetType = Server.DecodePacketType(m);
            Server.DeconstructPacket(m, out byte[] hmac, out ClientMessage message, out byte[] raw);

            if (enableDebug)
                ReplaceWith("");
            ProcessDebugMessage(string.Format("Message received from {0} ({1})", message.Entity, ip));

            // This step allows the verification of:
            // 1. if the entity is still accepted by the server
            // 2. if the hmac is corect 
            Database.ValidationError validation = database.ValidateEntity(message, hmac, raw);

            switch (validation) {
                case Database.ValidationError.NoError:
                    if (ClientOperations.RequestConnectedEntities.Value == message.Operation)
                        RequestedEntitiesList(clientHandler);
                    else {
                        ServerCommand sv_cmd_invalid_operation = new ServerCommand
                        {
                            Command = ServerOperations.InvalidComand.Value
                        };
                        server.SendMessage(sv_cmd_invalid_operation,
                                       PacketType.ServerCommand,
                                       database.RetreiveEntityKey(clientHandler.Entity),
                                       clientHandler.Socket);
                    }
                    break;
                case Database.ValidationError.InvalidKey:
                    ProcessDebugMessage(string.Format("HMAC for message from {0} ({1}) failed!", message.Entity, ip));
                    ServerCommand sv_cmd_invalid_hmac = new ServerCommand
                    {
                        Command = ServerOperations.InvalidHMAC.Value,
                        Message = string.Format("RECEIVED_HMAC={0}|COMPUTED_HMAC={1}", hmac.FromByteArrayToHex(), SHA256hmac.ComputeHMAC(raw, database.RetreiveEntityKey(clientHandler.Entity)).FromByteArrayToHex())
                    };
                    server.SendMessage(sv_cmd_invalid_hmac, 
                                       PacketType.ServerCommand, 
                                       DEFAULT_KEY.FromHexToByteArray(), 
                                       clientHandler.Socket);
                    break;
                case Database.ValidationError.NotFound:
                    ProcessDebugMessage(string.Format("Entity {0} at {1} not found!", message.Entity, ip));
                    ServerCommand sv_cmd_entity_not_found = new ServerCommand
                    {
                        Command = ServerOperations.EntityNoLongerAvailable.Value
                    };
                    server.SendMessage(sv_cmd_entity_not_found, 
                                       PacketType.ServerCommand, 
                                       database.RetreiveEntityKey(clientHandler.Entity), 
                                       clientHandler.Socket);
                    break;
                case Database.ValidationError.InvalidTime:
                    ProcessDebugMessage("Packet time missmatch");
                    ServerCommand sv_cmd_ts_missmatch = new ServerCommand
                    {
                        Command = ServerOperations.InvalidTime.Value
                    };
                    server.SendMessage(sv_cmd_ts_missmatch, 
                                       PacketType.ClientMessage, 
                                       database.RetreiveEntityKey(clientHandler.Entity), 
                                       clientHandler.Socket);
                    break;
            }

            if (enableDebug) {
                Console.WriteLine("");
                Console.Write("\r" + menuManager.commandPrefix);
            }

        }

        /// <summary>
        /// Sends the message to the client with the connected clients
        /// </summary>
        /// <param name="clientHandler">Client handler.</param>
        static void RequestedEntitiesList(ClientHandler clientHandler) {
            List<(string, string)> entities = FetchConnectedEntities(clientHandler.Entity);
            ServerCommand sv_cmd_entities = new ServerCommand
            {
                Command = ServerOperations.ResponseSuccessEntities.Value,
                Message = Convert.ToBase64String(Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(entities)))
            };
            server.SendMessage(sv_cmd_entities,
                               PacketType.ServerCommand,
                               database.RetreiveEntityKey(clientHandler.Entity),
                               clientHandler.Socket);
        }

        /// <summary>
        /// Begins the negotiation the the key between two entities
        /// </summary>
        /// <param name="clientHandler">Client handler.</param>
        /// <param name="destEntity">Destination entity.</param>
        static void NegotiateKey(ClientMessage message, ClientHandler clientHandler) {

            string destEntity = message.Message.Split('|')[0];
            string sourcePort = message.Message.Split('|')[1];
            string key = message.Message.Split('|')[2];
            string iv = message.Message.Split('|')[3];

            byte[] unencryptedKey = DecryptData(Convert.FromBase64String(key), database.RetreiveEntityKey(clientHandler.Entity), Convert.FromBase64String(iv));
            byte[] encryptedKey = EncryptData(unencryptedKey, database.RetreiveEntityKey(destEntity), Convert.FromBase64String(iv));

            IPAddress ip = ((IPEndPoint)clientHandler.Socket.Client.RemoteEndPoint).Address;

            string tmp_msg = string.Format("{0}|{1}:{2}|{3}|{4}", clientHandler.Entity, ip.ToString(), sourcePort, Convert.ToBase64String(encryptedKey), iv);

            ClientHandler destClientHandler = (ClientHandler)server.clientHandlers[destEntity];
            IPAddress destip = ((IPEndPoint)clientHandler.Socket.Client.RemoteEndPoint).Address;

            ProcessDebugMessage(string.Format("Key negotiation between {0} ({1}) and {2} ({3}) has started", clientHandler.Entity, ip, destEntity, destip));

            ClientMessage msg = new ClientMessage
            {
                Operation = ClientOperations.RequestKeyNegotiation.Value,
                Message = tmp_msg
            };
            server.SendMessage(msg, PacketType.ClientMessage, database.RetreiveEntityKey(destEntity), destClientHandler.Socket);
        }

        /// <summary>
        /// Fetchs the connected entities.
        /// </summary>
        /// <returns>The connected entities.</returns>
        static List<(string, string)> FetchConnectedEntities(string entity) {
            List<(string, string)> entities = new List<(string, string)>();
            foreach (ClientHandler client in server.clientHandlers.Values) {
                if (client.Entity == entity)
                    continue;
                IPAddress ip = ((IPEndPoint)client.Socket.Client.RemoteEndPoint).Address;
                entities.Add((client.Entity, ip.ToString()));
            }
            return entities;
        }

        /// <summary>
        /// ClientConnected handler, handles the first connection of a client.
        /// </summary>
        /// <param name="m">Packet without the packet length</param>
        /// <param name="socket">Socket.</param>
        /// <param name="e">E.</param>
        static void Server_ClientConnected(byte[] m, TcpClient socket, EventArgs e)
        {
            IPAddress ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;
            IPAddress local = ((IPEndPoint)socket.Client.LocalEndPoint).Address;

            PacketType packetType = Server.DecodePacketType(m);

            Server.DeconstructPacket(m, out byte[] hmac, out ClientMessage message, out byte[] raw);

            if (enableDebug)
                ReplaceWith("");
            ProcessDebugMessage(string.Format("New entity is trying to connect: {0} ({1})", message.Entity, ip));
            Database.ValidationError validation = database.ValidateEntity(message, hmac, raw);
            switch (validation)
            {
                case Database.ValidationError.NotFound:
                    server.DennyClient(message.Entity, ServerMessages.EntityNotFound, socket, DEFAULT_KEY.FromHexToByteArray());
                    ProcessDebugMessage(string.Format("Connection from entity {0} ({1}) refused! Entity not found!", message.Entity, ip));
                    break;
                case Database.ValidationError.InvalidKey:
                    server.DennyClient(message.Entity, ServerMessages.InvalidKey, socket, DEFAULT_KEY.FromHexToByteArray());
                    ProcessDebugMessage(string.Format("Connection from entity {0} ({1}) refused! Invalid key or message tempered!", message.Entity, ip));
                    break;
                case Database.ValidationError.AlreadyConnected:
                    server.DennyClient(message.Entity, ServerMessages.EntityAlreadyConnected, socket, DEFAULT_KEY.FromHexToByteArray());
                    ProcessDebugMessage(string.Format("Connection from entity {0} ({1}) refused! Entity already connected!", message.Entity, ip));
                    break;
                case Database.ValidationError.NoError:
                    ProcessDebugMessage(string.Format("Connection from entity {0} ({1}) accepted!", message.Entity, ip));
                    server.AcceptClient(message.Entity, socket, database.RetreiveEntityKey(message.Entity), enableSpy, local.ToString());
                    break;
            }

            if (enableDebug)
            {
                Console.WriteLine("");
                Console.Write("\r" + menuManager.commandPrefix);
            }
        }


        #region "Initialization Helpers"

        /// <summary>
        /// Processes the initialization arguments
        /// </summary>
        /// <param name="args">Arguments.</param>
        static void ProcessArgs(string[] args) {
            enableSpy |= args.Contains("-spy");
            enableDebug |= args.Contains("-debug");

            ProcessDebugMessage("TrustAgent is using AES-256-CBC as cipher");
            ProcessDebugMessage("TrustAgent is using SHA-256 to build HMACs");

            ProcessDebugMessage("Debug mode is enabled");
            if (enableSpy)
                ProcessDebugMessage("SPY Mode is enabled");

            if (args.Contains("-port"))
            {
                int pos = Array.IndexOf(args, "-port");
                bool success = int.TryParse(args[pos + 1], out serverPort);
                if (!success)
                    ProcessLog(ProcessPrint.Warn, "Unable to parse the port, falling to default!");
            }
            else
                ProcessLog(ProcessPrint.Warn, "Port not detected, falling to default!");

            if (args.Contains("-seed1")) {
                int pos = Array.IndexOf(args, "-seed1");
                bool success = int.TryParse(args[pos + 1], out dbSeed1);
                if (!success) {
                    Seed1Validation();
                }
            } else {
                Seed1Validation();
            }

            if (args.Contains("-seed2"))
            {
                int pos = Array.IndexOf(args, "-seed2");
                bool success = int.TryParse(args[pos + 1], out dbSeed2);
                if (!success)
                {
                    Seed2Validation();
                }
            }
            else
            {
                Seed2Validation();
            }

        }

        /// <summary>
        /// Validates the seed1 argument
        /// </summary>
        static void Seed1Validation() {
            if (File.Exists(dbPath))
            {
                ProcessLog(ProcessPrint.Error, "Unable to parse seed 1! Further execution will result in data loss!");
                ProcessLog(ProcessPrint.Question, "Do you wish to continue (y/n)");
                if (Console.ReadLine().ToBool())
                {
                    dbSeed1 = GenerateSeed();
                    ProcessLog(ProcessPrint.Info, "New seed was generated. Check the system menu!");
                }
                else
                {
                    ProcessLog(ProcessPrint.Critical, "Server initialization stopped by user");
                    Environment.Exit(0);
                }
            } else
                dbSeed1 = GenerateSeed();
        }

        /// <summary>
        /// Validates the seed2 argument
        /// </summary>
        static void Seed2Validation()
        {
            if (File.Exists(dbPath))
            {
                ProcessLog(ProcessPrint.Error, "Unable to parse seed 2! Further execution will result in data loss!");
                ProcessLog(ProcessPrint.Question, "Do you wish to continue (y/n)");
                if (Console.ReadLine().ToBool())
                {
                    dbSeed2 = GenerateSeed();
                    ProcessLog(ProcessPrint.Info, "New seed was generated. Check the system menu!");
                }
                else
                {
                    ProcessLog(ProcessPrint.Critical, "Server initialization stopped by user");
                    Environment.Exit(0);
                }
            } else
                dbSeed2 = GenerateSeed();
        }

        #endregion

    }
}
