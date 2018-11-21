/*
 * TrustAgent.MenuManager.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Operates the menus for the program
 * 
 * Requires initialization: YES
 * Contains:
 *     Class Level Variables: 1 Private (Read Only), 1 Public
 *     Inner Classes: 1 Public
 *     Enums: 6 Private
 *     Methods:
 *         Non Static: 5 Private
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using static TrustAgent.ActionProcessor;
using static TrustAgent.StandardPrints;

namespace TrustAgent
{
    public class MenuManager
    {

        public class MenuEventArgs : EventArgs
        {
            public Dictionary<object, object> Arguments { get; set; }
        }

        public string commandPrefix;
        readonly string computerName;

        public MenuManager()
        {
            computerName = Dns.GetHostName().Replace(".local", "").Replace(".lan", "");
            commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
            Thread thread = new Thread(MainMenu);
            thread.Start();
        }

        #region "Enums"

        enum Menu
        {
            Main, Keys, Server, System
        }

        enum MainCommand
        {
            Keys, Server, System, Clear, Help, Exit, Invalid
        }

        enum KeysCommand
        {
            List, Add, Del, Import, Save, Discard, ListChanges, Clear, Help, Back, Exit, Invalid
        }

        enum KeysArg
        {
            Entity, Key, ShowKey
        }

        enum ServerCommand
        {
            List, Disconnect, ServerInfo, Clear, Help, Back, Exit, Invalid
        }

        enum ServerArg
        {
            Entity
        }

        #endregion

        /// <summary>
        /// Manages the main menu
        /// </summary>
        void MainMenu() {
            MainCommand command = MainCommand.Invalid;
            bool terminate = false;
            while (command != MainCommand.Exit) {
                Console.Write("\r" + commandPrefix);
                command = (MainCommand)ToCommand(Menu.Main, Console.ReadLine());
                switch (command) {
                    case MainCommand.Keys:
                        commandPrefix = String.Format("TrustAgent ({0}): Keys > ", computerName);
                        terminate = KeysMenu();
                        commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
                        if (terminate)
                            command = MainCommand.Exit;
                        break;
                    case MainCommand.Server:
                        commandPrefix = String.Format("TrustAgent ({0}): Server > ", computerName);
                        terminate = ServerMenu();
                        commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
                        if (terminate)
                            command = MainCommand.Exit;
                        break;
                    case MainCommand.System:
                        Console.WriteLine("");
                        IEnumerable<Tuple<string, string>> cmds =
                            new[]
                            {
                              Tuple.Create("SEED 1", Program.dbSeed1.ToString()),
                              Tuple.Create("SEED 2", Program.dbSeed2.ToString()),
                              Tuple.Create("",""),
                              Tuple.Create("CIPHER","AES-256"),
                              Tuple.Create("HMAC", "SHA-256"),
                              Tuple.Create("",""),
                              Tuple.Create("LOCAL IP", Program.server.ListeningIP.ToString()),
                              Tuple.Create("LOCAL PORT", Program.server.ListeningPort.ToString()),
                              Tuple.Create("",""),
                              Tuple.Create("DEBUG",Program.enableDebug.ToString()),
                              Tuple.Create("SPY",Program.enableSpy.ToString())
                            };
                        Console.WriteLine(cmds.ToStringTable(
                            new[] { "PARAMETER", "VALUE" },
                            a => a.Item1, a => a.Item2));
                        break;
                    case MainCommand.Clear: Console.Clear(); break;
                    case MainCommand.Help: HelpPrints.PrintMainHelp(); break;
                    case MainCommand.Exit: break;
                    case MainCommand.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                }
            }
            Program.server.Shutdown();
            Program.database.WriteToFile();
        }

        /// <summary>
        /// Manages the keys menu
        /// </summary>
        /// <returns><c>true</c>, if the exit command was issued, <c>false</c> otherwise.</returns>
        bool KeysMenu() {
            KeysCommand command = KeysCommand.Invalid;
            while (command != KeysCommand.Back) {
                Console.Write("\r" + commandPrefix);
                string cmd = Console.ReadLine();
                command = (KeysCommand)ToCommand(Menu.Keys, cmd);
                switch (command) {
                    case KeysCommand.Add:
                        MenuEventArgs addArgs = ProcessCommandArgs(Menu.Keys, KeysCommand.Add, cmd);
                        string addEntity = (string)addArgs.Arguments.First(m => m.Key.Equals(KeysArg.Entity)).Value;
                        string addKey = (string)addArgs.Arguments.First(m => m.Key.Equals(KeysArg.Key)).Value;
                        Keys.AddAction(Keys.Action.Add, addEntity, addKey);
                        break;
                    case KeysCommand.Back:
                        if (Keys.Pending > 0) {
                            ProcessLog(ProcessPrint.Warn, "There are pending changes to apply!");
                            ProcessLog(ProcessPrint.Question, "Do you want to discard the changes (y/n)");
                            if (Console.ReadLine().ToBool())
                                Keys.Discard();
                            else
                                Keys.Save();
                        }
                        break;
                    case KeysCommand.Clear: Console.Clear(); break;
                    case KeysCommand.Del:
                        MenuEventArgs delArgs = ProcessCommandArgs(Menu.Keys, KeysCommand.Del, cmd);
                        string delEntity = (string)delArgs.Arguments.First(m => m.Key.Equals(KeysArg.Entity)).Value;
                        Keys.AddAction(Keys.Action.Add, delEntity, "");
                        break;
                    case KeysCommand.Discard: Keys.Discard(); break;
                    case KeysCommand.Exit: return true;
                    case KeysCommand.Help: HelpPrints.PrintKeyshelp(); break;
                    case KeysCommand.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                    case KeysCommand.List:
                        MenuEventArgs listArgs = ProcessCommandArgs(Menu.Keys, KeysCommand.List, cmd);
                        bool showKey = (bool)listArgs.Arguments.First(m => m.Key.Equals(KeysArg.ShowKey)).Value;
                        Program.database.PrintEntities(showKey);
                        break;
                    case KeysCommand.ListChanges: Keys.PrintChanges(); break;
                    case KeysCommand.Save: Keys.Save(); break;
                }
            }
            return false;
        }

        /// <summary>
        /// Manages the server menu
        /// </summary>
        /// <returns><c>true</c>, if the exit command was issued, <c>false</c> otherwise.</returns>
        bool ServerMenu() {
            ServerCommand command = ServerCommand.Invalid;
            while (command != ServerCommand.Back) {
                Console.Write("\r" + commandPrefix);
                string cmd = Console.ReadLine();
                command = (ServerCommand)ToCommand(Menu.Server, cmd);
                switch (command) {
                    case ServerCommand.List:
                        Console.WriteLine("");
                        List<(string, string)> cmds = new List<(string, string)>();
                        foreach (string key in Program.server.clientHandlers.Keys)
                        {
                            ClientHandler handler = (ClientHandler)Program.server.clientHandlers[key];
                            IPAddress ip = ((IPEndPoint)handler.Socket.Client.RemoteEndPoint).Address;
                            cmds.Add((key, ip.ToString()));
                        }
                        Console.WriteLine(cmds.ToStringTable(
                            new[] { "Entity", "IP" },
                            a => a.Item1, a => a.Item2));
                        break;
                    case ServerCommand.Back: break;
                    case ServerCommand.Clear: Console.Clear(); break;
                    case ServerCommand.Disconnect:
                        MenuEventArgs listArgs = ProcessCommandArgs(Menu.Server, ServerCommand.Disconnect, cmd);
                        string entity = (string)listArgs.Arguments.First(m => m.Key.Equals(ServerArg.Entity)).Value;
                        Program.server.DisconnectClient(entity);
                        ProcessLog(ProcessPrint.Info, "Entity " + entity + " was kicked", true);
                        break;
                    case ServerCommand.Exit: return true;
                    case ServerCommand.Help: HelpPrints.PrintServerHelp(); break;
                    case ServerCommand.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                    case ServerCommand.ServerInfo:
                        ProcessLog(ProcessPrint.Info, string.Format("Server listening at {0}:{1}", Program.server.ListeningIP, Program.server.ListeningPort));
                        ProcessLog(ProcessPrint.Info, string.Format("There are {0} connected clients", Program.server.clientHandlers.Count));
                        Console.WriteLine("");
                        List<(string, string)> cmds1 = new List<(string, string)>();
                        foreach (string key in Program.server.clientHandlers.Keys)
                        {
                            ClientHandler handler = (ClientHandler)Program.server.clientHandlers[key];
                            IPAddress ip = ((IPEndPoint)handler.Socket.Client.RemoteEndPoint).Address;
                            cmds1.Add((key, ip.ToString()));
                        }
                        Console.WriteLine(cmds1.ToStringTable(
                            new[] { "Entity", "IP" },
                            a => a.Item1, a => a.Item2));
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts to command
        /// </summary>
        /// <returns>The command.</returns>
        /// <param name="menu">Current Menu.</param>
        /// <param name="input">Input.</param>
        object ToCommand(Menu menu, string input)
        {
            switch (menu)
            {
                case Menu.Main:
                    if (input.ToLower().Contains("keys"))
                        return MainCommand.Keys;
                    if (input.ToLower().Contains("server"))
                        return MainCommand.Server;
                    if (input.ToLower().Contains("system"))
                        return MainCommand.System;
                    if (input.ToLower().Contains("clear"))
                        return MainCommand.Clear;
                    if (input.ToLower().Contains("help"))
                        return MainCommand.Help;
                    if (input.ToLower().Contains("exit"))
                        return MainCommand.Exit;
                    return MainCommand.Invalid;
                case Menu.Keys:
                    if (input.ToLower().Contains("list") && !input.ToLower().Contains("changes"))
                        return KeysCommand.List;
                    if (input.ToLower().Contains("add"))
                        return KeysCommand.Add;
                    if (input.ToLower().Contains("del"))
                        return KeysCommand.Del;
                    if (input.ToLower().Contains("import"))
                        return KeysCommand.Import;
                    if (input.ToLower().Contains("save"))
                        return KeysCommand.Save;
                    if (input.ToLower().Contains("discard"))
                        return KeysCommand.Discard;
                    if (input.ToLower().Contains("list changes"))
                        return KeysCommand.ListChanges;
                    if (input.ToLower().Contains("clear"))
                        return KeysCommand.Clear;
                    if (input.ToLower().Contains("help"))
                        return KeysCommand.Help;
                    if (input.ToLower().Contains("back"))
                        return KeysCommand.Back;
                    if (input.ToLower().Contains("exit"))
                        return KeysCommand.Exit;
                    return KeysCommand.Invalid;
                case Menu.Server:
                    if (input.ToLower().Contains("list"))
                        return ServerCommand.List;
                    if (input.ToLower().Contains("clear"))
                        return ServerCommand.Clear;
                    if (input.ToLower().Contains("help"))
                        return ServerCommand.Help;
                    if (input.ToLower().Contains("back"))
                        return ServerCommand.Back;
                    if (input.ToLower().Contains("exit"))
                        return ServerCommand.Exit;
                    if (input.ToLower().Contains("disconnect"))
                        return ServerCommand.Disconnect;
                    if (input.ToLower().Contains("server info"))
                        return ServerCommand.ServerInfo;
                    return ServerCommand.Invalid;
            }
            return null;
        }

        /// <summary>
        /// Converts to command
        /// </summary>
        /// <returns>The command.</returns>
        /// <param name="menu">Current Menu.</param>
        /// <param name="input">Input.</param>
        MenuEventArgs ProcessCommandArgs(Menu menu, object command, string input)
        {
            switch (menu)
            {
                case Menu.Keys:
                    switch ((KeysCommand)command)
                    {
                        case KeysCommand.Add:
                            input = input.Replace("add ", "").Trim();
                            string[] strSplited = input.Split(' ');
                            string key = strSplited[strSplited.Length - 1];
                            Array.Resize(ref strSplited, strSplited.Length - 1);
                            string entityName = string.Join(" ", strSplited);
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArg.Entity, entityName }, { KeysArg.Key, key } }
                            };
                        case KeysCommand.Del:
                            input = input.Replace("del ", "").Trim();
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArg.Entity, input } }
                            };
                        case KeysCommand.List:
                            if (input.Replace("list ", "").Trim().Contains("-k"))
                                return new MenuEventArgs
                                {
                                    Arguments = new Dictionary<object, object> { { KeysArg.ShowKey, true } }
                                };
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArg.ShowKey, false } }
                            };
                    }
                    break;
                case Menu.Server:
                    switch ((ServerCommand)command)
                    {
                        case ServerCommand.Disconnect:
                            input = input.Replace("disconnect ", "").Trim();
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { ServerArg.Entity, input } }
                            };
                    }
                    break;
            }
            return null;
        }

    }
}
