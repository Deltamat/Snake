using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

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
        }
       
        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }
    }
}