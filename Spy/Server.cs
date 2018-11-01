using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Spy
{
    public class Server
    {
        Thread thread;
        bool stop;
        int i = 0;
        public event ClientHandler ClientConnected;
        public delegate void ClientHandler(byte[] packet, byte[] m, TcpClient socket, EventArgs e);

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

                    byte[] dataLength = new byte[4];

                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(dataLength, 0, 4);
                    byte[] bytesFrom = new byte[BitConverter.ToInt32(dataLength)];
                    networkStream.Read(bytesFrom, 0, BitConverter.ToInt32(dataLength));

                    byte[] packet = new byte[4 + bytesFrom.Length];
                    Array.Copy(dataLength, packet, 4);
                    Array.Copy(bytesFrom, 0, packet, 4, bytesFrom.Length);
                    ClientConnected(packet, bytesFrom, clientSocket, e);
                    AcceptClient(i.ToString(), clientSocket);
                    i++;
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
            HandleClient client = new HandleClient();
            client.CommandReceived += Client_CommandReceived;
            client.StartClient(clientSocket, identifier, clientsList);
            clientsHandlers.Add(identifier, client);
        }

        void Client_CommandReceived(HandleClient handler, byte[] m)
        {
            string id = handler.clientID;
            byte[] dataLength = new byte[4];
            Array.Copy(m, 0, dataLength, 0, 4);
            byte[] data = new byte[BitConverter.ToInt32(dataLength)];
            byte[] cleanPacket = new byte[4 + BitConverter.ToInt32(dataLength)];
            Array.Copy(dataLength, cleanPacket, 4);
            Array.Copy(m,4,data, 0, BitConverter.ToInt32(dataLength));
            Array.Copy(data, 0, cleanPacket, 4, data.Length);
            ClientConnected(cleanPacket, data, (TcpClient)clientsList[id], null);

            /*clientsList.Remove(id);
            clientsHandlers.Remove(id);
            handler.Disconnect();*/
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
