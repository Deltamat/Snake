﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class Snakehead : Snake
    {
        public static Vector2 savedDirection;
        private bool Alive = true;

        public Snakehead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            smallCollisionBox = new GameObject(new Vector2(position.X + 10, position.Y + 10), "Snake_Collision", content);
            GameWorld.gameObjects.Add(smallCollisionBox);
            int player = 1;
            direction = new Vector2(1, 0);
            switch (player)
            {
                case 1:
                    //newPosition = GameWorld.TileSet[4, 3].position;
                    //oldPosition = GameWorld.TileSet[3, 3].position;
                    newPosition = GameWorld.TileSet[8, 3].position;
                    oldPosition = GameWorld.TileSet[7, 3].position;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Alive)
            {
                foreach (Snake snakePart in snakeParts)
                {
                    GameWorld.toBeRemoved.Add(snakePart);
                    GameWorld.toBeRemoved.Add(snakePart.smallCollisionBox);
                }
            }
            
            #region head-movement
            if (position == newPosition)
            {
                if (savedDirection != Vector2.Zero)
                {
                    direction = savedDirection;
                    savedDirection = Vector2.Zero;
                }
                oldPosition = newPosition;
                newPosition += direction * 30;
            }

            position += direction * speed;
            #endregion

            #region input-handling
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (direction != new Vector2(0, 1))
                {
                    savedDirection = new Vector2(0, -1);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (direction != new Vector2(0, -1))
                {
                    savedDirection = new Vector2(0, 1);
                }
                
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (direction != new Vector2(1, 0))
                {
                    savedDirection = new Vector2(-1, 0);
                }
                
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (direction != new Vector2(-1, 0))
                {
                    savedDirection = new Vector2(1, 0);
                }
                
            }
            #endregion

            #region collision
            smallCollisionBox.position = new Vector2(position.X + 10, position.Y + 10);
            foreach (GameObject obj in GameWorld.gameObjects)
            {
                if (smallCollisionBox.CollisionBox.Intersects(obj.CollisionBox) && obj != this && obj != smallCollisionBox && obj != snakeParts[1] && obj.GetType() != typeof(Apple))
                {
                    Alive = false;
                }
            }

            foreach (Apple apple in Apple.AppleList)
            {
                if (smallCollisionBox.CollisionBox.Intersects(apple.CollisionBox))
                {
                    Apple.ToBeRemovedApple.Add(apple);
                    Wall.SpawnEnemyWalls(GameWorld.Player, (int)(apple.position.X / 30), (int)(apple.position.Y / 30));
                    //Increase tail length
                    //Spawn new apple
                }
            }

            foreach (Wall wall in GameWorld.wallList)
            {
                if (smallCollisionBox.CollisionBox.Intersects(wall.CollisionBox))
                {
                    Alive = false;
                }
            }
            #endregion
            base.Update(gameTime);
        }
    }
}