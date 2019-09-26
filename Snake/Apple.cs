using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Apple : GameObject
    {
        private static bool emptySpace;
        private static List<Apple> appleList = new List<Apple>();

        public static Vector2 oldApplePos;

        public static List<Apple> AppleList { get => appleList; set => appleList = value; }

        public Apple(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Changes the position of the existing main player apple and the others ghost apples.
        /// </summary>
        /// <param name="player">Selects what player it is, 1 - 4</param>
        public static void ChangeApplePosition(int player)
        {
            emptySpace = false;

            while (emptySpace == false)
            {
                emptySpace = true;

                //Picks a random number to the length of the tile array X and Y
                int xCoordinate = GameWorld.Rng.Next(1, 31);
                int yCoordinate = GameWorld.Rng.Next(1, 17);

                Apple ghostApple = new Apple(Vector2.Zero, "Apple", GameWorld.ContentManager);

                //Creates an apple depending on the player position
                if (player == 1)
                {
                    ghostApple = new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 2)
                {
                    ghostApple = new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 3)
                {
                    ghostApple = new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 4)
                {
                    ghostApple = new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }

                int surroundingWallTiles = 0;

                // lock because the Main Thread runs through wallList and the TCPListener can add a wall 
                lock (GameWorld.ghostPartsLock)
                {
                    foreach (Wall wall in GameWorld.wallList)
                    {
                        //Checks if the surrounding tiles around where the apple would be placed are walls
                        if (wall.position == new Vector2(ghostApple.position.X - 30, ghostApple.position.Y))
                        {
                            surroundingWallTiles++;
                        }

                        if (wall.position == new Vector2(ghostApple.position.X, ghostApple.position.Y + 30))
                        {
                            surroundingWallTiles++;
                        }

                        if (wall.position == new Vector2(ghostApple.position.X + 30, ghostApple.position.Y))
                        {
                            surroundingWallTiles++;
                        }

                        if (wall.position == new Vector2(ghostApple.position.X, ghostApple.position.Y - 30))
                        {
                            surroundingWallTiles++;
                        }
                    }
                }


                //If there are 3 or more walls around the apple, the placement is invalid
                if (surroundingWallTiles >= 3)
                {
                    emptySpace = false;
                }

                foreach (Snake snakePart in Snake.snakeParts)
                {
                    //Checks if the ghost apple intersects with any of the snake's bodyparts
                    if (snakePart.CollisionBox.Intersects(ghostApple.CollisionBox))
                    {
                        emptySpace = false;
                    }
                }

                //Checks if the apple would hit a wall, if it does it will retry
                if (player == 1)
                {
                    foreach (Wall wall in GameWorld.wallList)
                    {
                        if (wall.position == new Vector2(xCoordinate * 30, yCoordinate * 30))
                        {
                            emptySpace = false;
                        }
                    }
                }

                lock (GameWorld.ghostPartsLock)
                {
                    if (player == 2)
                    {
                        foreach (Wall wall in GameWorld.wallList)
                        {
                            if (wall.position == new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30))
                            {
                                emptySpace = false;
                            }
                        }
                    }

                    if (player == 3)
                    {
                        foreach (Wall wall in GameWorld.wallList)
                        {
                            if (wall.position == new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength))
                            {
                                emptySpace = false;
                            }
                        }
                    }

                    if (player == 4)
                    {
                        foreach (Wall wall in GameWorld.wallList)
                        {
                            if (wall.position == new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength))
                            {
                                emptySpace = false;
                            }
                        }
                    }
                }

                if (emptySpace == true)
                {
                    if (player == 1)
                    {
                        GameWorld.apple1.position = new Vector2(xCoordinate * 30, yCoordinate * 30);
                    }

                    if (player == 2)
                    {
                        GameWorld.apple2.position = new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30);
                    }

                    if (player == 3)
                    {
                        GameWorld.apple3.position = new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength);
                    }

                    if (player == 4)
                    {
                        GameWorld.apple4.position = new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength);
                    }
                }
            }
        }
    }
}