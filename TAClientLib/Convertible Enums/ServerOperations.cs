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
    }

    public class ServerMessages {
        ServerMessages(string value) { Value = value; }

        public string Value { get; set; }

        public static ServerMessages InvalidKey { get { return new ServerMessages("invalid_key"); } }
        public static ServerMessages EntityNotFound { get { return new ServerMessages("entity_not_found"); } }
        public static ServerMessages EntityAlreadyConnected { get { return new ServerMessages("entity_already_connected"); } }
    }

}
