using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

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

        public GameObject[,] TileSet = new GameObject[64, 36];

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


            graphics.IsFullScreen = false;


            graphics.ApplyChanges();
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

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            

            base.Initialize();

            for (int i = 0; i < 64; i++)
            {
                for (int k = 0; k < 36; k++)
                {
                    if (i % 2 == 0  )
                    {
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "light grass tile", content);
                        k++;
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Dark_Grass_Tile", content);
                    }
                    else
                    {
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "Dark_Grass_Tile", content);
                        k++;
                        TileSet[i, k] = new GameObject(new Vector2(30 * i, 30 * k), "light grass tile", content);
                   
                    }
                }
            }

            for (int i = 0; i < 64; i++)
			{
                wallList.Add(new Wall(new Vector2(30 * i, 0), "WallTile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 35), "WallTile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 17), "WallTile", content));
                wallList.Add(new Wall(new Vector2(30 * i, 30 * 18), "WallTile", content));
			}

            for (int i = 0; i < 36; i++)
			{
                wallList.Add(new Wall(new Vector2(0, 30 * i), "WallTile", content));
                wallList.Add(new Wall(new Vector2(1890, 30 * i), "WallTile", content));
                wallList.Add(new Wall(new Vector2(30 * 31, 30 * i), "WallTile", content));
                wallList.Add(new Wall(new Vector2(30 * 32, 30 * i), "WallTile", content));
			}

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Snakehead head = new Snakehead(new Vector2(GameObject.TranslatePosition(3), GameObject.TranslatePosition(3)), "Snake Head", content);
            Snakebody body = new Snakebody(new Vector2(GameObject.TranslatePosition(2), GameObject.TranslatePosition(3)), "SnakeBody1", content);
            Snakebody body2 = new Snakebody(new Vector2(GameObject.TranslatePosition(1), GameObject.TranslatePosition(3)), "SnakeBody1", content);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (GameObject obj in gameObjects)
            {
                obj.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            for (int i = 0; i < 64; i++)
            {
                for (int k = 0; k < 36; k++)
                {
                    TileSet[i, k].Draw(spriteBatch);
                }
            }

            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(spriteBatch);
            }

            foreach (Wall wall in wallList)
	        {
                wall.Draw(spriteBatch);
	        }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
