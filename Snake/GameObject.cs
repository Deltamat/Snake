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
        protected Vector2 position;

        public GameObject(Vector2 position, string spriteName, ContentManager content)
        {
            this.position = position;
            sprite = content.Load<Texture2D>(spriteName);
            GameWorld.gameObjects.Add(this);
        }
       
        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public int TranslatePosition(int tilePosition)
        {
            int pos = 0;

            pos = tilePosition * 30 + 15;

            return pos;
        }
    }
}