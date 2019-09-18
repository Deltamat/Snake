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
        private static int appleSpawnCounter = 0;

        public static List<Apple> AppleList { get => appleList; set => appleList = value; }
        internal static List<Apple> ToBeRemovedApple { get => toBeRemovedApple; set => toBeRemovedApple = value; }
        public static int AppleSpawnCounter { get => appleSpawnCounter; set => appleSpawnCounter = value; }

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

        public static void SpawnApple()
        {
            emptySpace = false;

            while (emptySpace == false)
            {
                emptySpace = true;

                int xCoordinate = GameWorld.Rng.Next(1, 31);
                int yCoordinate = GameWorld.Rng.Next(1, 19);

                if (GameWorld.Player == 1)
                {
                    foreach (Wall item in GameWorld.wallList)
                    {
                        if (item.position == new Vector2(xCoordinate * 30, yCoordinate * 30))
                        {
                            emptySpace = false;
                        }
                    }
                }

                if (GameWorld.Player == 2)
                {
                    foreach (Wall item in GameWorld.wallList)
                    {
                        if (item.position == new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30))
                        {
                            emptySpace = false;
                        }
                    }
                }

                if (GameWorld.Player == 3)
                {
                    foreach (Wall item in GameWorld.wallList)
                    {
                        if (item.position == new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength))
                        {
                            emptySpace = false;
                        }
                    }
                }

                if (GameWorld.Player == 4)
                {
                    foreach (Wall item in GameWorld.wallList)
                    {
                        if (item.position == new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength))
                        {
                            emptySpace = false;
                        }
                    }
                }

                if (emptySpace == true)
                {
                    if (GameWorld.Player == 1)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30), "Apple", GameWorld.ContentManager));
                    }

                    if (GameWorld.Player == 2)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30), "Apple", GameWorld.ContentManager));
                    }

                    if (GameWorld.Player == 3)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager));
                    }

                    if (GameWorld.Player == 4)
                    {
                        AppleList.Add(new Apple(new Vector2(xCoordinate * 30 + Wall.xJumpLength, yCoordinate * 30 + Wall.yJumpLength), "Apple", GameWorld.ContentManager));
                    }

                }
            }
        }
    }
}