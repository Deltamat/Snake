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
        private static int player = 1;
        private static Random rng = new Random();

        public static GameObject[,] TileSet = new GameObject[64, 36];
        public static Snakehead head;

        public static List<GameObject> toBeRemoved = new List<GameObject>();

        string test;
        SpriteFont font;

        public static List<GameObject> wallList = new List<GameObject>();
        public static List<GameObject> gameObjects = new List<GameObject>();

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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
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

            Snakehead head = new Snakehead(TileSet[8, 3].position, "Snake_Head", content);
            Snakebody body = new Snakebody(TileSet[7, 3].position, "Snake_Body1", content);
            Snakebody body2 = new Snakebody(TileSet[6, 3].position, "Snake_Body1", content);
            Snakebody body3 = new Snakebody(TileSet[5, 3].position, "Snake_Body1", content);
            Snakebody body6 = new Snakebody(TileSet[4, 3].position, "Snake_Body1", content);
            Snakebody body4 = new Snakebody(TileSet[3, 3].position, "Snake_Body1", content);
            Snakebody body5 = new Snakebody(TileSet[2, 3].position, "Snake_Body1", content);
            Snakebody body8 = new Snakebody(TileSet[1, 3].position, "Snake_Body1", content);
            Snakebody body7 = new Snakebody(TileSet[0, 3].position, "Snake_Body1", content);
            Thread t = new Thread(RecieveUDP);
            t.IsBackground = true;
            t.Start();

            Apple.SpawnApple(1);

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

            foreach (GameObject obj in gameObjects)
            {
                obj.Update(gameTime);
            }

            foreach (GameObject objRemove in toBeRemoved)
            {
                gameObjects.Remove(objRemove);
            }
            toBeRemoved.Clear();

            foreach (Apple apple in Apple.ToBeRemovedApple)
            {
                Apple.AppleList.Remove(apple);
            }
            Apple.ToBeRemovedApple.Clear();

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

            base.Update(gameTime);

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
                //Wall.SpawnEnemyWalls(1,4,9);
                //Wall.SpawnEnemyWalls(2,40,4);
                //Wall.SpawnEnemyWalls(3,9,2);
                //Wall.SpawnEnemyWalls(4,2,14);
                //int lastBodyPartInList = Snake.snakeParts.Count - 1;
                //new Snakebody((Snake.snakeParts[lastBodyPartInList].position + Snake.snakeParts[lastBodyPartInList].direction * 30),"Snake_Body1",content);
                new Snakebody(Vector2.Zero, "Snake_Body1", content);
                delay = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && delay > 50)
            {
                Apple.SpawnApple(Player);
                delay = 0;
            }
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

            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(spriteBatch);
                DrawCollisionBox(obj);
            }           

            if (test != null)
            {
                spriteBatch.DrawString(font, test, new Vector2(0), Color.Red);
            }

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

            byte[] sendbuf = Encoding.ASCII.GetBytes(Snakehead.savedDirection.ToString());

            IPEndPoint ep = new IPEndPoint(serverIPAddress, 42070);

            socket.SendTo(sendbuf, ep);
        }
        
        public void RecieveUDP()
        {
            int listenPort = 11001;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            while (true)
            {
                byte[] bytes = listener.Receive(ref groupEP);
                test = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}
