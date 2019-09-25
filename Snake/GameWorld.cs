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
using Keys = Microsoft.Xna.Framework.Input.Keys;
using System.Security.Cryptography;

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
        private string gameState = "Startup";

        public static GameObject[,] TileSet = new GameObject[64, 36];
        public static SnakeHead head;

        public static List<GameObject> toBeRemoved = new List<GameObject>();
        public static List<GameObject> toBeAdded = new List<GameObject>();

        public static bool player1Dead = false;
        public static bool player2Dead = false;
        public static bool player3Dead = false;
        public static bool player4Dead = false;
        public static Apple apple1;
        public static Apple apple2;
        public static Apple apple3;
        public static Apple apple4;

        public static List<GameObject> ghostPlayer1 = new List<GameObject>();
        public static List<GameObject> ghostPlayer2 = new List<GameObject>();
        public static List<GameObject> ghostPlayer3 = new List<GameObject>();
        public static List<GameObject> ghostPlayer4 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer1 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer2 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer3 = new List<GameObject>();
        public static List<GameObject> toBeAddedGhostPlayer4 = new List<GameObject>();

        public static bool reset = false;
        public static bool sentDead = false;

        string data;
        SpriteFont font;
        bool testBool = false;
        static StreamWriter sWriter;
        public static bool isGhostPlayerReset = false;

        public static List<GameObject> wallList = new List<GameObject>();
        public static List<GameObject> wallsToBeAdded = new List<GameObject>();
        public static List<GameObject> gameObjects = new List<GameObject>();

        public static int player1Score;
        public static int player2Score;
        public static int player3Score;
        public static int player4Score;

        private static string IPInput = "";
        private static bool enter;

        public GameWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
            //Sets the window size
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1020;

#if !DEBUG
            //graphics.IsFullScreen = true;
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
        /// related content. Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            
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

            Thread t = new Thread(RecieveUDP);
            t.IsBackground = true;
            t.Start();

            ghostPlayer1.Add(new GameObject(new Vector2(-100), "Snake_Head_N", ContentManager));
            ghostPlayer2.Add(new GameObject(new Vector2(-100), "Snake_Head_N", ContentManager));
            ghostPlayer3.Add(new GameObject(new Vector2(-100), "Snake_Head_N", ContentManager));
            ghostPlayer4.Add(new GameObject(new Vector2(-100), "Snake_Head_N", ContentManager));
            apple1 = new Apple(new Vector2(300), "Apple", Content);
            apple2 = new Apple(new Vector2(1500, 300), "Apple", Content);
            apple3 = new Apple(new Vector2(300, 900), "Apple", Content);
            apple4 = new Apple(new Vector2(1500, 900), "Apple", Content);
            Apple.AppleList.Add(apple1);
            Apple.AppleList.Add(apple2);
            Apple.AppleList.Add(apple3);
            Apple.AppleList.Add(apple4);

            Window.TextInput += TextInputHandler;
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
                case "Running": //in the 90s!
                    if (!testBool)
                    {
                        SnakeHead head = new SnakeHead(TileSet[8, 3].position, "Snake_Head_N", content);
                        SnakeBody body = new SnakeBody(TileSet[7, 3].position, "Snake_Body1", content);
                        SnakeBody body2 = new SnakeBody(TileSet[6, 3].position, "Snake_Body1", content);
                        testBool = true;
                    }

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

                    #region 

                    lock (ghostPartsLock)
                    {
                        foreach (Wall wall in wallsToBeAdded)
                        {
                            wallList.Add(wall);
                        }
                    }
                    
                    wallsToBeAdded.Clear();

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
                    #endregion

                    SendUDP();

                    break;
                case "Startup":
                    if (enter)
                    {
                        IPInput = IPInput.Remove(IPInput.Length - 1, 1);
                        Thread TCPThread = new Thread(TCPListener);
                        TCPThread.IsBackground = true;
                        TCPThread.Start();

                        Thread.Sleep(500);
                        
                        enter = false;
                    }
                    break;
            }
            
            base.Update(gameTime);

            #region temporary

            if (Keyboard.GetState().IsKeyDown(Keys.F11) && delay > 100)
            {
                graphics.IsFullScreen = true;
                graphics.HardwareModeSwitch = false;
                graphics.ApplyChanges();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F12) && delay > 100)
            {
                graphics.IsFullScreen = false;
                graphics.HardwareModeSwitch = false;
                graphics.ApplyChanges();
            }
#if DEBUG

            if (gameState != "Startup")
            {
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

                if (Keyboard.GetState().IsKeyDown(Keys.P) && delay > 500)
                {
                    if (gameState == "Running")
                    {
                        gameState = "Paused";
                    }
                    else if (gameState == "Paused")
                    {
                        GameState = "Running";
                    }
                    delay = 0;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.R) && delay > 500)
                {
                    ResetGame();
                    delay = 0;
                }
            }
#endif
            #endregion

            if (SnakeHead.Alive == false && sentDead == false)
            {
                SendTCPPlayerDead();
            }

            if (reset == true)
            {
                ResetGame();
                reset = false;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (gameState != "Paused")
            {
                GraphicsDevice.Clear(Color.DarkBlue);
            }
            
            spriteBatch.Begin();

            switch (gameState)
            {
                case "Paused":
                    break;
                case "Running":
                    for (int i = 0; i < 64; i++)
                    {
                        for (int k = 0; k < 36; k++)
                        {
                            TileSet[i, k].Draw(spriteBatch);
                        }
                    }

                    lock (ghostPartsLock)
                    {
                        foreach (Wall wall in wallList)
                        {
                            wall.Draw(spriteBatch);
                        }
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
                    //spriteBatch.DrawString(font, $"{(int)Snake.snakeParts[0].position.X / 30} , {(int)Snake.snakeParts[0].position.Y / 30}", Vector2.Zero, Color.White);
#endif

                    //Score
                    spriteBatch.DrawString(font, $"{player1Score}", new Vector2(480 - font.MeasureString(Convert.ToString(player1Score)).X * 0.5f, 0), Color.WhiteSmoke);
                    spriteBatch.DrawString(font, $"{player2Score}", new Vector2(1440 - font.MeasureString(Convert.ToString(player2Score)).X * 0.5f, 0), Color.WhiteSmoke);
                    spriteBatch.DrawString(font, $"{player3Score}", new Vector2(480 - font.MeasureString(Convert.ToString(player3Score)).X * 0.5f, 540), Color.WhiteSmoke);
                    spriteBatch.DrawString(font, $"{player4Score}", new Vector2(1440 - font.MeasureString(Convert.ToString(player4Score)).X * 0.5f, 540), Color.WhiteSmoke);

                    if (player1Dead)
                    {
                        spriteBatch.DrawString(font, "DEAD", new Vector2(480 - font.MeasureString("DEAD").X * 0.5f, 270 - font.MeasureString("DEAD").Y * 0.5f), Color.Red);
                    }
                    if (player2Dead)
                    {
                        spriteBatch.DrawString(font, "DEAD", new Vector2(1440 - font.MeasureString("DEAD").X * 0.5f, 270 - font.MeasureString("DEAD").Y * 0.5f), Color.Red);
                    }
                    if (player3Dead)
                    {
                        spriteBatch.DrawString(font, "DEAD", new Vector2(480 - font.MeasureString("DEAD").X * 0.5f, 810 - font.MeasureString("DEAD").Y * 0.5f), Color.Red);
                    }
                    if (player4Dead)
                    {
                        spriteBatch.DrawString(font, "DEAD", new Vector2(1440 - font.MeasureString("DEAD").X * 0.5f, 810 - font.MeasureString("DEAD").Y * 0.5f), Color.Red);
                    }

                    foreach (Apple apple in Apple.AppleList)
                    {
                        apple.Draw(spriteBatch);
                    }
                    break;
                case "Startup":
                    try
                    {
                        spriteBatch.DrawString(font, IPInput, new Vector2(960 - font.MeasureString(IPInput).X * 0.5f, 540 - font.MeasureString(IPInput).Y * 0.5f), Color.White);
                    }
                    catch (Exception)
                    {
                        IPInput = "";
                    }
                    break;
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

            IPAddress serverIPAddress = IPAddress.Parse(IPInput);
            string datastring = $"{Player}";
            foreach (Snake obj in Snake.snakeParts)
            {
                datastring += ":" + obj.position.X.ToString() + ":" + obj.position.Y.ToString();
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
                if (player == Player.ToString())
                {
                    player = "0";
                }
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
                    // divided by 2 because there are 2 coordinates for each element in "list"
                    while ((array.Length - 1) / 2 > list.Count)
                    {
                        list.Add(new GameObject(Vector2.Zero, "Snake_Body1", Content));
                    }
                    // if the UDP package contains more parts because the package has been sent after a reset
                    // reduce the size of the snake
                    if ((array.Length - 1) * 0.5 < list.Count)
                    {
                        list.RemoveRange(array.Length / 2, list.Count - array.Length / 2);
                    }
                    foreach (GameObject obj in list)
                    {
                        obj.position = new Vector2(Convert.ToInt32(array[counter]), Convert.ToInt32(array[counter + 1]));
                        counter += 2;
                    }
                }
            }
            catch
            {

            }
        }

        private void TCPListener()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(IPInput), serverPort);
                // sets two streams
                sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
                StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);

                IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                IPEndPoint localPoint = (IPEndPoint)client.Client.LocalEndPoint;

                Player = Convert.ToInt32(sReader.ReadLine());
                string data;

                GameState = "Running";

                SnakeHead.Alive = false;
                SendTCPPlayerDead();

                try
                {
                    while (true)
                    {
                        data = sReader.ReadLine();
                        string decrypted = CryptoHelper.Decrypt<TripleDESCryptoServiceProvider>(data, "password1234", "salt");
                        string[] stringArray = decrypted.Split(':');
                        switch (stringArray[0])
                        {
                            case "EatApple":
                                Wall.SpawnEnemyWalls(Convert.ToInt32(stringArray[1]), Convert.ToInt32(stringArray[2]) / 30, Convert.ToInt32(stringArray[3]) / 30);
                                switch (Convert.ToInt32(stringArray[1]))
                                {
                                    case 1:
                                        apple1.position = new Vector2(Convert.ToInt32(stringArray[4]), Convert.ToInt32(stringArray[5]));
                                        player1Score++;
                                        break;
                                    case 2:
                                        apple2.position = new Vector2(Convert.ToInt32(stringArray[4]), Convert.ToInt32(stringArray[5]));
                                        player2Score++;
                                        break;
                                    case 3:
                                        apple3.position = new Vector2(Convert.ToInt32(stringArray[4]), Convert.ToInt32(stringArray[5]));
                                        player3Score++;
                                        break;
                                    case 4:
                                        apple4.position = new Vector2(Convert.ToInt32(stringArray[4]), Convert.ToInt32(stringArray[5]));
                                        player4Score++;
                                        break;
                                }
                                break;
                            case "PlayerDead":
                                switch (Convert.ToInt32(stringArray[1]))
                                {
                                    case 1:
                                        player1Dead = true;
                                        break;
                                    case 2:
                                        player2Dead = true;
                                        break;
                                    case 3:
                                        player3Dead = true;
                                        break;
                                    case 4:
                                        player4Dead = true;
                                        break;
                                }
                                break;
                            case "Reset":
                                reset = true;
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    Exit();
                }
            }
            catch (Exception)
            {
                IPInput = "";
            }
        }

        public static void SendTCPApple(Vector2 oldApplePosition, Vector2 newApplePosition)
        {
            string data = "EatApple" + ":" + $"{Player}" + ":" + $"{oldApplePosition.X}" + ":" + $"{oldApplePosition.Y}" + ":" + $"{newApplePosition.X}" + ":" + $"{newApplePosition.Y}";
            // encrypt the datastring
            string encrypted = CryptoHelper.Encrypt<TripleDESCryptoServiceProvider>(data, "password1234", "salt");
            sWriter.WriteLine(encrypted);
            sWriter.Flush();
        }

        public static void SendTCPPlayerDead()
        {
            string data = string.Empty;
            switch (Player)
            {
                case 1:
                    data = "PlayerDead:" + $"{Player}:" + player1Score;
                    break;
                case 2:
                    data = "PlayerDead:" + $"{Player}:" + player2Score;
                    break;
                case 3:
                    data = "PlayerDead:" + $"{Player}:" + player3Score;
                    break;
                case 4:
                    data = "PlayerDead:" + $"{Player}:" + player4Score;
                    break;
            }

            // encrypt the datastring
            string encrypted = CryptoHelper.Encrypt<TripleDESCryptoServiceProvider>(data, "password1234", "salt");

            sWriter.WriteLine(encrypted);
            sWriter.Flush();
            sentDead = true;
        }

        /// <summary>
        /// Resets relevant values to base
        /// </summary>
        public void ResetGame()
        {
            lock (ghostPartsLock)
            {
                SnakeHead.Alive = true;
                sentDead = false;
                SnakeHead.savedDirection = new Vector2(1, 0);

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
                //Apple.AppleList.Clear();
                //for (int i = 1; i <= 4; i++)
                //{
                //    Apple.SpawnApple(i);
                //}
                apple1.position = new Vector2(300);
                apple2.position = new Vector2(1500, 300);
                apple3.position = new Vector2(300, 900);
                apple4.position = new Vector2(1500, 900);
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
                player1Dead = false;
                player2Dead = false;
                player3Dead = false;
                player4Dead = false;
                #endregion

                ghostPlayer1.Clear();
                ghostPlayer2.Clear();
                ghostPlayer3.Clear();
                ghostPlayer4.Clear();
                ghostPlayer1.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
                ghostPlayer2.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
                ghostPlayer3.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));
                ghostPlayer4.Add(new GameObject(new Vector2(-100), "Snake_Head", ContentManager));

                isGhostPlayerReset = true;
            }
        }

        /// <summary>
        /// Handles user keystrokes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
           
            if (gameState == "Startup")
            {
                Keys pressedKey = args.Key;
                char character = args.Character;
                
                IPInput += character;
                if (pressedKey == Keys.Enter || character.ToString() == "\r")
                {
                    enter = true;
                }
            }
        }
    }
}
