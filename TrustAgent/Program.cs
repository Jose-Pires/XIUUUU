using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TrustAgent
{
    class Program
    {

        enum ProcessPrint {
            info,
            warn,
            debug,
            error,
            question,
            input,
            critical
        }

        static bool enableDebug = true;
        static Thread thread;
        static string preCommand = "TrustAgent () > ";
        static string computerName;
        static int seed1 = 100000;
        static int seed2 = 100000;
        static int port = 12345;

        static void Main(string[] args)
        {
            Console.Clear();
            ProcessLog(ProcessPrint.info, "TrustAgent is initializing...");


            if (args.Length > 0)
            {
                if (arrayContains(args, "-nodebug"))
                {
                    ProcessLog(ProcessPrint.info, "Debug is disabled!");
                    enableDebug = false;
                }

                CheckSeed1(args);

                CheckSeed2(args);

                if (arrayContains(args, "-port"))
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

        static bool arrayContains(object[] arr, object obj) {
            foreach (object _obj in arr) {
                if (_obj.Equals(obj))
                    return true;
            }
            return false;
        }

        static void CheckSeed1(string[] args) {
            // Request the seed1 that will generate the key to encrypt the data
            // It can be passed by argument using -seed1 [seed] 
            // If seed1 is not passed on the arguments it's possible to generate a new seed or enter a seed
            if (arrayContains(args, "-seed1"))
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

        static void CheckSeed2(string[] args)
        {
            // Request the seed1 that will generate the key to encrypt the data
            // It can be passed by argument using -seed1 [seed] 
            // If seed1 is not passed on the arguments it's possible to generate a new seed or enter a seed
            if (arrayContains(args, "-seed2"))
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

        static void RequestSeed(out int seed) {
            ProcessLog(ProcessPrint.warn, "If the entered seed does not match the previous used seed the stored data will be overwritten!");
            ProcessLog(ProcessPrint.input, "Please enter a value between 100000 and 999999");
            bool success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            while (!success) {
                ProcessLog(ProcessPrint.input, "Please enter a value between 100000 and 999999");
                success = int.TryParse(Console.ReadLine(), out seed) && (seed >= 100000 && seed <= 999999);
            }
        }

        static int GenerateSeed() {
            var rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        static void App()
        {
            List<IPAddress> ips = GetLocalIPAddress();
            computerName = Dns.GetHostName().Replace(".local", "").Replace(".lan", "");
            preCommand = "TrustAgent (" + computerName + ") > ";
            ProcessLog(ProcessPrint.debug, "Computer name: " + computerName);
            if (enableDebug)
            {
                ProcessLog(ProcessPrint.debug, "Fetching available IP's");
                if (ips.Count <= 0)
                {
                    ProcessLog(ProcessPrint.critical, "There are no available networks!");
                    Environment.Exit(0);
                }
                foreach (IPAddress ip in ips)
                {
                    ProcessLog(ProcessPrint.debug, ip.ToString());
                }
            }
            Console.WriteLine("");
            ProcessLog(ProcessPrint.info, "Type help at anytime to show the help menu\n");
            thread = new Thread(ReadCommands);
            thread.Start();
        }

        static void ReadCommands()
        {
            string cmd = "";
            while (cmd != "exit")
            {
                Console.Write(preCommand); cmd = Console.ReadLine();
                if (cmd.Contains("keys"))
                {
                    ReadKeysCommands();
                }
                else if (cmd == "clear")
                    Console.Clear();
                else if (cmd == "help")
                    PrintMainHelp();
                else if (cmd == "exit") {

                } else
                    ProcessLog(ProcessPrint.error, "Command not found!");
            }
        }

        static void ReadKeysCommands() {
            string cmd = "";
            preCommand = "TrustAgent (" + computerName + "): keys > ";
            while (cmd != "back")
            {
                Console.Write(preCommand); cmd = Console.ReadLine();
                if (cmd.Contains("keys")) {

                }
                else if (cmd == "clear")
                    Console.Clear();
                else if (cmd == "help")
                    PrintKeyshelp();
                else if (cmd == "back") {

                } else
                    ProcessLog(ProcessPrint.error, "Command not found!");
            }
            preCommand = "TrustAgent (" + computerName + ") > ";
        }

        static void PrintMainHelp() {
            Console.WriteLine("\n\t\t\t======== HELP ========");
            Console.WriteLine("keys                                   - Enters the submenu to manage the keys");
            Console.WriteLine("    keys list                          - Prints the clients on the TrustAgent database");
            Console.WriteLine("    keys add [username] [key]          - Adds a new client to the TrustAgent database");
            Console.WriteLine("    keys import [file path]            - Import a csv file with the clients and clients keys");
            Console.WriteLine("    keys del [username]                - Deletes an client from the TrustAgent database\n");
            Console.WriteLine("system                                 - Enters the admin menu");
            Console.WriteLine(""); Console.WriteLine("");
            Console.WriteLine("clear                                  - Clears the console output");
            Console.WriteLine("help                                   - Shows the help of the current selected menu");
            Console.WriteLine("exit                                   - Terminates the TrustAgent server\n");
        }

        static void PrintKeyshelp() {
            Console.WriteLine("\n\t\t\t======== KEYS HELP ========");
            Console.WriteLine("list                          - Prints the clients on the TrustAgent database");
            Console.WriteLine("add [username] [key]          - Adds a new client to the TrustAgent database");
            Console.WriteLine("import [file path] [options]  - Import a csv file with the clients and clients keys");
            Console.WriteLine("del [username]                - Deletes an client from the TrustAgent database");
            Console.WriteLine(""); Console.WriteLine("");
            Console.WriteLine("clear                                  - Clears the console output");
            Console.WriteLine("help                                   - Shows the help of the current selected menu");
            Console.WriteLine("back                   - Navigates to the previous menu\n");
        }

        static List<IPAddress> GetLocalIPAddress()
        {
            var _host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ips = new List<IPAddress>();
            foreach (IPAddress ip in _host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ips.Add(ip);
            }
            return ips;
        }

        static void ProcessLog(ProcessPrint type, string message) {
            var initColor = Console.ForegroundColor;
            switch (type) {
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
                    Console.Write("CRITYCAL");
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
            }
        }

    }
}
