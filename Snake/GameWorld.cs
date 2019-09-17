﻿using Microsoft.Xna.Framework;
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
        private Texture2D collisionTexture;

        public static GameObject[,] TileSet = new GameObject[64, 36];

        public static List<GameObject> toBeRemoved = new List<GameObject>();

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

            // Generates the background tiles
            for (int i = 0; i < 64; i++)
            {
                for (int k = 0; k < 36; k++)
                {
                    //Checks if "i" is dividable by 2.
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

            // Creates walls at appropiate places.
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

            Snakehead head = new Snakehead(TileSet[3, 3].position, "Snake Head", content);
            Snakebody body = new Snakebody(TileSet[2, 3].position, "SnakeBody1", content);
            Snakebody body2 = new Snakebody(TileSet[1, 3].position, "SnakeBody1", content);
            Snakebody body3 = new Snakebody(TileSet[0, 3].position, "SnakeBody1", content);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
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

            foreach (Wall wall in wallList)
            {
                wall.Draw(spriteBatch);
            }

            foreach (GameObject obj in gameObjects)
            {
                obj.Draw(spriteBatch);
                DrawCollisionBox(obj);
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
    }
}
