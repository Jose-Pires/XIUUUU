using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using static TrustAgent.StandardPrints;

namespace TrustAgent
{
    public class MenuEventArgs: EventArgs {
        public Dictionary<object, object> Arguments { get; set; }
    }

    public class MenuHandler
    {
        public event CommandHandler CommandReceived;
        public delegate void CommandHandler(Menu menu, object command, MenuEventArgs e);

        public string commandPrefix = "TrustAgent ()";
        string computerName = "";

        public enum Menu {
            Main,
            Keys,
            Server,
            System
        }

        public enum MainCommsands {
            Keys,
            Server,
            System,
            Clear,
            Help,
            Exit,
            Invalid
        }

        public enum KeysCommands {
            List,
            Add,
            Del,
            Import,
            Save,
            Discard,
            ListChanges,
            Clear,
            Help,
            Back,
            Exit,
            Invalid
        }
        public enum KeysArgs {
            Entity,
            Key,
            ShowKey
        }

        public enum ServerCommands {
            List,
            Disconnect,
            ServerInfo,
            Clear,
            Help,
            Back,
            Exit,
            Invalid
        }
        public enum ServerArgs {
            Entity
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TrustAgent.MenuHandler"/> class.
        /// </summary>
        public MenuHandler() {
            computerName = Dns.GetHostName().Replace(".local", "").Replace(".lan", "");
            commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
            Thread thread = new Thread(ListenMain);
            thread.Start();
        }

        /// <summary>
        /// Listens for commands on the main menu.
        /// </summary>
        void ListenMain() {
            MainCommsands command = MainCommsands.Invalid;
            while (command != MainCommsands.Exit) {
                Console.Write("\r" + commandPrefix); 
                command =(MainCommsands)ToCommand(Menu.Main, Console.ReadLine());
                switch (command) {
                    case MainCommsands.Keys:
                        commandPrefix = String.Format("TrustAgent ({0}): Keys > ", computerName);
                        bool kListenResult = ListenKeys();
                        commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
                        if (kListenResult)
                        {
                            Program.sv.Shutdown();
                            Program.db.WriteToFile();
                            command = MainCommsands.Exit;
                        }
                        break;
                    case MainCommsands.Server:
                        commandPrefix = String.Format("TrustAgent ({0}): Server > ", computerName);
                        bool sListenResult = ListenServer();
                        commandPrefix = String.Format("TrustAgent ({0}) > ", computerName);
                        if (sListenResult)
                        {
                            Program.sv.Shutdown();
                            Program.db.WriteToFile();
                            command = MainCommsands.Exit;
                        }
                        break;
                    case MainCommsands.System:
                        //TODO: initialize listening to the system menu
                        break;
                    case MainCommsands.Clear:
                        Console.Clear();
                        break;
                    case MainCommsands.Help:
                        HelpPrints.PrintMainHelp();
                        break;
                    case MainCommsands.Exit:
                        Program.sv.Shutdown();
                        Program.db.WriteToFile();
                        break;
                    case MainCommsands.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                }
            }
        } 

        /// <summary>
        /// Listens for commands on the keys menu.
        /// </summary>
        /// <returns><c>true</c>, if the last command entered is exit, <c>false</c> otherwise.</returns>
        bool ListenKeys() {
            KeysCommands command = KeysCommands.Invalid;
            while (command != KeysCommands.Back) {
                Console.Write("\r" + commandPrefix);
                string cmd = Console.ReadLine();
                command = (KeysCommands)ToCommand(Menu.Keys, cmd);
                switch (command) {
                    case KeysCommands.Add:
                        CommandReceived(Menu.Keys, KeysCommands.Add, ProcessCommandArgs(Menu.Keys, KeysCommands.Add, cmd));
                        break;
                    case KeysCommands.Back:
                        if (ActionProcessor.Keys.Pending > 0)
                        {
                            ProcessLog(ProcessPrint.Warn, "There are pending changes to apply!");
                            ProcessLog(ProcessPrint.Question, "Do you want to discard the changes (y/n)");
                            bool a = Console.ReadLine().ToBool();
                            if (!a)
                                ActionProcessor.Keys.Save();
                            else
                                ActionProcessor.Keys.Discard();
                        }
                        break;
                    case KeysCommands.Clear:
                        Console.Clear();
                        break;
                    case KeysCommands.Del:
                        CommandReceived(Menu.Keys, KeysCommands.Del, ProcessCommandArgs(Menu.Keys, KeysCommands.Del, cmd));
                        break;
                    case KeysCommands.Discard:
                        ActionProcessor.Keys.Discard();
                        break;
                    case KeysCommands.Exit:
                        return true;
                    case KeysCommands.Help:
                        HelpPrints.PrintKeyshelp();
                        break;
                    case KeysCommands.Import:
                        //TODO: Implement import functionality
                        /* NOTE: OLD CODE
                         * bool ow = false || cmd.Contains("-overwrite");
                         * cmd = cmd.Replace("import ", "").Replace(" -overwrite", "");
                         * bool actionResult = ActionProcessor.Keys.PerformAction(cmd);
                         * if (actionResult)
                         *     ProcessLog(ProcessPrint.info, ActionProcessor.Keys.Pending + " changes pending");
                         * else
                         *     ProcessLog(ProcessPrint.error, "File not found");
                         */
                        break;
                    case KeysCommands.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                    case KeysCommands.List:
                        CommandReceived(Menu.Keys, KeysCommands.List, ProcessCommandArgs(Menu.Keys, KeysCommands.List, cmd));
                        break;
                    case KeysCommands.ListChanges:
                        CommandReceived(Menu.Keys, KeysCommands.ListChanges, null);
                        break;
                    case KeysCommands.Save:
                        ActionProcessor.Keys.Save();
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// Listens for commands on the server menu.
        /// </summary>
        /// <returns><c>true</c>, if the last command entered is exit, <c>false</c> otherwise.</returns>
        bool ListenServer() {
            ServerCommands command = ServerCommands.Invalid;
            while (command != ServerCommands.Back)
            {
                Console.Write("\r" + commandPrefix);
                string cmd = Console.ReadLine();
                command = (ServerCommands)ToCommand(Menu.Server, cmd);
                switch (command)
                {
                    case ServerCommands.List:
                        CommandReceived(Menu.Server, ServerCommands.List, null);
                        break;
                    case ServerCommands.Back:
                        break;
                    case ServerCommands.Clear:
                        Console.Clear();
                        break;
                    case ServerCommands.Disconnect:
                        CommandReceived(Menu.Server, ServerCommands.Disconnect, ProcessCommandArgs(Menu.Server, ServerCommands.Disconnect, cmd));
                        break;
                    case ServerCommands.Exit:
                        return true;
                    case ServerCommands.Help:
                        HelpPrints.PrintServerHelp();
                        break;
                    case ServerCommands.Invalid:
                        ProcessLog(ProcessPrint.Error, "Invalid command", true);
                        break;
                    case ServerCommands.ServerInfo:
                        CommandReceived(Menu.Server, ServerCommands.ServerInfo, null);
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
        object ToCommand(Menu menu, string input) {
            switch (menu) {
                case Menu.Main:
                    if (input.ToLower().Contains("keys"))
                        return MainCommsands.Keys;
                    if (input.ToLower().Contains("server"))
                        return MainCommsands.Server;
                    if (input.ToLower().Contains("system"))
                        return MainCommsands.System;
                    if (input.ToLower().Contains("clear"))
                        return MainCommsands.Clear;
                    if (input.ToLower().Contains("help"))
                        return MainCommsands.Help;
                    if (input.ToLower().Contains("exit"))
                        return MainCommsands.Exit;
                    return MainCommsands.Invalid;
                case Menu.Keys:
                    if (input.ToLower().Contains("list") && !input.ToLower().Contains("changes"))
                        return KeysCommands.List;
                    if (input.ToLower().Contains("add"))
                        return KeysCommands.Add;
                    if (input.ToLower().Contains("del"))
                        return KeysCommands.Del;
                    if (input.ToLower().Contains("import"))
                        return KeysCommands.Import;
                    if (input.ToLower().Contains("save"))
                        return KeysCommands.Save;
                    if (input.ToLower().Contains("discard"))
                        return KeysCommands.Discard;
                    if (input.ToLower().Contains("list changes"))
                        return KeysCommands.ListChanges;
                    if (input.ToLower().Contains("clear"))
                        return KeysCommands.Clear;
                    if (input.ToLower().Contains("help"))
                        return KeysCommands.Help;
                    if (input.ToLower().Contains("back"))
                        return KeysCommands.Back;
                    if (input.ToLower().Contains("exit"))
                        return KeysCommands.Exit;
                    return KeysCommands.Invalid;
                case Menu.Server:
                    if (input.ToLower().Contains("list"))
                        return ServerCommands.List;
                    if (input.ToLower().Contains("clear"))
                        return ServerCommands.Clear;
                    if (input.ToLower().Contains("help"))
                        return ServerCommands.Help;
                    if (input.ToLower().Contains("back"))
                        return ServerCommands.Back;
                    if (input.ToLower().Contains("exit"))
                        return ServerCommands.Exit;
                    if (input.ToLower().Contains("disconnect"))
                        return ServerCommands.Disconnect;
                    if (input.ToLower().Contains("server info"))
                        return ServerCommands.ServerInfo;
                    return ServerCommands.Invalid;
            }
            return null;
        }

        /// <summary>
        /// Processes the command arguments.
        /// </summary>
        /// <returns>The command arguments.</returns>
        /// <param name="menu">Menu.</param>
        /// <param name="command">Command.</param>
        /// <param name="input">Input.</param>
        MenuEventArgs ProcessCommandArgs(Menu menu, object command, string input) {
            switch (menu) {
                case Menu.Keys:
                    switch ((KeysCommands)command) {
                        case KeysCommands.Add:
                            input = input.Replace("add ", "").Trim();
                            string[] strSplited = input.Split(' ');
                            string key = strSplited[strSplited.Length - 1];
                            Array.Resize(ref strSplited, strSplited.Length - 1);
                            string entityName = string.Join(" ", strSplited);
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArgs.Entity, entityName }, { KeysArgs.Key, key } }
                            };
                        case KeysCommands.Del:
                            input = input.Replace("del ", "").Trim();
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArgs.Entity, input } }
                            };
                        case KeysCommands.List:
                            if (input.Replace("list ", "").Trim().Contains("-k"))
                                return new MenuEventArgs
                                {
                                    Arguments = new Dictionary<object, object> { { KeysArgs.ShowKey, true } }
                                };
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { KeysArgs.ShowKey, false } }
                            };
                    }
                    break;
                case Menu.Server:
                    switch ((ServerCommands)command) {
                        case ServerCommands.Disconnect:
                            input = input.Replace("disconnect ", "").Trim();
                            return new MenuEventArgs
                            {
                                Arguments = new Dictionary<object, object> { { ServerArgs.Entity, input } }
                            };
                    }
                    break;
            }
            return null;
        }

    }
}
