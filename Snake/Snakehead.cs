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
        

        public Snakehead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            #region head-movement
            if (position == NewPosition)
            {
                if (savedDirection != Vector2.Zero)
                {
                    direction = savedDirection;
                    savedDirection = Vector2.Zero;
                }
                oldPosition = NewPosition;
                NewPosition += direction * 30;
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