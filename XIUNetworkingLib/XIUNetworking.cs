using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace XIUNetworkingLib
{
    /*
     * HOW TO USE:
     * 
     * Initialize XIUNetworking with the name of the entity
     * Listen to the events "Connected", "NewUser", "MessageReceived" and "SVMessageReceived"
     * Events Info
     *      "Connected"         -> When an attempt to connect to another user was a success
     *      "NewUser"           -> A has connected
     *      "MessageReceived"   -> You (as connection initializer) received a message   
     *      "SVMessageReceived" -> You (as connection receiver) received a message
     * 
     * To send messages
     * Allways use the function Client.BuildPacket()
     * If you started the connection
     *         ClientInstances.FirstOrDefault(ci => ci.RemoteEntity == "the entity you want to send to").ClientNetworking.SendRequest(byte[] packet)
     * If you received the connection
     *         RemoteClients.Where(rc => rc.Key == "the entity you want to send to").Value.SendMessage(byte[] packet)
     */
    public class XIUNetworking
    {

        public delegate void NetworkingEvent(ClientEventArgs e);
        public NetworkingEvent Connected;
        public InfoEvent NewUser;
        public SVMessageEvent ConnectionLost;
        public MessageEvent CLConnectionLost;
        public delegate void MessageEvent(byte[] data, ClientInstance instance, ClientEventArgs e);
        public delegate void SVMessageEvent(byte[] data, ClientHandler instance, ClientEventArgs e);
        public delegate void InfoEvent(string message);
        public MessageEvent MessageReceived;
        public SVMessageEvent SVMessageReceived;

        public List<ClientInstance> ClientInstances { get; }
        public Dictionary<string, ClientHandler> RemoteClients => server.clientHandlers;
        public Server server;
        public string Entity { get; 
        }
        public XIUNetworking(string Entity, int svPort)
        {
            ClientInstances = new List<ClientInstance>();
            List<IPAddress> ips = GetLocalIPAddress();
            this.Entity = Entity;
            server = new Server(ips.First(), svPort, Entity);
            server.ClientConnected += Server_ClientConnected;
            server.MessageReceived += Server_MessageReceived;
            server.ConnectionLost += Server_ConnectionLost;
        }

        private void Server_ConnectionLost(byte[] m, ClientHandler clientHandler, ClientEventArgs e)
        {
            ConnectionLost(null, clientHandler, e);
        }

        public void InitializeConnection(string ip, int port, string entity) {
            Client tempClient = new Client(ip, port);
            tempClient.Connected += TempClient_Connected;
            try {
                tempClient.Connect(entity);
            } catch (ClientInitException cie) {
                throw cie;
            } catch (Exception ex) {
                throw ex;
            }
        }

        void TempClient_Connected(Client instance, ClientEventArgs e)
        {
            string strData = Encoding.ASCII.GetString(e.Data);
            string cmd = strData.Split('_')[0];
            if (cmd == "connected") {
                // Connection was accepted.
                instance.MessageReceived += Instance_MessageReceived;
                instance.ConnectionLost += Instance_ConnectionLost;
                ClientInstances.Add(new ClientInstance
                {
                    ClientNetworking = instance,
                    RemoteEntity = strData.Split('_')[1]
                });
                Connected(e);
            } else
                throw new Exception("An error ocurred: " + strData.Split('_')[0]);
        }
        
        void Instance_ConnectionLost(Client instance, ClientEventArgs e) 
        {
            CLMessageReceived(null, ClientInstances.First(m => m.ClientNetworking == instance), null);
        }

        void Instance_MessageReceived(Client instance, ClientEventArgs e)
        {
            MessageReceived(e.Data, ClientInstances.First(m => m.ClientNetworking == instance), e);
        }

        void Server_ClientConnected(byte[] m, TcpClient socket, EventArgs e)
        {
            NewUser(Encoding.ASCII.GetString(m).Split('_')[1]);
            server.AcceptClient(Encoding.ASCII.GetString(m).Split('_')[1], socket);
        }

        void Server_MessageReceived(byte[] m, ClientHandler clientHandler, ClientEventArgs e)
        {
            SVMessageReceived(m, clientHandler, e);
        }


        #region "Helpers"
        List<IPAddress> GetLocalIPAddress()
        {
            var _host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> ips = new List<IPAddress>();
            foreach (IPAddress ip in _host.AddressList)
            {
                if ((ip.AddressFamily == AddressFamily.InterNetwork) && (ip.ToString() != "127.0.0.1"))
                    ips.Add(ip);
            }
            return ips;
        }
        #endregion
    }
}
