using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Snake
{
    public class Snakebody : Snake
    {
        public Snakebody(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
