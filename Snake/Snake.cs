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
        protected static List<Snake> snakeParts = new List<Snake>();
        public Vector2 direction = new Vector2(1, 0);
        protected int placeInList;
        public Vector2 newPosition;
        public Vector2 oldPosition;
        public float speed = 3;



        public Snake(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            if (snakeParts.Count == 0)
            {
                newPosition = GameWorld.TileSet[4, 3].position;
                oldPosition = GameWorld.TileSet[3, 3].position;
            }
            else
            {
                newPosition = GameWorld.TileSet[4, 3].position;
                oldPosition = GameWorld.TileSet[3, 3].position;
            }
            

            GameWorld.gameObjects.Add(this);
            snakeParts.Add(this);
            placeInList = snakeParts.Count - 1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 1f, new SpriteEffects(), 1f);
            spriteBatch.Draw(sprite, CollisionBox, Color.White);
        }
    }
}