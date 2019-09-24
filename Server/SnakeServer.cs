using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using RestSharp;

namespace Server
{
    class SnakeServer
    {
        private static readonly int port = 42000;
        private static TcpListener server;
        private static bool isRunning;
        private static TcpClient[] Players = new TcpClient[4];
        private static List<int> deadPlayers = new List<int>();
        private static List<StreamWriter> streamWriters = new List<StreamWriter>();
        private static List<IPAddress> iPs = new List<IPAddress>();
        private static int connectedPlayers = 0;

        public static object playersLock = new object();

        // RESTful
        private static RestClient client = new RestClient("http://localhost:62915/");


        static void Main(string[] args)
        {
            Server(port);
        }

        static void Server(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            LoopClients();
        }

        static void LoopClients()
        {

            Thread t2 = new Thread(RecieveAndTransmitUDPData);
            t2.IsBackground = true;
            t2.Start();

            while (isRunning)
            {
                bool placed = false;
                TcpClient newClient = server.AcceptTcpClient();
                for (int i = 0; i < Players.Count(); i++)
                {
                    if (Players[i] == null && !placed)
                    {
                        connectedPlayers++;
                        placed = true;
                        Players[i] = newClient;
                        Thread t = new Thread(new ParameterizedThreadStart(HandleClient))
                        {
                            IsBackground = true
                        };
                        t.Start(newClient);
                    }
                }

                if (!placed)
                {
                    newClient.Close();
                }
            }
        }

        static void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;
            int playerNumber = Array.IndexOf(Players, client) + 1;
            Console.WriteLine($"Player {playerNumber} connected");
            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            streamWriters.Add(sWriter);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            IPEndPoint localPoint = (IPEndPoint)client.Client.LocalEndPoint;

            iPs.Add(endPoint.Address);
            //IPEndPoint ep = new IPEndPoint(local, listenPort);


            sWriter.WriteLine(playerNumber);
            sWriter.Flush();

            Byte[] bytes = new Byte[256];
            String data;
            while (true)
            {
                // reads from stream
                try
                {
                    data = sReader.ReadLine();
                    string[] array = data.Split(':');
                    switch (array[0])
                    {
                        case "0":
                            Console.WriteLine($"Player {array[1]} ate an apple. YUMMY");
                            PostREST();
                            break;
                        case "1":
                            deadPlayers.Add(Convert.ToInt32(array[1]));
                            Console.WriteLine($"Player {array[1]} died. How SAD");
                            // hvis kun en spiller tilbage send score til REST og send besked til klienter om reset
                            // ved reset Clear() listen.
                            break;
                    }
                    foreach (var writer in streamWriters)
                    {
                        writer.WriteLine(data);
                        writer.Flush();
                    }

                    if (deadPlayers.Count == connectedPlayers)
                    {
                        Reset();
                    }
                }
                catch (Exception)
                {
                    connectedPlayers--;
                    lock (playersLock)
                    {
                        iPs.Remove(endPoint.Address);
                        streamWriters.Remove(sWriter);
                        Players[Array.IndexOf(Players, client)] = null;
                    }
                    if (deadPlayers.Count == connectedPlayers)
                    {
                        Reset();
                    }
                    Console.WriteLine("Player " + playerNumber + " " + endPoint.Address.ToString() + ":" + endPoint.Port.ToString() + " disconnected");
                    Thread.CurrentThread.Abort();
                }
            }
        }

        static void RecieveAndTransmitUDPData(object obj)
        {
            int listenPort = 43000;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //string returnAdresse = groupEP.ToString().Remove(groupEP.ToString().IndexOf(":"));

            //IPAddress broadcast = IPAddress.Parse("10.131.69.125");

            //IPEndPoint ep = new IPEndPoint(broadcast, 43001);

            while (true)
            {
                // recieve from klient
                byte[] bytes = listener.Receive(ref groupEP);

                //socket.SendTo(bytes, ep);

                //send to all players unfinished
                lock (playersLock)
                {
                    foreach (var ip in iPs)
                    {
                        IPEndPoint ep = new IPEndPoint(ip, 43001);
                        socket.SendTo(bytes, ep);

                    }

                }
            }
        }

        private static void Reset()
        {
            foreach (var writer in streamWriters)
            {
                writer.WriteLine("2:RESET");
                writer.Flush();
            }
            deadPlayers.Clear();
            Console.WriteLine("All players dead. Reset the game");
        }

        private static void PostREST()
        {
            var request = new RestRequest("api/highscore", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { Name = "IPTEST", value = "123" });
            client.Execute(request);
        }
    }
}
