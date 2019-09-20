using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace Snake
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameWorld : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private static ContentManager content;
        private float delay;
        private Texture2D collisionTexture;
        private static int player = 0;
        private static Random rng = new Random();
        public static object ghostPartsLock = new object();
        public int serverPort = 42000;
        private string gameState = "Running";

        public static GameObject[,] TileSet = new GameObject[64, 36];
        public static SnakeHead head;

        public static List<GameObject> toBeRemoved = new List<GameObject>();
        public static List<GameObject> toBeAdded = new List<GameObject>();

        public static List<GameObject> ghostPlayer1 = new List<GameObject>();
        public static List<GameObject> ghostPlayer2 = new List<GameObject>();
        public static List<GameObject> ghostPlayer3 = new List<GameObject>();
        public static List<GameObject> ghostPlayer4 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer1 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer2 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer3 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer4 = new List<GameObject>();

        string data;
        SpriteFont font;

        public static List<GameObject> wallList = new List<GameObject>();
        public static List<GameObject> gameObjects = new List<GameObject>();

        public static int player1Score;
        public static int player2Score;
        public static int player3Score;
        public static int player4Score;

        public GameWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            //Sets the window size
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1020;

#if !DEBUG
            graphics.IsFullScreen = true;
#endif

            graphics.ApplyChanges();

            IsMouseVisible = true;
        }

        /// <summary>
        /// Gets content
        /// </summary>
        public static ContentManager ContentManager
        {
            get
            {
                return content;
            }
        }

        public static int Player { get => player; set => player = value; }
        public static Random Rng { get => rng; set => rng = value; }
        public string GameState { get => gameState; set => gameState = value; }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Thread TCPThread = new Thread(TCPListener);
            TCPThread.IsBackground = true;
            TCPThread.Start();

            Thread.Sleep(500);

            // Generates the background tiles
            for (int i = 0; i < 64; i++)
            {
                for (int k = 0; k < 36; k++)
                {
                    //Checks if "i" is dividable by 2.
                    if (i % 2 == 0  )
                    {
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Light_Grass_Tile", content);
                        k++;
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Dark_Grass_Tile", content);
                    }
                    else
                    {
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Dark_Grass_Tile", content);
                        k++;
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Light_Grass_Tile", content);
                    }
                }
            }

            // Creates walls at appropiate places.
            for (int i = 0; i < 64; i++)
			{
                wallList.Add(new Wall(new Vector2(30 * i, 0), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 35), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 17), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 18), "Wall_Tile", content));
			}

            for (int i = 0; i < 36; i++)
			{
                wallList.Add(new Wall(new Vector2(0, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(1890, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * 31, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * 32, 30 * i), "Wall_Tile", content));
			}

            SnakeHead head = new SnakeHead(Vector2.Zero, "Snake_Head_N", content);
            SnakeBody body = new SnakeBody(Vector2.Zero, "Snake_Body1", content);
            SnakeBody body2 = new SnakeBody(Vector2.Zero, "Snake_Body1", content);

            Thread t = new Thread(RecieveUDP);
            t.IsBackground = true;
            t.Start();

            ghostPlayer1.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
            ghostPlayer2.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
            ghostPlayer3.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
            ghostPlayer4.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));

            Apple.SpawnApple(player);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("Font");
            
            collisionTexture = content.Load<Texture2D>("CollisionTexture");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            delay += gameTime.ElapsedGameTime.Milliseconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            switch (GameState)
            {
                case "Paused":
                    break;
                case "Running":
                    foreach (GameObject obj in gameObjects)
                    {
                        obj.Update(gameTime);
                    }

                    foreach (GameObject objAdd in toBeAdded)
                    {
                        gameObjects.Add(objAdd);
                    }
                    toBeAdded.Clear();

                    foreach (GameObject objRemove in toBeRemoved)
                    {
                        gameObjects.Remove(objRemove);
                    }
                    toBeRemoved.Clear();

            #region apples
            foreach (Apple apple in Apple.ToBeRemovedApple)
            {
                Apple.AppleList.Remove(apple);
            }
            Apple.ToBeRemovedApple.Clear();

                    lock (ghostPartsLock)
                    {
                        foreach (GameObject obj in toBeAddedGhostPlayer1)
                        {
                            ghostPlayer1.Add(obj);
                        }
                        toBeAddedGhostPlayer1.Clear();
                        foreach (GameObject obj in toBeAddedGhostPlayer2)
                        {
                            ghostPlayer2.Add(obj);
                        }
                        toBeAddedGhostPlayer2.Clear();
                        foreach (GameObject obj in toBeAddedGhostPlayer3)
                        {
                            ghostPlayer3.Add(obj);
                        }
                        toBeAddedGhostPlayer3.Clear();
                        foreach (GameObject obj in toBeAddedGhostPlayer4)
                        {
                            ghostPlayer4.Add(obj);
                        }
                        toBeAddedGhostPlayer4.Clear();
                    }


                    //Checks if there are any apples to create like a pseudo-list
                    if (Apple.AppleSpawnCounterPlayer1 != 0)
                    {
                        for (int i = 0; i < Apple.AppleSpawnCounterPlayer1; i++)
                        {
                            Apple.SpawnApple(1);
                        }
                        Apple.AppleSpawnCounterPlayer1 = 0;
                    }

                    if (Apple.AppleSpawnCounterPlayer2 != 0)
                    {
                        for (int i = 0; i < Apple.AppleSpawnCounterPlayer2; i++)
                        {
                            Apple.SpawnApple(2);
                        }
                        Apple.AppleSpawnCounterPlayer2 = 0;
                    }

                    if (Apple.AppleSpawnCounterPlayer3 != 0)
                    {
                        for (int i = 0; i < Apple.AppleSpawnCounterPlayer3; i++)
                        {
                            Apple.SpawnApple(3);
                        }
                        Apple.AppleSpawnCounterPlayer3 = 0;
                    }

                    if (Apple.AppleSpawnCounterPlayer4 != 0)
                    {
                        for (int i = 0; i < Apple.AppleSpawnCounterPlayer4; i++)
                        {
                            Apple.SpawnApple(4);
                        }
                        Apple.AppleSpawnCounterPlayer4 = 0;
                    }
                    SendUDP();
                    break;
            }
            #endregion

            base.Update(gameTime);

            #region temporary
#if DEBUG
            if (Keyboard.GetState().IsKeyDown(Keys.D1) && delay > 100)
            {
                player = 1;
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D2) && delay > 100)
            {
                player = 2;
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D3) && delay > 100)
            {
                player = 3;
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D4) && delay > 100)
            {
                player = 4;
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E) && delay > 500)
            {
                new SnakeBody(Vector2.Zero, "Snake_Body1", content);
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && delay > 50)
            {
                Apple.SpawnApple(Player);
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P) && delay > 50)
            {
                GameState = "Running";
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.R) && delay > 500)
            {
                ResetGame();
                delay = 0;
            }
#endif
            #endregion
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Cyan);
            spriteBatch.Begin();

            for (int i = 0; i < 64; i++)
            {
                for (int k = 0; k < 36; k++)
                {
                    TileSet[i, k].Draw(spriteBatch);
                }
            }

            foreach (Wall wall in wallList)
            {
                wall.Draw(spriteBatch);
            }
            lock (ghostPartsLock)
            {
                foreach (GameObject obj in ghostPlayer1)
                {
                    obj.Draw(spriteBatch);
                }
                foreach (GameObject obj in ghostPlayer2)
                {
                    obj.Draw(spriteBatch);
                }
                foreach (GameObject obj in ghostPlayer3)
                {
                    obj.Draw(spriteBatch);
                }
                foreach (GameObject obj in ghostPlayer4)
                {
                    obj.Draw(spriteBatch);
                }
            }
            

            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(spriteBatch);
#if DEBUG
                DrawCollisionBox(obj);
#endif
            }

#if DEBUG
            spriteBatch.DrawString(font, $"{(int)Snake.snakeParts[0].position.X / 30} , {(int)Snake.snakeParts[0].position.Y / 30}", Vector2.Zero, Color.White);
#endif

            //Score
            spriteBatch.DrawString(font, $"{player1Score}", new Vector2(480 - font.MeasureString(Convert.ToString(player1Score)).X * 0.5f, 0), Color.WhiteSmoke);
            spriteBatch.DrawString(font, $"{player2Score}", new Vector2(1440 - font.MeasureString(Convert.ToString(player1Score)).X * 0.5f, 0), Color.WhiteSmoke);
            spriteBatch.DrawString(font, $"{player3Score}", new Vector2(480 - font.MeasureString(Convert.ToString(player1Score)).X * 0.5f, 540), Color.WhiteSmoke);
            spriteBatch.DrawString(font, $"{player4Score}", new Vector2(1440 - font.MeasureString(Convert.ToString(player1Score)).X * 0.5f, 540), Color.WhiteSmoke);

            foreach (Apple item in Apple.AppleList)
            {
                item.Draw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draw collision boxes around the GameObject 'go'
        /// </summary>
        /// <param name="go">A GameObject</param>
        private void DrawCollisionBox(GameObject go)
        {
            Rectangle collisionBox = go.CollisionBox;
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(collisionTexture, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw collision boxes for the Rectangle 'collisionBox'
        /// </summary>
        /// <param name="collisionBox">A rectangle</param>
        public void DrawRectangle(Rectangle collisionBox)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(collisionTexture, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(collisionTexture, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public void SendUDP()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress serverIPAddress = IPAddress.Parse("127.0.0.1");
            string datastring = $"{Player + 1}:";
            foreach (Snake obj in Snake.snakeParts)
            {
                obj.position += new Vector2(960, 0);
                datastring += obj.position.X.ToString() + ":" + obj.position.Y.ToString() + ":";
                obj.position -= new Vector2(960, 0);
            }
            byte[] sendbuf = Encoding.ASCII.GetBytes(datastring);

            IPEndPoint ep = new IPEndPoint(serverIPAddress, 43000);

            socket.SendTo(sendbuf, ep);
        }
        
        public void RecieveUDP()
        {
            int listenPort = 43001;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                string[] stringArray = data.Split(':');
                string player = stringArray[0];
                switch (player)
                {
                    case "1":
                        UpdateGhostPlayers(ghostPlayer1, stringArray);
                        break;
                    case "2":
                        UpdateGhostPlayers(ghostPlayer2, stringArray);
                        break;
                    case "3":
                        UpdateGhostPlayers(ghostPlayer3, stringArray);
                        break;
                    case "4":
                        UpdateGhostPlayers(ghostPlayer4, stringArray);
                        break;
                }
            }
        }

        private void UpdateGhostPlayers(List<GameObject> list, string[] array)
        {
            int counter = 1;
            try
            {
                lock (ghostPartsLock)
                {
                    while ((array.Length + 1) * 2 > list.Count)
                    {
                        list.Add(new GameObject(Vector2.Zero, "Snake_Body1", Content));
                    }
                }
                foreach (GameObject obj in list)
                {
                    obj.position = new Vector2(Convert.ToInt32(array[counter]), Convert.ToInt32(array[counter + 1]));
                    counter += 2;
                }
            }
            catch
            {

            }
        }

        private void TCPListener()
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), serverPort);
            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

            IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            IPEndPoint localPoint = (IPEndPoint)client.Client.LocalEndPoint;

            Player = Convert.ToInt32(sReader.ReadLine());
            //while (true)
            //{
                
            //}
        }

        /// <summary>
        /// Resets relevant values to base
        /// </summary>
        public void ResetGame()
        {
            #region walls
            wallList.Clear();
            for (int i = 0; i < 64; i++)
            {
                wallList.Add(new Wall(new Vector2(30 * i, 0), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 35), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 17), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 18), "Wall_Tile", content));
            }

            for (int i = 0; i < 36; i++)
            {
                wallList.Add(new Wall(new Vector2(0, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(1890, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * 31, 30 * i), "Wall_Tile", content));
                wallList.Add(new Wall(new Vector2(30 * 32, 30 * i), "Wall_Tile", content));
            }
            #endregion
            #region apples
            Apple.AppleList.Clear();
            for (int i = 1; i <= 4; i++)
            {
                Apple.SpawnApple(i);
            }
            #endregion
            #region snake
            foreach (Snake snakePart in Snake.snakeParts)
            {
                gameObjects.Remove(snakePart.smallCollisionBox);
                gameObjects.Remove(snakePart);
            }
            Snake.snakeParts.Clear();
            SnakeHead head = new SnakeHead(Vector2.Zero, "Snake_Head_N", content);
            SnakeBody body = new SnakeBody(Vector2.Zero, "Snake_Body1", content);
            SnakeBody body2 = new SnakeBody(Vector2.Zero, "Snake_Body1", content);
            #endregion
            #region score
            player1Score = 0;
            player2Score = 0;
            player3Score = 0;
            player4Score = 0;
            #endregion
        }
    }
}
