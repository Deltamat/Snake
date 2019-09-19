using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Snake
{
    public class GameObject
    {
        protected Texture2D sprite;
        public Vector2 position;

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle((int)(position.X), (int)(position.Y), (int)(sprite.Width), (int)(sprite.Height));
            }
        }

        public GameObject(Vector2 position, string spriteName, ContentManager content)
        {
            this.position = position;
            sprite = content.Load<Texture2D>(spriteName);
        }
       
        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void UpdatePosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        /// <summary>
        /// Returns the integer for the center of tilePosition, either x or y, from TileSet array
        /// </summary>
        /// <param name="tilePosition"></param>
        /// <returns></returns>
        public static int TranslatePosition(int tilePosition)
        {
            int returnPosition = 0;

            returnPosition = tilePosition * 30 + 15;

            return returnPosition;
        }

        /// <summary>
        /// Returns the vector for the center of tilePosition from TileSet array
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 TranslatePosition(Vector2 vector)
        {
            Vector2 returnVector = Vector2.Zero;

            returnVector = new Vector2(vector.X * 30 + 15, vector.Y * 30 + 15);

            return returnVector;
        }
    }
}