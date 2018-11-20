/*
 * TrustAgent.ServerCommand.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Model for a server comand
 * 
 * Requires initialization: NO
 * Contains:
 *     Getters/Setters: 5 Public (work as variables)
 * 
 */

namespace TrustAgent
{
    public class ServerCommand
    {
        public string Command { get; set; }
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }
    }
}
