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
        public Snakebody(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            smallCollisionBox = new GameObject(new Vector2(position.X + 10, position.Y + 10), "Snake_Collision", content);
            GameWorld.gameObjects.Add(smallCollisionBox);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            #region body movement
            //movement of the body-parts
            if (position == newPosition)
            {
                oldPosition = newPosition;
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
                newPosition += direction * 30;
            }
            position += direction * speed;
            #endregion

            smallCollisionBox.position = new Vector2(position.X + 10, position.Y + 10);

            base.Update(gameTime);
        }
    }
}