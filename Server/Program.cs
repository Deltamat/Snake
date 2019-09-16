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
    class Program
    {
        private static int port = 42069;
        private static TcpListener server;
        private static bool isRunning;
        private static TcpClient[] Players = new TcpClient[3];

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
            while (isRunning)
            {
                TcpClient newClient = server.AcceptTcpClient();
                bool placed = false;
                for (int i = 0; i < 3; i++)
                {
                    if (Players[i] != null && !placed)
                    {
                        Players[i] = newClient;
                        placed = true;
                    }
                }

                if (placed)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.IsBackground = true;
                    t.Start(newClient);
                }    
            }
        }

        static void HandleClient(object obj)
        {
            Byte[] bytes = new Byte[256];
            String data;
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                }
            }
        }
    }
}
