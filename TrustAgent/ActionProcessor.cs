using System;
using System.Collections.Generic;
using System.Linq;
using static TrustAgent.StandardPrints;

namespace TrustAgent
{
    public static class ActionProcessor
    {
        public static class Keys {

            public enum Action {
                add,del,import
            }

            public static int Pending { get { return pendingActions.Count; } }

            static List<(Action, string, string)> pendingActions = new List<(Action, string, string)>();

            /// <summary>
            /// Prepares an action to be performed, this implementation is for the add and del actions.
            /// </summary>
            /// <returns><c>true</c>, if action was performed, <c>false</c> otherwise.</returns>
            /// <param name="action">Action to perform.</param>
            /// <param name="entity">Entity name.</param>
            /// <param name="hexKey">Key in hex (not need to del).</param>
            public static bool PerformAction(Action action, string entity, string hexKey) {
                (Action, string, string) operation = (action, entity, hexKey);
                if (action == Action.add)
                {
                    if (Helpers.ValidHex(hexKey)) {
                        if (Helpers.StringToByteArray(hexKey).Length != 32) {
                            ProcessLog(ProcessPrint.Error, "Invalid key!");
                            Console.WriteLine("");
                            return false;
                        }
                    } else {
                        ProcessLog(ProcessPrint.Error, "Invalid key!");
                        Console.WriteLine("");
                        return false;
                    }
                    if (pendingActions.Any(m => m.Item1 == Action.add && m.Item2 == entity && m.Item3 == hexKey)) {
                        ProcessLog(ProcessPrint.Warn, "There is a duplicate of this action pending!");
                        ProcessLog(ProcessPrint.Warn, "This action was ignored");
                        Console.WriteLine("");
                        return false;
                    }
                    if (pendingActions.Any(m => m.Item1 == Action.add && m.Item2 == entity))
                    {
                        ProcessLog(ProcessPrint.Warn, "There is an action pending with different key!");
                        ProcessLog(ProcessPrint.Question, "Do you want to replace the previous action (y/n)");
                        string a = Console.ReadLine();
                        if (a == "y")
                        {
                            pendingActions.Remove(pendingActions.First(m => m.Item1 == Action.add && m.Item2 == entity));
                            pendingActions.Add(operation);
                            return true;
                        }
                        ProcessLog(ProcessPrint.Warn, "This action was ignored");
                        Console.WriteLine("");
                        return false;
                    }
                    pendingActions.Add(operation);
                    return true;
                }
                if (action == Action.del) {
                    if (pendingActions.Any(m => m.Item1 == Action.del && m.Item2 == entity)) {
                        ProcessLog(ProcessPrint.Warn, "There is a duplicate of this action pending!");
                        ProcessLog(ProcessPrint.Warn, "This action was ignored");
                        Console.WriteLine("");
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
            public static bool PerformAction(string file) {
                if (System.IO.File.Exists(file))
                {
                    (Action, string, string) operation = (Action.import, file, "");
                    pendingActions.Add(operation);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Saves the pending changes (import, del and add)
            /// </summary>
            public static void Save() {
                int Warn = 0, err = 0;
                foreach ((Action, string, string) change in pendingActions) {
                    if (change.Item1 == Action.add) {
                        TADatabase.AddEntityError tryAdd = Program.db.AddEntity(change.Item2, change.Item3);
                        switch (tryAdd) {
                            case TADatabase.AddEntityError.exists:
                                ProcessLog(ProcessPrint.Warn, "The entity " + change.Item2 + " is already on the database!");
                                Warn += 1;
                                break;
                            case TADatabase.AddEntityError.invalidKey:
                                ProcessLog(ProcessPrint.Error, "The key for the entity " + change.Item2 + " is invalid!");
                                err += 1;
                                break;
                        }
                    }
                    if (change.Item1 == Action.del) {
                        TADatabase.DelEntityError tryDel = Program.db.DelEntity(change.Item2);
                        switch (tryDel) {
                            case TADatabase.DelEntityError.notFound:
                                ProcessLog(ProcessPrint.Error, "The user " + change.Item2 + " was not found on the database!");
                                err += 1;
                                break;
                        }
                    }
                }
                pendingActions = new List<(Action, string, string)>();
                ProcessLog(ProcessPrint.Info, "All changes saved");
                if (Warn != 0 || err != 0)
                    ProcessLog(ProcessPrint.Warn, "There were " + Warn + " Warnings and " + err + " Errors");
                Console.WriteLine("");
            }

            /// <summary>
            /// Discards all pending changes
            /// </summary>
            public static void Discard() {
                pendingActions = new List<(Action, string, string)>();
            }

            /// <summary>
            /// Prints all pending changes
            /// </summary>
            public static void PrintChanges() {
                Console.WriteLine();
                ProcessLog(ProcessPrint.Info, pendingActions.Count.ToString() + " changes pending");
                Console.WriteLine("");
                if (pendingActions.Count > 0) {
                    List<(string, string, string)> cmds = new List<(string, string, string)>();
                    foreach ((Action, string, string) change in pendingActions.Where(m => m.Item1 != Action.import))
                    {
                        switch (change.Item1)
                        {
                            case Action.add:
                                cmds.Add(("Add", change.Item2, change.Item3));
                                break;
                            case Action.del:
                                cmds.Add(("Delete", change.Item2, change.Item3));
                                break;
                        }
                    }
                    Console.WriteLine(cmds.ToStringTable(
                        new[] { "Action", "Entity", "Key" },
                        a => a.Item1, a => a.Item2, a => a.Item3));

                    Console.WriteLine();

                    List<(string, string)> cmdsI = new List<(string, string)>();
                    foreach ((Action, string, string) cmd in pendingActions.Where(m => m.Item1 == Action.import))
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
