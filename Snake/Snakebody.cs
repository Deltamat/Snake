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
            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            #region body movement
            //movement of the body-parts
            if(position == NewPosition)
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