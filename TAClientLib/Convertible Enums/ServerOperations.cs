using System;
namespace TAClientLib
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
        public static ServerOperations InvalidTime { get { return new ServerOperations("invalid_time"); } }
    }

    public class ServerMessages {
        ServerMessages(string value) { Value = value; }

        public string Value { get; set; }

        public static ServerMessages InvalidKey { get { return new ServerMessages("invalid_key"); } }
        public static ServerMessages EntityNotFound { get { return new ServerMessages("entity_not_found"); } }
        public static ServerMessages EntityAlreadyConnected { get { return new ServerMessages("entity_already_connected"); } }
    }

}
