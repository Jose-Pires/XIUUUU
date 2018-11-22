/*
 * TrustAgent.ClientOperations.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * An enum simulator that can be converted to a string
 * 
 * Requires initialization: NO
 * 
 */

namespace TrustAgent
{
    public class ClientOperations
    {

        ClientOperations(string value) { Value = value; }

        public string Value { get; set; }

        public static ClientOperations AttemptConnect { get { return new ClientOperations("try_connect"); } }
        public static ClientOperations RequestConnectedEntities { get { return new ClientOperations("retreive_entities_list"); } }
        public static ClientOperations RequestKeyNegotiation { get { return new ClientOperations("request_key_negotiation"); } }
        public static ClientOperations Disconnect { get { return new ClientOperations("disconnect"); } }

    }
}
