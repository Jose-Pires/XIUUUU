/*
 * TAClientLib.TAClient.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * This class is the one needed to the program interact with the TrustAgent Server
 * All complex operations are handled by this Library and the program only needs
 * to catch 8 exceptions, handle 3 events and call 3 methods
 * 
 * Requires initialization: YES
 * 
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using static TAClientLib.AESCipher;

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
        public event ServerCommandHandler Kicked;
        public delegate void ServerCommandHandler();
        public event ServerCommandHandler Disconnected;
        public event EntitiesListResponseHandler EntityListReceived;
        public delegate void EntitiesListResponseHandler(List<(string, string)> e);
        public event KeyNegotiationHandler KeyReceived;
        public delegate void KeyNegotiationHandler(byte[] key, IPAddress remoteIP, int remotePORT);

        public delegate void ExceptionHandler(string Message);
        public event ExceptionHandler TimeMissmatch;
        public event ExceptionHandler InvalidCommand;
        public event ExceptionHandler ErrorConnecting;

        public delegate void ClientRejectedExceptionHandler(ServerCommandEventArgs e);
        public event ClientRejectedExceptionHandler EntityNotFound;
        public event ClientRejectedExceptionHandler ClientRejected;

        public delegate void SecurityExceptionHandler(ServerCommandEventArgs e, string originalHMAC = "", string computedHMAC = "");
        public event SecurityExceptionHandler InvalidHMAC;

        internal Client client;
        internal Client spy;
        internal byte[] firstPacket;
        internal readonly byte[] key;
        internal string entity;

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
                this.key = key.FromHexToByteArray();
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
            client.ClientKicked += Client_ClientKicked;
            client.EntityNotFound += Client_EntityNotFound;
            client.InvalidComand += Client_InvalidComand;
            client.InvalidTime += Client_InvalidTime;
            client.KeyReceived += Client_KeyReceived;
            client.Disconnected += Client_Disconnected;
        }

        /// <summary>
        /// Attempts to connected to the TrustAgent server @ IP and Port
        /// </summary>
        /// <param name="IP">TA IP.</param>
        /// <param name="Port">TA Port.</param>
        public void AttemptConnect(string IP, int Port)
        {
            ClientMessage payload = new ClientMessage
            {
                Entity = entity,
                Operation = ClientOperations.AttemptConnect.Value,
                Message = ""
            };
            byte[] packet = Client.BuildPacket(payload, key, PacketType.ClientMessage);
            firstPacket = packet;
            client.Connect(IP, Port, packet);
        }

        /// <summary>
        /// Initializes the key negotiation process
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void RequestKey(string entity, string waitingPort) {

            //byte[] randKey = GenKey(Helpers.GenerateSeed(), 10);
            //byte[] randIV = GenIV(Helpers.GenerateSeed(), 20);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            byte[] encryptedKey = EncryptData(aes.Key, key, aes.IV);

            string tmp_msg = string.Format("{0}|{1}|{2}|{3}", entity, waitingPort, Convert.ToBase64String(encryptedKey), Convert.ToBase64String(aes.IV));

            ClientMessage msg = new ClientMessage
            {
                Entity = this.entity,
                Operation = ClientOperations.RequestKeyNegotiation.Value,
                Message = tmp_msg
            };
            byte[] packet = Client.BuildPacket(msg, key, PacketType.ClientMessage);
            client.SendRequest(packet);
        }

        /// <summary>
        /// Performs a request to the server to find out witch entities are
        /// connected to the server
        /// The list will be returned on the event <see cref="EntityListReceived"/>
        /// </summary>
        public void RequestConnectedEntities() {
            ClientMessage payload = new ClientMessage
            {
                Entity = entity,
                Operation = ClientOperations.RequestConnectedEntities.Value,
                Message = ""
            };
            byte[] packet = Client.BuildPacket(payload, key, PacketType.ClientMessage);
            client.SendRequest(packet);
        }

        public void Disconnect() {
            client.Disconnect();
        }


        #region "Client Connection Handlers"

        /// <summary>
        /// Handles the server disconnected notification
        /// </summary>
        /// <param name="e">E.</param>
        void Client_Disconnected(ServerCommandEventArgs e)
        {
            Disconnected();
        }

        /// <summary>
        /// Handles the entity list received event
        /// </summary>
        /// <param name="e">E.</param>
        void Client_EntityListReceived(List<(string, string)> e)
        {
            EntityListReceived(e);
        }

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
        /// Handles the key received from the key negitiation
        /// </summary>
        /// <param name="_key">Key.</param>
        /// <param name="remoteIP">Remote ip.</param>
        /// <param name="remotePORT">Remote port.</param>
        void Client_KeyReceived(byte[] _key, IPAddress remoteIP, int remotePORT)
        {
            KeyReceived(_key, remoteIP, remotePORT);
        }


        /// <summary>
        /// Invalid packet time to avoid hmac storing
        /// </summary>
        /// <param name="e">E.</param>
        void Client_InvalidTime(ServerCommandEventArgs e)
        {
            TimeMissmatch("An error ocurred while matching the packet time");
        }

        /// <summary>
        /// Handles entity no longer available
        /// </summary>
        /// <param name="e">E.</param>
        void Client_EntityNotFound(ServerCommandEventArgs e)
        {
            EntityNotFound(e);
        }

        /// <summary>
        /// Handles an invalid comand
        /// </summary>
        /// <param name="e">E.</param>
        void Client_InvalidComand(ServerCommandEventArgs e)
        {
            InvalidCommand("Command not recognized");
        }

        /// <summary>
        /// Handles the client kicked
        /// </summary>
        /// <param name="e">E.</param>
        void Client_ClientKicked(ServerCommandEventArgs e)
        {
            Kicked();
        }

        /// <summary>
        /// Handles the connection Rejected
        /// </summary>
        /// <param name="e">Event Arguments.</param>
        /// <exception cref="ClientRejectedException">
        /// inside this exception there is an message with the following errors
        /// --> Invalid Key
        /// --> Entity not found
        /// --> Enity Rejected
        /// You can check witch was the error by checking the Message
        /// </exception>
        void Client_Rejected(ServerCommandEventArgs e)
        {
            ClientRejected(e);
        }

        /// <summary>
        /// Handle the connection Failed when the server cannot be reached
        /// </summary>
        /// <param name="e">Event Arguments.</param>
        /// <exception cref="ConnectionFailedException">Is thrown when the server can not be reached
        /// </exception>
        void Client_ConnectionFailed(ServerCommandEventArgs e)
        {
            ErrorConnecting("Connection to the server failed");
        }

        /// <summary>
        /// Handles the invalid HMAC
        /// </summary>
        /// <param name="e">E.</param>
        void Client_InvalidHMAC(ServerCommandEventArgs e)
        {
            InvalidHMAC(e, e.OriginalHMAC.FromByteArrayToHex(), e.ComputedHMAC.FromByteArrayToHex());
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
