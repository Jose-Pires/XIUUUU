using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Linq;
using SharedModels;
using System.Text;
using System.Security.Cryptography;

namespace TrustAgent
{
    class Program
    {

        public enum ProcessPrint {
            info,
            warn,
            debug,
            error,
            question,
            input,
            critical,
            spy
        }

        #region "Variables"

        static bool enableDebug;
        static Thread thread;
        static string preCommand = "TrustAgent () > ";
        static string computerName;
        static int seed1 = 100000;
        static int seed2 = 100000;
        static int port = 12345;
        public static TADatabase db;
        static string execPath;
        static Server sv;
        static bool enableSpy;

        #endregion

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// Performs a basic loading of the TrustAgent server
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            Console.Clear();
            ProcessLog(ProcessPrint.info, "TrustAgent is initializing...");
            ProcessLog(ProcessPrint.info, "TrustAgent is using AES256");

            if (args.Length > 0)
            {
                if (Helpers.ArrayContains(args, "-spy"))
                {
                    ProcessLog(ProcessPrint.info, "Spy is enabled, any unencripted communications will be visible");
                    enableSpy = true;
                }
                if (Helpers.ArrayContains(args, "-debug"))
                {
                    ProcessLog(ProcessPrint.info, "Debug is enabled!");
                    enableDebug = true;
                }

                CheckSeed1(args);

                CheckSeed2(args);

                if (Helpers.ArrayContains(args, "-port"))
                {
                    int pos = Array.IndexOf(args, "-port");
                    bool success = int.TryParse(args[pos + 1], out port);
                    if (!success)
                    {
                        ProcessLog(ProcessPrint.warn, "Unable to fetch the port, generating one...");
                    }
                }
                else
                    ProcessLog(ProcessPrint.warn, "Port not detected, generating one...");
            } else {
                // The trust agent was started with no arguments
                ProcessLog(ProcessPrint.warn, "seed1 argument not found!");
                ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed1);
                else
                    seed1 = GenerateSeed();

                ProcessLog(ProcessPrint.warn, "seed2 argument not found!");
                ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed2);
                else
                    seed2 = GenerateSeed();

                ProcessLog(ProcessPrint.warn, "Port not detected, generating one...");
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
            if (Helpers.ArrayContains(args, "-seed1"))
            {
                int pos = Array.IndexOf(args, "-seed1");
                bool success = int.TryParse(args[pos + 1], out seed1);
                if (!success)
                {
                    ProcessLog(ProcessPrint.warn, "Unable to fech the seed, if data is stored it will not be possible to decrypt it!");
                    ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                    var a = Console.ReadLine();
                    if (a != "y")
                        RequestSeed(out seed1);
                    else
                        seed1 = GenerateSeed();
                } else {
                    if (seed1 < 100000 || seed1 > 999999)
                        RequestSeed(out seed1);
                }
            }
            else
            {
                ProcessLog(ProcessPrint.warn, "Argument seed1 not found!");
                ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed1);
                else
                    seed1 = GenerateSeed();
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
            if (Helpers.ArrayContains(args, "-seed2"))
            {
                int pos = Array.IndexOf(args, "-seed2");
                bool success = int.TryParse(args[pos + 1], out seed2);
                if (!success)
                {
                    ProcessLog(ProcessPrint.warn, "Unable to fech the seed, if data is stored it will not be possible to decrypt it!");
                    ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                    var a = Console.ReadLine();
                    if (a != "y")
                        RequestSeed(out seed2);
                    else
                        seed2 = GenerateSeed();
                } else {
                    if (seed1 < 100000 || seed1 > 999999)
                        RequestSeed(out seed1);
                }
            }
            else
            {
                ProcessLog(ProcessPrint.warn, "Argument seed2 not found!");
                ProcessLog(ProcessPrint.question, "Do you wish to generate a seed value, this will invalidate any previously stored data (y/n)");
                var a = Console.ReadLine();
                if (a != "y")
                    RequestSeed(out seed2);
                else
                    seed2 = GenerateSeed();
            }
        }

        /// <summary>
        /// Small helper to request a valid seed value 
        /// </summary>
        /// <param name="seed">The entered seed.</param>
        static void RequestSeed(out int seed) {
            ProcessLog(ProcessPrint.warn, "If the entered seed does not match the previous used seed the stored data will be overwritten!");
            ProcessLog(ProcessPrint.input, "Please enter a value between 100000 and 999999");
            bool success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            while (!success) {
                ProcessLog(ProcessPrint.input, "Please enter a value between 100000 and 999999");
                success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            }
        }

        #endregion

        /// <summary>
        /// Secondary TrustAgent server initialization, this loads information about
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
                ProcessLog(ProcessPrint.debug, "Executable path: " + execPath);
                ProcessLog(ProcessPrint.debug, "Computer name: " + computerName);
                ProcessLog(ProcessPrint.debug, "Fetching available IP's");
                foreach (IPAddress ip in ips)
                {
                    ProcessLog(ProcessPrint.debug, ip.ToString());
                }
            }
            if (ips.Count <= 0)
            {
                ProcessLog(ProcessPrint.critical, "There are no available networks!");
                Environment.Exit(0);
            }
            Console.WriteLine("");
            ProcessLog(ProcessPrint.info, "Type help at anytime to show the help menu\n");
            string dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            db = new TADatabase(seed1, seed2, dbPath + "/tadb.bin");
            sv = new Server(ips.First(), 12345);
            sv.ClientConnected += Sv_ClientConnected;
            thread = new Thread(ReadCommands);
            thread.Start();
        }

        static void Sv_ClientConnected(byte[] m, System.Net.Sockets.TcpClient socket, EventArgs e)
        {
            EntityClass entity = Helpers.FromByteArray<EntityClass>(m);
            if (enableDebug) {
                Helpers.ReplaceWith("");
                ProcessLog(ProcessPrint.debug, "New entity connected: " + entity.IdentityName);
            }
            if (enableSpy) {
                int i = 0;
                for (i = m.Length -1; i >= 0; i--)
                {
                    if (m[i] != 0)
                        break;
                }
                byte[] _m = new byte[i];
                Array.Copy(m, _m, i);
                Helpers.ReplaceWith("");
                ProcessLog(ProcessPrint.spy, "This comunication was intercepted");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Console.OutputEncoding = Encoding.GetEncoding(1252);
                Console.WriteLine(Hex.Dump(_m));
                Console.WriteLine("");
            }

            TADatabase.ValidationError validation = db.ValidateEntity(entity);
            IPAddress ip = ((IPEndPoint)socket.Client.RemoteEndPoint).Address;
            if (validation == TADatabase.ValidationError.notFound)
                ProcessLog(ProcessPrint.debug, string.Format("Connection from entity {0} ({1}) refused, the user was not found", entity.IdentityName, ip));
            if (validation == TADatabase.ValidationError.invalidKey)
                ProcessLog(ProcessPrint.debug, string.Format("Connection from entity {0} ({1}) refused, the user has an invalid key", entity.IdentityName, ip));
            if (validation == TADatabase.ValidationError.noError) {
                if (enableDebug)
                    ProcessLog(ProcessPrint.debug, string.Format("Connection from entity {0} ({1}) accepted", entity.IdentityName, ip));
                sv.AcceptClient(entity.IdentityName, socket);
            }
                

            Console.WriteLine("");
            Console.Write(preCommand);
        }


        #region "Command Handler"

        /// <summary>
        /// Command handler, interprets commands on the main menu
        /// </summary>
        static void ReadCommands()
        {
            string cmd = "";
            while (cmd != "exit")
            {
                Console.Write("\r" + preCommand); cmd = Console.ReadLine();
                if (cmd == "keys")
                {
                    bool res = ReadKeysCommands();
                    cmd = (res) ? "exit" : "";
                }
                else if (cmd == "clear")
                    Console.Clear();
                else if (cmd == "help")
                    PrintMainHelp();
                else if (cmd == "exit") {
                    sv.Shutdown();
                    db.WriteToFile();
                } else
                    ProcessLog(ProcessPrint.error, "Command not found!");
            }
        }

        /// <summary>
        /// Command handler, interprets commands on the keys menu
        /// </summary>
        /// <returns><c>true</c>, if the user wants to terminate the server, <c>false</c> otherwise.</returns>
        static bool ReadKeysCommands() {
            string cmd = "";
            preCommand = "TrustAgent (" + computerName + "): keys > ";
            while (cmd != "back")
            {
                Console.Write("\r" + preCommand); cmd = Console.ReadLine();
                if (cmd.Contains("list") && !cmd.Contains("changes"))
                {
                    bool k = false || cmd.Contains("-k");
                    db.PrintEntities(k);
                }
                else if (cmd.Contains("add")) {
                    cmd = cmd.Replace("add ", "").Trim();
                    string[] cmdSplited = cmd.Split(' ');
                    string _key = cmdSplited[cmdSplited.Length - 1];
                    Array.Resize(ref cmdSplited, cmdSplited.Length - 1);
                    string entityName = string.Join(" ", cmdSplited);
                    bool res = ActionProcessor.Keys.PerformAction(ActionProcessor.Keys.Action.add, entityName, _key);
                    if (res)
                    {
                        ProcessLog(ProcessPrint.info, ActionProcessor.Keys.Pending + " changes pending");
                        Console.WriteLine("");
                    }
                }
                else if (cmd.Contains("import"))
                {
                    bool ow = false || cmd.Contains("-overwrite");
                    cmd = cmd.Replace("import ", "").Replace(" -overwrite", "");
                    bool actionResult = ActionProcessor.Keys.PerformAction(cmd);
                    if (actionResult)
                        ProcessLog(ProcessPrint.info, ActionProcessor.Keys.Pending + " changes pending");
                    else
                        ProcessLog(ProcessPrint.error, "File not found");
                }
                else if (cmd.Contains("del")) {
                    cmd = cmd.Replace("del ", "").Trim();
                    bool res = ActionProcessor.Keys.PerformAction(ActionProcessor.Keys.Action.del, cmd, "");
                    if (res)
                    {
                        ProcessLog(ProcessPrint.info, ActionProcessor.Keys.Pending + " changes pending");
                        Console.WriteLine("");
                    }
                }
                else if (cmd == "save")
                    ActionProcessor.Keys.Save();
                else if (cmd == "discard")
                    ActionProcessor.Keys.Discard();
                else if (cmd == "list changes")
                    ActionProcessor.Keys.PrintChanges();
                else if (cmd == "clear")
                    Console.Clear();
                else if (cmd == "help")
                    PrintKeyshelp();
                else if (cmd == "back")
                {
                    if (ActionProcessor.Keys.Pending > 0) {
                        ProcessLog(ProcessPrint.warn, "There are pending changes to apply!");
                        ProcessLog(ProcessPrint.question, "Do you want to discard the changes (y/n)");
                        string a = Console.ReadLine();
                        if (a != "y")
                            ActionProcessor.Keys.Save();
                    } else
                        preCommand = "TrustAgent (" + computerName + ") > ";
                    return false;
                }
                else if (cmd == "exit")
                    return true;
                else
                    ProcessLog(ProcessPrint.error, "Command not found!");
            }
            return false;
        }

        #endregion

        #region "Help Prints"

        /// <summary>
        /// Prints the main menu help
        /// </summary>
        static void PrintMainHelp()
        {
            Console.WriteLine("");
            IEnumerable<Tuple<string, string>> cmds =
                new[]
                {
                  Tuple.Create("keys", "Enters the submenu to manage the keys"),
                  Tuple.Create("system", "Enters the admin menue"),
                  Tuple.Create("",""),
                  Tuple.Create("clear", "Clears the output"),
                  Tuple.Create("help", "Shows the help of the current selected menu"),
                  Tuple.Create("exit", "Terminates the TrustAgent server")
                };
            Console.WriteLine(cmds.ToStringTable(
                new[] { "Command", "Description" },
                a => a.Item1, a => a.Item2));
        }

        /// <summary>
        /// Prints the keys menu help
        /// </summary>
        static void PrintKeyshelp()
        {
            Console.WriteLine("");
            IEnumerable<Tuple<string, string>> cmds =
                new[]
                {
                  Tuple.Create("list [options]", "Lists the entities on the TrustAgent database"),
                  Tuple.Create("add [username] [key]", "Adds a new client to the TrustAgent database"),
                  Tuple.Create("del [username]", "Deletes an client from the TrustAgent database"),
                  Tuple.Create("import [file path] [options]", "Imports data from a csv file"),
                  Tuple.Create("",""),
                  Tuple.Create("save", "Saves current changes without leaving the current menu"),
                  Tuple.Create("discard", "Discards current changes without leaving the current menu"),
                  Tuple.Create("list changes", "Lists all the changes"),
                  Tuple.Create("",""),
                  Tuple.Create("clear", "Clears the output"),
                  Tuple.Create("help", "Shows the help of the current selected menu"),
                  Tuple.Create("back", "Navigates to the previous menu"),
                  Tuple.Create("exit", "Terminates the TrustAgent server")
                };
            Console.WriteLine(cmds.ToStringTable(
                new[] { "Command", "Description" },
                a => a.Item1, a => a.Item2 ));
            Console.WriteLine("list options");
            cmds =
                new[]
                {
                  Tuple.Create("-k", "lists entities and keys")
                };
            Console.WriteLine(cmds.ToStringTable(
                new[] { "Option", "Description" },
                a => a.Item1, a => a.Item2));

            Console.WriteLine("import options");
            cmds =
                new[]
                {
                  Tuple.Create("-overwrite", "overwrites the entire existing database")
                };
            Console.WriteLine(cmds.ToStringTable(
                new[] { "Option", "Description" },
                a => a.Item1, a => a.Item2));
        }

        #endregion

        #region "Helpers"

        /// <summary>
        /// Helps processing logs shown on the console by formatting the spaces,
        /// change the text colors of the INFO, WARN, DEBUG, INPUT, QUESTION, CRYTICAL
        /// and ERROR tags
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="message">Message.</param>
        public static void ProcessLog(ProcessPrint type, string message)
        {
            var initColor = Console.ForegroundColor;
            switch (type)
            {
                case ProcessPrint.info:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("INFO");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]     - " + message);
                    break;
                case ProcessPrint.warn:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARN");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]     - " + message);
                    break;
                case ProcessPrint.debug:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("DEBUG");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]    - " + message);
                    break;
                case ProcessPrint.input:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("INPUT");
                    Console.ForegroundColor = initColor;
                    Console.Write("]    - " + message + ": ");
                    break;
                case ProcessPrint.question:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("QUESTION");
                    Console.ForegroundColor = initColor;
                    Console.Write("] - " + message + ": ");
                    break;
                case ProcessPrint.critical:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("CRITICAL");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("] - " + message);
                    break;
                case ProcessPrint.error:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]    - " + message);
                    break;
                case ProcessPrint.spy:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("SPY");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]      - " + message);
                    break;
            }
        }

        /// <summary>
        /// Generates a pseudo random number between 100000 and 999999
        /// </summary>
        /// <returns>The seed.</returns>
        static int GenerateSeed()
        {
            var rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        #endregion

    }
}
