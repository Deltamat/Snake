using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    public class Snakebody : Snake
    {
        private GameObject smallCollisionBox;


        public Snakebody(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
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
            #region body movement
            //movement of the body-parts
            if(position == NewPosition)

            if (position == newPosition)
            {
                oldPosition = NewPosition;
                if (placeInList == 1)
                {
                    direction = snakeParts[0].oldPosition - position;
                    direction.Normalize();
                }
                else
                {
                    direction = snakeParts[placeInList - 1].oldPosition - position;
                    direction.Normalize();
                }
                NewPosition += direction * 30;
            }
            position += direction;
            #endregion
            base.Update(gameTime);
        }
    }
}