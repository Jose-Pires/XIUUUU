/*
 * TrustAgent.ClientOperations.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * An enum simulator that can be converted to a string
 * 
 * Requires initialization: NO
 * Contains:
 *     Classes: 2 Public
 *     Class "ServerOperations":
 *         Getters/Setters: 1 Public (work as variable)
 *         Methods:
 *             Static: 4 Public
 *     Class "ServerMessages":
 *         Getters/Setters: 1 Public (work as variable)
 *         Methods:
 *             Static: 3 Public
 * 
 */

namespace TrustAgent
{
    public class ServerOperations
    {
        ServerOperations(string value) { Value = value; }

        public string Value { get; set; }

        public static ServerOperations Connected { get { return new ServerOperations("connected"); } }
        public static ServerOperations ServerShutdown { get { return new ServerOperations("sv_shutdown"); } }
        public static ServerOperations ConnectionRefused { get { return new ServerOperations("connection_refused"); } }
        public static ServerOperations KickEntity { get { return new ServerOperations("connection_terminated"); } }
        public static ServerOperations ResponseSuccessEntities { get { return new ServerOperations("response_success_entities"); } }
        public static ServerOperations InvalidHMAC { get { return new ServerOperations("hmac_verification_failed"); } }
        public static ServerOperations EntityNoLongerAvailable { get { return new ServerOperations("entity_no_longer_available"); } }
        public static ServerOperations InvalidComand { get { return new ServerOperations("invalid_comand"); } }
    }

    public class ServerMessages {
        ServerMessages(string value) { Value = value; }

        public string Value { get; set; }

        public static ServerMessages InvalidKey { get { return new ServerMessages("invalid_key"); } }
        public static ServerMessages EntityNotFound { get { return new ServerMessages("entity_not_found"); } }
        public static ServerMessages EntityAlreadyConnected { get { return new ServerMessages("entity_already_connected"); } }
    }

}
