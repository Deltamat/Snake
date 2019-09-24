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
        private static List<Apple> toBeRemovedApple = new List<Apple>();
        private static int appleSpawnCounterPlayer1 = 0;
        private static int appleSpawnCounterPlayer2 = 0;
        private static int appleSpawnCounterPlayer3 = 0;
        private static int appleSpawnCounterPlayer4 = 0;

        public static Vector2 oldApplePos;

        public static List<Apple> AppleList { get => appleList; set => appleList = value; }
        public static List<Apple> ToBeRemovedApple { get => toBeRemovedApple; set => toBeRemovedApple = value; }
        public static int AppleSpawnCounterPlayer1 { get => appleSpawnCounterPlayer1; set => appleSpawnCounterPlayer1 = value; }
        public static int AppleSpawnCounterPlayer2 { get => appleSpawnCounterPlayer2; set => appleSpawnCounterPlayer2 = value; }
        public static int AppleSpawnCounterPlayer3 { get => appleSpawnCounterPlayer3; set => appleSpawnCounterPlayer3 = value; }
        public static int AppleSpawnCounterPlayer4 { get => appleSpawnCounterPlayer4; set => appleSpawnCounterPlayer4 = value; }

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
        /// Spawns an apple at a random tile within the given player's playing field
        /// </summary>
        /// <param name="player"></param>
        public static void SpawnApple(int player)
        {
            emptySpace = false;

            while (emptySpace == false)
            {
                emptySpace = true;

                int xCoordinate = GameWorld.Rng.Next(1, 31);
                int yCoordinate = GameWorld.Rng.Next(1, 17);

                Apple ghostApple = new Apple(Vector2.Zero, "Apple", GameWorld.ContentManager);
                
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
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30), "Apple", GameWorld.ContentManager));
                    }

                    if (player == 2)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30), "Apple", GameWorld.ContentManager));
                    }

                    if (player == 3)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager));
                    }

                    if (player == 4)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager));
                    }
                }
            }
        }

        public static void ChangeApplePosition(int player)
        {
            emptySpace = false;

            while (emptySpace == false)
            {
                emptySpace = true;

                int xCoordinate = GameWorld.Rng.Next(1, 31);
                int yCoordinate = GameWorld.Rng.Next(1, 17);

                Apple ghostApple = new Apple(Vector2.Zero, "Apple", GameWorld.ContentManager);

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
                        //GameWorld.SendTCPApple(oldApplePos, GameWorld.apple1.position);
                    }

                    if (player == 2)
                    {
                        GameWorld.apple2.position = new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30);
                        //GameWorld.SendTCPApple(oldApplePos, GameWorld.apple2.position);
                    }

                    if (player == 3)
                    {
                        GameWorld.apple3.position = new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength);
                        //GameWorld.SendTCPApple(oldApplePos, GameWorld.apple3.position);
                    }

                    if (player == 4)
                    {
                        GameWorld.apple4.position = new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength);
                        //GameWorld.SendTCPApple(oldApplePos, GameWorld.apple4.position);
                    }
                }
            }
        }
    }
}