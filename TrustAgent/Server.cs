using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace TrustAgent
{
    public class Server
    {
        //Global Variables
        Thread thread;
        bool stop = false;
        public EventArgs e = null;
        public Hashtable clientList = new Hashtable();
        public Hashtable clientHandlers = new Hashtable();
        byte[] agentDatabase;

        //Events
        public event ClientHandler ClientConnected;
        public delegate void ClientHandler(byte[] m, TcpClient socket, EventArgs e);

        public Server(IPAddress listeningIP, int listeningPort, byte[] adb = null, int seed = 0)
        {
            thread = new Thread(() => Listen(listeningIP, listeningPort));
            thread.Start();
        }

        void Listen(IPAddress listeningIP, int listeningPort)
        {
            TcpListener serverSocket = new TcpListener(listeningIP, listeningPort);
            TcpClient clientSocket = default(TcpClient);

            int counter = 0;

            serverSocket.Start();
            counter = 0;

            while (!stop)
            {
                if (serverSocket.Pending())
                {
                    counter += 1;
                    clientSocket = serverSocket.AcceptTcpClient();

                    byte[] bytesFrom = new byte[clientSocket.ReceiveBufferSize];

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);

                    ClientConnected(bytesFrom, clientSocket, e);
                }
            }

            if (clientSocket != null)
                clientSocket.Close();
            serverSocket.Stop();
            /*foreach (DictionaryEntry item in clientsHandlers)
                ((HandleClient)item.Value).Disconnect();*/

        }

        /*public void EvalConnection(string identifier, TcpClient clientSocket)
        {

            clientsList.Add(identifier, clientSocket);
            new Thread(delegate ()
            {
                NetworkStream broadcastStream = clientSocket.GetStream();
                byte[] connectedString = Encoding.ASCII.GetBytes("connected");
                broadcastStream.Write(connectedString, 0, connectedString.Length);
                broadcastStream.Flush();
            }).Start();
            HandleClient client = new HandleClient();
            client.CommandReceived += Client_CommandReceived;
            client.StartClient(clientSocket, identifier, clientsList);
            clientsHandlers.Add(identifier, client);
        }*/



    }
}
