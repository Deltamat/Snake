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
    public class Snake : GameObject
    {
        List<Snake> snakeParts = new List<Snake>();
        protected Vector2 direction = new Vector2(1, 0);

        public Snake(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            snakeParts.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 0.1f, new SpriteEffects(), 1f);
        }
    }
}