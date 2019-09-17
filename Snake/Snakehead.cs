using System;
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
    class Snakehead : Snake
    {
        Vector2 savedDirection;
        private GameObject smallCollisionBox;
        private bool Alive = true;

        public Snakehead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            smallCollisionBox = new GameObject(new Vector2(position.X + 10, position.Y + 10), "SnakeCollision", content);
            GameWorld.gameObjects.Add(smallCollisionBox);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            smallCollisionBox.position = new Vector2(position.X + 10, position.Y + 10);
            foreach (GameObject obj in GameWorld.gameObjects)
            {
                if (smallCollisionBox.CollisionBox.Intersects(obj.CollisionBox) && obj != this && obj != smallCollisionBox && obj != snakeParts[1])
                {
                    Alive = false;
                }
            }
            foreach (Wall wall in GameWorld.wallList)
            {
                if (smallCollisionBox.CollisionBox.Intersects(wall.CollisionBox))
                {
                    Alive = false;
                }
            }

            if (!Alive)
            {
                foreach (GameObject snakePart in snakeParts)
                {
                    GameWorld.toBeRemoved.Add(snakePart);
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

            position += direction;
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

            base.Update(gameTime);
        }
    }
}