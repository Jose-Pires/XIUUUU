using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Net.Sockets;

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

        static int dbSeed1;
        static int dbSeed2;
        static string dbPath;

        static string commandPrompt;
        static string computerName;

        public static Database database;
        public static Server server;
        static MenuManager menuManager;
        public static Client spy;

        #endregion

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

            ProcessDebugMessage(string.Format("Computer name: {0} > ", computerName));
            ProcessDebugMessage(string.Format("Fetchig available IP's: {0} > ", computerName));

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

        static void Server_MessageReceived(byte[] m, ClientHandler clientHandler)
        {
        }

        static void Server_ClientConnected(byte[] m, TcpClient socket, EventArgs e)
        {
            IPAddress ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;
            IPAddress local = ((IPEndPoint)socket.Client.LocalEndPoint).Address;

            Server.DeconstructPacket(m, out byte[] hmac, out ClientMessage message);
            if (enableDebug)
                ReplaceWith("");
            ProcessDebugMessage(string.Format("New entity is trying to connect: {0} ({1})", message.Entity, ip));

            Database.ValidationError validation = database.ValidateEntity(message, hmac, m);
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
            Console.WriteLine("");
            Console.WriteLine("\r" + menuManager.commandPrefix);
        }


        #region "Initialization Helpers"

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
                ProcessDebugMessage("Port not detected, falling to default!");

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
            }
        }

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
            }
        }



        #endregion

    }
}
