using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Linq;
using System.Net.Sockets;
using Newtonsoft.Json;
using static TrustAgent.StandardPrints;
using System.Text;

namespace TrustAgent
{
    class Program
    {

        #region "Variables"

        static bool enableDebug;
        static string preCommand = "TrustAgent () > ";
        static string computerName;
        static int seed1 = 100000;
        static int seed2 = 100000;
        static int port = 12345;
        static string execPath;
        static bool enableSpy;

        public static TADatabase db;
        public static Server sv;
        public static MenuHandler mh;
        public static Client spyClient;

        #endregion

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// Performs a basic loading of the TrustAgent server
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            Console.Clear();
            ProcessLog(ProcessPrint.Info, "TrustAgent is initializing...");
            ProcessLog(ProcessPrint.Info, "TrustAgent is using the cipher AES256");
            ProcessLog(ProcessPrint.Info, "TrustAgent is using the hash SHA-256 for HMAC");

            if (args.Length > 0)
            {
                enableSpy |= args.Contains("-spy");

                if (args.Contains("-debug"))
                {
                    ProcessLog(ProcessPrint.Info, "Debug is enabled!");
                    enableDebug = true;
                }

                CheckSeed1(args);

                CheckSeed2(args);

                if (args.Contains("-port"))
                {
                    int pos = Array.IndexOf(args, "-port");
                    bool success = int.TryParse(args[pos + 1], out port);
                    if (!success)
                    {
                        ProcessLog(ProcessPrint.Warn, "Unable to fetch the port, generating one...");
                    }
                }
                else
                    ProcessLog(ProcessPrint.Warn, "Port not detected, generating one...");
            } else {
                // The trust agent was started with no arguments
                ProcessLog(ProcessPrint.Warn, "seed1 argument not found!");
                ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed1);
                else
                    seed1 = Helpers.GenerateSeed();

                ProcessLog(ProcessPrint.Warn, "seed2 argument not found!");
                ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed2);
                else
                    seed2 = Helpers.GenerateSeed();

                ProcessLog(ProcessPrint.Warn, "Port not detected, generating one...");
            }
            App();
        }

        #region "Startup helpers"

        /// <summary>
        /// Checks if the first seed (seed1) is passed as argument, if not it asks to
        /// generate a new seed1 or enter a value.
        /// </summary>
        /// <param name="args">Main args to check if parameter exists.</param>
        static void CheckSeed1(string[] args) {
            if (args.Contains("-seed1"))
            {
                int pos = Array.IndexOf(args, "-seed1");
                bool success = int.TryParse(args[pos + 1], out seed1);
                if (!success)
                {
                    ProcessLog(ProcessPrint.Warn, "Unable to fech the seed, if data is stored it will not be possible to decrypt it!");
                    ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                    var a = Console.ReadLine().ToBool();
                    if (!a)
                        RequestSeed(out seed1);
                    else
                        seed1 = Helpers.GenerateSeed();
                } else {
                    if (seed1 < 100000 || seed1 > 999999)
                        RequestSeed(out seed1);
                }
            }
            else
            {
                ProcessLog(ProcessPrint.Warn, "Argument seed1 not found!");
                ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed1);
                else
                    seed1 = Helpers.GenerateSeed();
            }
        }

        /// <summary>
        /// Checks if the second seed (seed2) is passed as argument, if not it asks to
        /// generate a new seed1 or enter a value.
        /// </summary>
        /// <param name="args">Main args to check if parameter exists.</param>
        static void CheckSeed2(string[] args)
        {
            // Request the seed1 that will generate the key to encrypt the data
            // It can be passed by argument using -seed1 [seed] 
            // If seed1 is not passed on the arguments it's possible to generate a new seed or enter a seed
            if (args.Contains("-seed2"))
            {
                int pos = Array.IndexOf(args, "-seed2");
                bool success = int.TryParse(args[pos + 1], out seed2);
                if (!success)
                {
                    ProcessLog(ProcessPrint.Warn, "Unable to fech the seed, if data is stored it will not be possible to decrypt it!");
                    ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                    var a = Console.ReadLine();
                    if (a != "y")
                        RequestSeed(out seed2);
                    else
                        seed2 = Helpers.GenerateSeed();
                } else {
                    if (seed1 < 100000 || seed1 > 999999)
                        RequestSeed(out seed1);
                }
            }
            else
            {
                ProcessLog(ProcessPrint.Warn, "Argument seed2 not found!");
                ProcessLog(ProcessPrint.Question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed2);
                else
                    seed2 = Helpers.GenerateSeed();
            }
        }

        /// <summary>
        /// Small helper to request a valid seed value 
        /// </summary>
        /// <param name="seed">The entered seed.</param>
        static void RequestSeed(out int seed) {
            ProcessLog(ProcessPrint.Warn, "If the entered seed does not match the previous used seed the stored data will be overwritten!");
            ProcessLog(ProcessPrint.Input, "Please enter a value between 100000 and 999999");
            bool success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            while (!success) {
                ProcessLog(ProcessPrint.Input, "Please enter a value between 100000 and 999999");
                success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            }
        }

        #endregion

        /// <summary>
        /// Secondary TrustAgent server initialization, this loads Information about
        /// the computer such as IP's and computer name.
        /// It also initializes the TrustAgent server listner and starts awaiting for commands
        /// </summary>
        static void App()
        {
            List<IPAddress> ips = Helpers.GetLocalIPAddress();
            computerName = Dns.GetHostName().Replace(".local", "").Replace(".lan", "");
            preCommand = "TrustAgent (" + computerName + ") > ";
            execPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if (enableDebug)
            {
                ProcessLog(ProcessPrint.Debug, "Executable path: " + execPath);
                ProcessLog(ProcessPrint.Debug, "Computer name: " + computerName);
                ProcessLog(ProcessPrint.Debug, "Fetching available IP's");
                foreach (IPAddress ip in ips)
                {
                    ProcessLog(ProcessPrint.Debug, ip.ToString());
                }
            }
            if (ips.Count <= 0)
            {
                ProcessLog(ProcessPrint.Critical, "There are no available networks!");
                Environment.Exit(0);
            }
            Console.WriteLine("");
            ProcessLog(ProcessPrint.Info, "Type help at anytime to show the help menu\n");
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            db = new TADatabase(seed1, seed2, dbPath + "/tadb.bin");
            sv = new Server(ips.First(), 12345);
            mh = new MenuHandler();

            byte[] connectedString = Encoding.ASCII.GetBytes("f");
            byte[] size = BitConverter.GetBytes(connectedString.Length);
            byte[] packet = new byte[connectedString.Length + 4];
            Array.Copy(size, 0, packet, 0, 4);
            Array.Copy(connectedString, 0, packet, 4, connectedString.Length);

            if (enableSpy)
            {
                spyClient = new Client();
                spyClient.Connect(ips.First().ToString(), 22334, packet);
            }



            sv.ClientConnected += Sv_ClientConnected;
            mh.CommandReceived += Mh_CommandReceived;

        }

        static void Mh_CommandReceived(MenuHandler.Menu menu, object command, MenuEventArgs e)
        {
            switch (menu) {
                case MenuHandler.Menu.Keys:
                    switch ((MenuHandler.KeysCommands)command) {
                        case MenuHandler.KeysCommands.List:
                            ListEntities(e);
                            break;
                        case MenuHandler.KeysCommands.Add:
                            AddEntity(e);
                            break;
                        case MenuHandler.KeysCommands.Del:
                            DelEntity(e);
                            break;
                        case MenuHandler.KeysCommands.ListChanges:
                            ActionProcessor.Keys.PrintChanges();
                            break;
                    }
                    break;
                case MenuHandler.Menu.Server:
                    switch ((MenuHandler.ServerCommands)command) {
                        case MenuHandler.ServerCommands.List:
                            ListConnectedEntities();
                            break;
                    }
                    break;
            }
        }

        #region "MenuHandler Helpers"

        static void ListEntities(MenuEventArgs e) {
            bool showKey = (bool)e.Arguments.First(m => m.Key.Equals(MenuHandler.KeysArgs.ShowKey)).Value;
            db.PrintEntities(showKey);
        }

        static void AddEntity(MenuEventArgs e) {
            string entity = (string)e.Arguments.First(m => m.Key.Equals(MenuHandler.KeysArgs.Entity)).Value;
            string key = (string)e.Arguments.First(m => m.Key.Equals(MenuHandler.KeysArgs.Key)).Value;
            ActionProcessor.Keys.PerformAction(ActionProcessor.Keys.Action.add, entity, key);
        }

        static void DelEntity(MenuEventArgs e) {
            string entity = (string)e.Arguments.First(m => m.Key.Equals(MenuHandler.KeysArgs.Entity)).Value;
            ActionProcessor.Keys.PerformAction(ActionProcessor.Keys.Action.del, entity, "");
        }

        static void ListConnectedEntities() {
            Console.WriteLine("");
            List<(string, string)> cmds = new List<(string, string)>();
            foreach (string key in sv.clientsList.Keys)
            {
                IPAddress ip = ((IPEndPoint)((TcpClient)sv.clientsList[key]).Client.RemoteEndPoint).Address;
                cmds.Add((key, ip.ToString()));
            }
            Console.WriteLine(cmds.ToStringTable(
                new[] { "Entity", "IP" },
                a => a.Item1, a => a.Item2));
        }

        #endregion

        static void Sv_ClientConnected(byte[] m, TcpClient socket, EventArgs e)
        {
            byte[] hmac = new byte[32];
            Array.Copy(m, hmac, 32);
            byte[] data = new byte[m.Length - 32];
            Array.Copy(m, 32, data, 0, m.Length - 32);
            string str = Encoding.ASCII.GetString(data);
            NetworkPacket packet = JsonConvert.DeserializeObject<NetworkPacket>(str);
            if (enableDebug) {
                Helpers.ReplaceWith("");
                ProcessLog(ProcessPrint.Debug, "New entity is trying to connect: " + packet.Entity);
            }

            TADatabase.ValidationError validation = db.ValidateEntity(packet, hmac, data);
            IPAddress ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;
            IPAddress local = ((IPEndPoint)socket.Client.LocalEndPoint).Address;
            if (validation == TADatabase.ValidationError.notFound)
                ProcessLog(ProcessPrint.Debug, string.Format("Connection from entity {0} ({1}) refused, the user was not found", packet.Entity, ip));
            if (validation == TADatabase.ValidationError.invalidKey)
                ProcessLog(ProcessPrint.Debug, string.Format("Connection from entity {0} ({1}) refused, the user has an invalid key or tempered data", packet.Entity, ip));
            if (validation == TADatabase.ValidationError.noError) {
                if (enableDebug)
                    ProcessLog(ProcessPrint.Debug, string.Format("Connection from entity {0} ({1}) accepted", packet.Entity, ip));
                sv.AcceptClient(packet.Entity, socket, enableSpy, local.ToString(), 22334);
            }
                
            Console.WriteLine("");
            Console.Write(mh.commandPrefix);

        }

    }
}
