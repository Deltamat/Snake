﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    class Wall : GameObject
    {
        public const int xJumpLength = 960;
        public const int yJumpLength = 540;

        public Wall(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public static void SpawnEnemyWalls(int player, int xSquare, int ySquare)
        {
            Wall player1Wall;
            Wall player2Wall;
            Wall player3Wall;
            Wall player4Wall;

            if (player == 1)
            {
                player2Wall = new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare), "Wall_Tile", GameWorld.ContentManager);
                player3Wall = new Wall(new Vector2(30 * xSquare, 30 * ySquare + yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player4Wall = new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare + yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                foreach (Apple apple in Apple.AppleList)
                {
                    if (apple.CollisionBox.Intersects(player2Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer2++;
                    }
                    else if (apple.CollisionBox.Intersects(player3Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer3++;
                    }
                    else if (apple.CollisionBox.Intersects(player4Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer4++;
                    }
                }
                GameWorld.wallList.Add(player2Wall);
                GameWorld.wallList.Add(player3Wall);
                GameWorld.wallList.Add(player4Wall);
            }

            if (player == 2)
            {
                player1Wall = new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare), "Wall_Tile", GameWorld.ContentManager);
                player3Wall = new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare + yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player4Wall = new Wall(new Vector2(30 * xSquare, 30 * ySquare + yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                foreach (Apple apple in Apple.AppleList)
                {
                    if (apple.CollisionBox.Intersects(player1Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer1++;
                    }
                    else if (apple.CollisionBox.Intersects(player3Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer3++;
                    }
                    else if (apple.CollisionBox.Intersects(player4Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer4++;
                    }
                }
                GameWorld.wallList.Add(player1Wall);
                GameWorld.wallList.Add(player3Wall);
                GameWorld.wallList.Add(player4Wall);
            }

            if (player == 3)
            {
                player1Wall = new Wall(new Vector2(30 * xSquare, 30 * ySquare - yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player2Wall = new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare - yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player4Wall = new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare), "Wall_Tile", GameWorld.ContentManager);
                foreach (Apple apple in Apple.AppleList)
                {
                    if (apple.CollisionBox.Intersects(player1Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer1++;
                    }
                    else if (apple.CollisionBox.Intersects(player2Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer2++;
                    }
                    else if (apple.CollisionBox.Intersects(player4Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer4++;
                    }
                }
                GameWorld.wallList.Add(player1Wall);
                GameWorld.wallList.Add(player2Wall);
                GameWorld.wallList.Add(player4Wall);
            }

            if (player == 4)
            {
                player1Wall = new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare - yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player2Wall = new Wall(new Vector2(30 * xSquare, 30 * ySquare - yJumpLength), "Wall_Tile", GameWorld.ContentManager);
                player3Wall = new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare), "Wall_Tile", GameWorld.ContentManager);
                foreach (Apple apple in Apple.AppleList)
                {
                    if (apple.CollisionBox.Intersects(player1Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer1++;
                    }
                    else if (apple.CollisionBox.Intersects(player2Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer2++;
                    }
                    else if (apple.CollisionBox.Intersects(player3Wall.CollisionBox))
                    {
                        Apple.ToBeRemovedApple.Add(apple);
                        Apple.AppleSpawnCounterPlayer3++;
                    }
                }
                GameWorld.wallList.Add(player1Wall);
                GameWorld.wallList.Add(player2Wall);
                GameWorld.wallList.Add(player3Wall);
            }
        }
    }
}