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

        public Snakehead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            position += direction;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                direction = new Vector2(0, -1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction = new Vector2(0, 1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                direction = new Vector2(-1, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                direction = new Vector2(1, 0);
            }
            base.Update(gameTime);
        }
    }
}
