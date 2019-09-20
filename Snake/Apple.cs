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
    class Apple : GameObject
    {
        private static bool emptySpace;
        private static List<Apple> appleList = new List<Apple>();
        private static List<Apple> toBeRemovedApple = new List<Apple>();
        private static int appleSpawnCounterPlayer1 = 0;
        private static int appleSpawnCounterPlayer2 = 0;
        private static int appleSpawnCounterPlayer3 = 0;
        private static int appleSpawnCounterPlayer4 = 0;

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
                int yCoordinate = GameWorld.Rng.Next(1, 19);

                Apple apple = new Apple(Vector2.Zero, "Apple", GameWorld.ContentManager);

                //Might not work, couldn't test it
                if (player == 1)
                {
                    apple = new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 2)
                {
                    apple = new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 3)
                {
                    apple = new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                else if (player == 4)
                {
                    apple = new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager); //Spawns a ghost apple
                }
                
                foreach (Snake snakePart in Snake.snakeParts)
                {
                    //Checks if the ghost apple intersects with any of the snake's bodyparts
                    if (snakePart.CollisionBox.Intersects(apple.CollisionBox))
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
    }
}