using System;
namespace TAClientLib
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
