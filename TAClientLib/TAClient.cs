using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TAClientLib
{
    /// <summary>
    /// TrustAgent Client that will handle all transactions between the client
    /// and the TrustAgent
    /// </summary>
    /// <exception cref="ConnectionFailedException">Thrown when one of the following errors ocurrs
    /// --> HMAC Missmatch     <see cref="HMACFailedException"/>
    /// --> Failed to connect
    /// --> Enity Rejected     <see cref="ClientRejectedException"/>
    /// You can check witch was the error by checking the Message and if it's 
    /// entity rejected check the InnerException
    /// </exception>
    /// <exception cref="ClientRejectedException">Thrown when one of the following errors ocurrs
    /// --> Invalid Key
    /// --> Entity not found
    /// --> Enity Rejected
    /// You can check witch was the error by checking the Message
    /// </exception>
    /// <exception cref="HMACFailedException">Thrown when HMAC validation fails
    /// --> Includes the Received HMAC
    /// --> Includes the Computed HMAC
    /// </exception>
    public class TAClient
    {
        public event ServerCommandHandler Connected;
        public delegate void ServerCommandHandler();
        public event EntitiesListResponseHandler EntityListReceived;
        public delegate void EntitiesListResponseHandler(EntitiesList e);

        internal Client client;
        internal Client spy;
        internal byte[] firstPacket;
        readonly byte[] key;
        string entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TAClientLib.TAClient"/> class.
        /// </summary>
        /// <param name="entity">Entity name.</param>
        /// <param name="key">Entity key in hexadecimal.</param>
        /// <exception cref="InvalidKeyException">Thrown when the key is invalid
        /// either is not hexadecimal or is not 256 bit (check message).</exception>
        public TAClient(string entity, string key)
        {
            if (Helpers.ValidHex(key))
            {
                this.key = Helpers.StringToByteArray(key);
                if (this.key.Length != 32)
                    throw new InvalidKeyException("Key is not 256bit long");
            }
            else
                throw new InvalidKeyException("Non hexadecimal key");

            this.entity = entity;
            client = new Client(this.key);
            client.Connected += Client_Connected;
            client.Rejected += Client_Rejected;
            client.ConnectionFailed += Client_ConnectionFailed;
            client.InvalidHMAC += Client_InvalidHMAC;
            client.EntityListReceived += Client_EntityListReceived;
        }

        /// <summary>
        /// Attempts to connected to the TrustAgent server @ IP and Port
        /// </summary>
        /// <param name="IP">TA IP.</param>
        /// <param name="Port">TA Port.</param>
        public void AttemptConnect(string IP, int Port)
        {
            NetworkPacket payload = new NetworkPacket
            {
                Entity = entity,
                Operation = ClientOperations.AttemptConnect.Value,
                Message = ""
            };
            byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload));
            byte[] packet = BuildPacket(message);
            firstPacket = packet;
            client.Connect(IP, Port, packet);
        }

        public void RequestKey(string entity) {

        }

        /// <summary>
        /// Performs a request to the server to find out witch entities are
        /// connected to the server
        /// The list will be returned on the event <see cref="EntityListReceived"/>
        /// </summary>
        public void RequestConnectedEntities() {
            NetworkPacket payload = new NetworkPacket
            {
                Entity = entity,
                Operation = ClientOperations.RequestConnectedEntities.Value,
                Message = ""
            };
            byte[] message = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(payload));
            byte[] packet = BuildPacket(message);
            client.SendRequest(packet);
        }

        /// <summary>
        /// Builds the packet to be sent to the network. Calculates the HMAC of
        /// the message and size of the HMAC + Message
        /// The final packet is like this
        /// +---------------------------------------------+
        /// | PACKET SIZE | HMAC (SHA256) |    MESSAGE    |
        /// |   4 BYTES   |   32 BYTES    | VARIABLE SIZE |
        /// +---------------------------------------------+
        /// </summary>
        /// <returns>The packet.</returns>
        /// <param name="message">The message part of the packet.</param>
        byte[] BuildPacket(byte[] message)
        {
            byte[] hmac = Helpers.Encode(message, key);
            byte[] size = BitConverter.GetBytes(message.Length + hmac.Length);
            byte[] packet = new byte[message.Length + hmac.Length + 4];
            Array.Copy(size, packet, 4);
            Array.Copy(hmac, 0, packet, 4, hmac.Length);
            Array.Copy(message, 0, packet, 4 + hmac.Length, message.Length);
            return packet;
        }

        void Client_EntityListReceived(EntitiesList e)
        {
            EntityListReceived(e);
        }


        #region "Client Connection Handlers"

        /// <summary>
        /// Handles the connection success
        /// If the TrustAgent says to connect to the spy the connection will be made
        /// The event "Connected" will be called
        /// </summary>
        /// <param name="e">E.</param>
        void Client_Connected(ServerCommandEventArgs e)
        {
            if (e.EnableSpy)
            {
                spy = new Client(null);
                spy.Connect(e.SpyIP, e.SpyPort, firstPacket);
            }
            Connected();
        }

        /// <summary>
        /// Handles the connection Rejected
        /// </summary>
        /// <param name="e">Event Arguments.</param>
        /// <exception cref="ConnectionFailedException">Is thrown, no other code is executed
        /// inside this exception there is an InnerException <see cref="ClientRejectedException"/>
        /// inside this exception there is an message with the following errors
        /// --> Invalid Key
        /// --> Entity not found
        /// --> Enity Rejected
        /// You can check witch was the error by checking the Message
        /// </exception>
        void Client_Rejected(ServerCommandEventArgs e)
        {
            throw new ConnectionFailedException("Client Rejected", new ClientRejectedException(e.Message));
        }

        /// <summary>
        /// Handle the connection Failed when the server cannot be reached
        /// </summary>
        /// <param name="e">Event Arguments.</param>
        /// <exception cref="ConnectionFailedException">Is thrown when the server can not be reached
        /// </exception>
        void Client_ConnectionFailed(ServerCommandEventArgs e)
        {
            throw new ConnectionFailedException("Connection to the server failed");
        }

        /// <summary>
        /// Handles the invalid HMAC
        /// </summary>
        /// <param name="e">E.</param>
        void Client_InvalidHMAC(ServerCommandEventArgs e)
        {
            throw new ConnectionFailedException("Failed to validate HMAC", new HMACFailedException("HMAC Validation failed", e.OriginalHMAC, e.ComputedHMAC));
        }

        #endregion

    }

    #region "Exceptions Declarations"

    public class InvalidKeyException : Exception
    {
        public InvalidKeyException()
        {
        }

        public InvalidKeyException(string message)
            : base(message)
        {
        }

        public InvalidKeyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ClientRejectedException : Exception
    {
        public ClientRejectedException()
        {
        }

        public ClientRejectedException(string message)
            : base(message)
        {
        }

        public ClientRejectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class HMACFailedException : Exception {

        public string ReceivedHMAC { get; }
        public string ComputedHMAC { get; }

        public HMACFailedException()
        {
        }

        public HMACFailedException(string message)
            : base(message)
        {
        }

        public HMACFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public HMACFailedException(string message, string original, string computed) 
            : base (message) {
            ReceivedHMAC = original;
            ComputedHMAC = computed;
        }
    }

    public class ConnectionFailedException : Exception
    {
        public ConnectionFailedException()
        {
        }

        public ConnectionFailedException(string message)
            : base(message)
        {
        }

        public ConnectionFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    #endregion

}
