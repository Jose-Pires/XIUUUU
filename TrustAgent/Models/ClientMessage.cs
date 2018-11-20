/*
 * TrustAgent.ClientMessage.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Model for a client message
 * 
 * Requires initialization: NO
 * Contains:
 *     Getters/Setters: 3 Public (work as variables)
 * 
 */

using System;

namespace TrustAgent
{
    public class ClientMessage
    {
        public string Timestamp { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }

        public ClientMessage() {
            Timestamp = Helpers.GetTimestamp(DateTime.Now);
        }

    }
}
