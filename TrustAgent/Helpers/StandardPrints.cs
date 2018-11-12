using System;
using System.Collections.Generic;

namespace TrustAgent
{
    public static class StandardPrints
    {

        public enum ProcessPrint
        {
            Info,
            Warn,
            Debug,
            Error,
            Question,
            Input,
            Critical
        }

        /// <summary>
        /// Helps processing logs shown on the console by formatting the spaces,
        /// change the text colors of the INFO, WARN, DEBUG, INPUT, QUESTION, CRYTICAL
        /// and ERROR tags
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="message">Message.</param>
        /// <param name="addBlankLine">Adds a blank line after printing the message.</param>
        public static void ProcessLog(ProcessPrint type, string message, bool addBlankLine = false)
        {
            var initColor = Console.ForegroundColor;
            switch (type)
            {
                case ProcessPrint.Info:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("INFO");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]     - " + message);
                    break;
                case ProcessPrint.Warn:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("WARN");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]     - " + message);
                    break;
                case ProcessPrint.Debug:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write("DEBUG");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]    - " + message);
                    break;
                case ProcessPrint.Input:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("INPUT");
                    Console.ForegroundColor = initColor;
                    Console.Write("]    - " + message + ": ");
                    break;
                case ProcessPrint.Question:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("QUESTION");
                    Console.ForegroundColor = initColor;
                    Console.Write("] - " + message + ": ");
                    break;
                case ProcessPrint.Critical:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write("CRITICAL");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("] - " + message);
                    break;
                case ProcessPrint.Error:
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("ERROR");
                    Console.ForegroundColor = initColor;
                    Console.WriteLine("]    - " + message);
                    break;
            }
            if (addBlankLine)
                Console.WriteLine();
        }
    
        public static class HelpPrints {

            /// <summary>
            /// Prints the main menu help
            /// </summary>
            public  static void PrintMainHelp()
            {
                Console.WriteLine("");
                IEnumerable<Tuple<string, string>> cmds =
                    new[]
                    {
                  Tuple.Create("keys", "Enters the submenu to manage the keys"),
                  Tuple.Create("server", "Enters the server informations"),
                  Tuple.Create("system", "Enters the TrustAgent Configurations"),
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
            public static void PrintKeyshelp()
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
                    a => a.Item1, a => a.Item2));
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

            /// <summary>
            /// Prints the server menu help
            /// </summary>
            public static void PrintServerHelp()
            {
                Console.WriteLine("");
                IEnumerable<Tuple<string, string>> cmds =
                    new[]
                    {
                  Tuple.Create("list", "Lists the connected entities"),
                  Tuple.Create("disconnect [entity name]", "Disconnects the specified entity"),
                  Tuple.Create("",""),
                  Tuple.Create("server info","Lists all server info"),
                  Tuple.Create("",""),
                  Tuple.Create("clear", "Clears the output"),
                  Tuple.Create("help", "Shows the help of the current selected menu"),
                  Tuple.Create("back", "Navigates to the previous menu"),
                  Tuple.Create("exit", "Terminates the TrustAgent server")
                    };

                Console.WriteLine(cmds.ToStringTable(
                    new[] { "Command", "Description" },
                    a => a.Item1, a => a.Item2));
            }

        }
    }
}
