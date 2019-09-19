using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class SnakeServer
    {
        private static readonly int port = 42000;
        private static TcpListener server;
        private static bool isRunning;
        private static List<TcpClient> Players = new List<TcpClient>(4);

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
                TcpClient newClient = server.AcceptTcpClient();
                if (Players.Count < 4)
                {
                    Players.Add(newClient);
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient))
                    {
                        IsBackground = true
                    };
                    t.Start(newClient);
                }
                else
                {
                    newClient.Close();
                }
            }
        }

        static void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            int playerNumber = Players.Count();
            TcpClient client = (TcpClient)obj;
            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            IPEndPoint localPoint = (IPEndPoint)client.Client.LocalEndPoint;

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
                }
                catch (Exception)
                {
                    Players.Remove(client);
                    Console.WriteLine(endPoint.Port.ToString() + " " + localPoint.Port.ToString() + " disconnected");
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

            IPAddress broadcast = IPAddress.Parse("127.0.0.1");

            IPEndPoint ep = new IPEndPoint(broadcast, listenPort);

            while (true)
            {
                // recieve from klient
                byte[] bytes = listener.Receive(ref groupEP);

                // send to all players (ToBeContinued)
                socket.SendTo(bytes, ep);
            }
        }
    }
}
