using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TrustAgent
{

    public class Server
    {
        Thread thread;
        bool stop;

        public event ClientHandler ClientConnected;
        public delegate void ClientHandler(byte[] m, TcpClient socket, EventArgs e);

        public EventArgs e;

        public Hashtable clientsList = new Hashtable();
        public Hashtable clientsHandlers = new Hashtable();

        public Server(IPAddress listeningIP, int listeningPort)
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
                    networkStream.Read(bytesFrom, 0, clientSocket.ReceiveBufferSize);

                    ClientConnected(bytesFrom, clientSocket, e);
                }
            }

            // TODO: Implement the server disconnect phase
            /*SendToClients(Encoding.ASCII.GetBytes("disconnecting"));
            if (clientSocket != null)
                clientSocket.Close();
            serverSocket.Stop();
            foreach (DictionaryEntry item in clientsHandlers)
                ((HandleClient)item.Value).Disconnect();*/
        }

        public void AcceptClient(string identifier, TcpClient clientSocket)
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
        }

        void Client_CommandReceived(HandleClient handler, byte[] m)
        {
            string id = handler.clientID;
            clientsList.Remove(id);
            clientsHandlers.Remove(id);
            handler.Disconnect();
        }

        public void DenyClientConnection(TcpClient clientSocket, string message)
        {
            new Thread(delegate ()
            {
                // TODO: Implement the connection denial message
                /*NetworkStream broadcastStream = clientSocket.GetStream();
                byte[] failedString = Encoding.ASCII.GetBytes("auth_failed");
                broadcastStream.Write(failedString, 0, failedString.Length);
                broadcastStream.Flush();*/
            }).Start();
        }

        public void Shutdown()
        {
            stop = true;
        }

        public void SendToClients(byte[] data)
        {
            int threads = clientsList.Count;
            CountdownEvent countdownEvent = new CountdownEvent(threads);

            foreach (DictionaryEntry Item in clientsList)
            {
                new Thread(delegate ()
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();

                    broadcastStream.Write(data, 0, data.Length);
                    broadcastStream.Flush();

                    countdownEvent.Signal();
                }).Start();
            }
            countdownEvent.Wait();
        }

    }

    public class HandleClient
    {
        public event ClientCommand CommandReceived;

        public delegate void ClientCommand(HandleClient handler, byte[] m);
        public EventArgs e;

        TcpClient clientSocket;
        public string clientID;
        Hashtable clientsList;
        Thread ctThread;

        public void StartClient(TcpClient inClientSocket, string clientID, Hashtable cList)
        {
            clientSocket = inClientSocket;
            this.clientID = clientID;
            clientsList = cList;
            ctThread = new Thread(DoChat)
            {
                IsBackground = true
            };
            ctThread.Start();
        }

        public void Disconnect()
        {
            ctThread.Abort();
        }

        void DoChat()
        {
            int requestCount = 0;
            requestCount = 0;

            while (true)
            {
                try
                {
                    byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    int length = networkStream.Read(inStream, 0, clientSocket.ReceiveBufferSize);
                    Array.Resize(ref inStream, length);
                    CommandReceived(this, inStream);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(ex.ToString());
                }
            }
        }
    }

}
