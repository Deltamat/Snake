﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using RestSharp;
using CryptoLibrary;
using System.Security.Cryptography;

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
            Console.WriteLine("Server IP: " + Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString());
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

            iPs.Add(endPoint.Address); // add ip to list so UDP can be sent to all connected players

            // send a player number to the client
            sWriter.WriteLine(playerNumber);
            sWriter.Flush();

            String data;
            while (true)
            {
                // reads from stream
                try
                {
                    data = sReader.ReadLine();
                    string decrypted = CryptoHelper.Decrypt<TripleDESCryptoServiceProvider>(data, "password1234", "salt");
                    string[] array = decrypted.Split(':');
                    switch (array[0])
                    {
                        case "EatApple":
                            Console.WriteLine($"Player {array[1]} ate an apple. YUMMY");
                            break;
                        case "PlayerDead":
                            deadPlayers.Add(Convert.ToInt32(array[1]));
                            Console.WriteLine($"Player {array[1]} died. How SAD");
                            PostREST(endPoint.Address.ToString(), Convert.ToInt32(array[2])); // sends highscore to REST
                            break;
                    }

                    string encrypted = CryptoHelper.Encrypt<TripleDESCryptoServiceProvider>(decrypted, "password1234", "salt");

                    foreach (StreamWriter writer in streamWriters)
                    {
                        writer.WriteLine(encrypted);
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
                    deadPlayers.Remove(playerNumber);
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

        /// <summary>
        /// Method that handles recieving UDP from the clients and sending those packages to all players.
        /// </summary>
        static void RecieveAndTransmitUDPData()
        {
            int listenPort = 43000;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (true)
            {
                // recieve from klient
                byte[] bytes = listener.Receive(ref groupEP);

                // send to all players
                // lock because an ip in the list could get removed if a player leaves
                lock (playersLock)
                {
                    foreach (IPAddress ip in iPs)
                    {
                        IPEndPoint ep = new IPEndPoint(ip, 43001);
                        socket.SendTo(bytes, ep);
                    }
                }
            }
        }

        /// <summary>
        /// Sends a message to all connected clients telling them to reset
        /// </summary>
        private static void Reset()
        {
            foreach (StreamWriter writer in streamWriters)
            {
                string resetString = "Reset";
                string encrypted = CryptoHelper.Encrypt<TripleDESCryptoServiceProvider>(resetString, "password1234", "salt");

                writer.WriteLine(encrypted);
                writer.Flush();
            }
            deadPlayers.Clear();
            Console.WriteLine("All players dead. Reset the game");
        }

        /// <summary>
        /// Sends a post request to the RESTful web service
        /// </summary>
        /// <param name="ip">The ip address of the player</param>
        /// <param name="score">The score of the player</param>
        private static void PostREST(string ip, int score)
        {
            RestRequest request = new RestRequest("api/highscore", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { Ip = ip, Score = score });
            client.Execute(request);
        }
    }
}
