using System;
using System.Collections.Generic;
using System.Linq;
using static TrustAgent.Helpers;
using static TrustAgent.StandardPrints;

namespace TrustAgent
{
    public static class ActionProcessor
    {

        public static class Keys {

            public enum Action {
                Add, Del, Import
            }

            static List<(Action, string, string)> pendingActions = new List<(Action, string, string)>();
            public static int Pending { get { return pendingActions.Count; } }

            /// <summary>
            /// Prepares an action to be performed, this implementation is for the add and del actions.
            /// </summary>
            /// <returns><c>true</c>, if action was performed, <c>false</c> otherwise.</returns>
            /// <param name="action">Action to perform.</param>
            /// <param name="entity">Entity name.</param>
            /// <param name="hexKey">Key in hex (not need to del).</param>
            public static bool AddAction(Action action, string entity, string hexKey) {
                (Action, string, string) operation = (action, entity, hexKey);
                if (action == Action.Add) {
                    if (ValidHex(hexKey)) {
                        if (hexKey.FromHexToByteArray().Length != 32) {
                            ProcessLog(ProcessPrint.Error, "Invalid key size!", true);
                            return false;
                        }
                    } else {
                        ProcessLog(ProcessPrint.Error, "Key is not an hexadecimal key!", true);
                        return false;
                    }
                    if (pendingActions.Any(m => m.Item1 == Action.Add && m.Item2 == entity && m.Item3 == hexKey))
                    {
                        ProcessLog(ProcessPrint.Warn, "This action is already pending!");
                        ProcessLog(ProcessPrint.Warn, "This action was ignored", true);
                        return false;
                    }
                    if (pendingActions.Any(m => m.Item1 == Action.Add && m.Item2 == entity))
                    {
                        ProcessLog(ProcessPrint.Warn, "There is an action pending with different key!");
                        ProcessLog(ProcessPrint.Question, "Do you want to replace the previous action (y/n)");
                        if (Console.ReadLine().ToBool())
                        {
                            pendingActions.Remove(pendingActions.First(m => m.Item1 == Action.Add && m.Item2 == entity));
                            pendingActions.Add(operation);
                            return true;
                        }
                        ProcessLog(ProcessPrint.Warn, "This action was ignored", true);
                        return false;
                    }
                    pendingActions.Add(operation);
                    return true;
                }
                if (action == Action.Del) {
                    if (pendingActions.Any(m => m.Item1 == Action.Del && m.Item2 == entity)) {
                        ProcessLog(ProcessPrint.Warn, "This action is already pending!");
                        ProcessLog(ProcessPrint.Warn, "This action was ignored!", true);
                        return false;
                    }
                    pendingActions.Add(operation);
                    return true;
                }
                pendingActions.Add(operation);
                return true;
            }

            /// <summary>
            /// Prepares an action to be performed, this implementation is for the import action.
            /// </summary>
            /// <returns><c>true</c>, if action was performed, <c>false</c> otherwise.</returns>
            /// <param name="file">File path.</param>
            public static bool AddAction(string file)
            {
                if (System.IO.File.Exists(file))
                {
                    (Action, string, string) operation = (Action.Import, file, "");
                    pendingActions.Add(operation);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Saves the pending changes (import, del and add)
            /// </summary>
            public static void Save() {
                int warn = 0, err = 0;
                foreach ((Action, string, string) operation in pendingActions.Where(m => m.Item1 == Action.Del)) {
                    Database.DelEntityError tryDel = Program.database.DelEntity(operation.Item2);
                    switch (tryDel) {
                        case Database.DelEntityError.NotFound:
                            ProcessLog(ProcessPrint.Error, String.Format("The entity {0} was not found", operation.Item2));
                            err++;
                            break;
                    }
                }
                foreach ((Action, string, string) operation in pendingActions.Where(m => m.Item1 == Action.Add)) {
                    Database.AddEntityError tryAdd = Program.database.AddEntity(operation.Item2, operation.Item3);
                    switch (tryAdd) {
                        case Database.AddEntityError.Exists:
                            ProcessLog(ProcessPrint.Warn, String.Format("The entity {0} already exists", operation.Item2));
                            warn++;
                            break;
                        case Database.AddEntityError.InvalidKey:
                            ProcessLog(ProcessPrint.Error, String.Format("The key for the entity {0} is invalid!", operation.Item2));
                            err++;
                            break;
                    }
                }
                pendingActions = new List<(Action, string, string)>();
                ProcessLog(ProcessPrint.Info, "All changes saved!", true);
                if (warn != 0 || err != 0)
                    ProcessLog(ProcessPrint.Warn, String.Format("There were {0} warnings and {1} errors", warn, err), true);
            }

            /// <summary>
            /// Discards all pending changes
            /// </summary>
            public static void Discard() {
                pendingActions = new List<(Action, string, string)>();
                ProcessLog(ProcessPrint.Info, "All changes were discarded", true);
            }

            /// <summary>
            /// Prints all pending changes
            /// </summary>
            public static void PrintChanges()
            {
                Console.WriteLine();
                ProcessLog(ProcessPrint.Info, pendingActions.Count.ToString() + " changes pending", true);
                if (pendingActions.Count > 0)
                {
                    List<(string, string, string)> cmds = new List<(string, string, string)>();
                    foreach ((Action, string, string) change in pendingActions.Where(m => m.Item1 != Action.Import))
                    {
                        switch (change.Item1)
                        {
                            case Action.Add:
                                cmds.Add(("Add", change.Item2, change.Item3));
                                break;
                            case Action.Del:
                                cmds.Add(("Delete", change.Item2, change.Item3));
                                break;
                        }
                    }
                    Console.WriteLine(cmds.ToStringTable(
                        new[] { "Action", "Entity", "Key" },
                        a => a.Item1, a => a.Item2, a => a.Item3));

                    Console.WriteLine();

                    List<(string, string)> cmdsI = new List<(string, string)>();
                    foreach ((Action, string, string) cmd in pendingActions.Where(m => m.Item1 == Action.Import))
                        cmdsI.Add(("Data import", cmd.Item2));

                    Console.WriteLine(cmdsI.ToStringTable(
                        new[] { "Action", "File" },
                        a => a.Item1, a => a.Item2
                    ));
                }

            }

        }

    }
}
