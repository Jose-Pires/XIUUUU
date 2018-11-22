/*
 * TrustAgent.ServerCommand.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Model for a server comand
 * 
 * Requires initialization: YES
 * 
 */

using System;

namespace TrustAgent
{
    public class ServerCommand
    {
        public string Timestamp { get; set; }
        public string Command { get; set; }
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }

        public ServerCommand() {
            Timestamp = Helpers.GetTimestamp(DateTime.Now);
        }
    }
}
