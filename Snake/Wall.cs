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
    class Wall : GameObject
    {
        public static int xJumpLength = 32 * 30;
        public static int yJumpLength = 18 * 30;
        public Wall(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public static void SpawnEnemyWalls(int player, int xSquare, int ySquare)
         {
            if (player == 1)
            {
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare + yJumpLength), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare, 30 * ySquare + yJumpLength), "WallTile", GameWorld.ContentManager));
            }

            if (player == 2)
            {
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare + yJumpLength), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare, 30 * ySquare + yJumpLength), "WallTile", GameWorld.ContentManager));
            }

            if (player == 3)
            {
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare + xJumpLength, 30 * ySquare - yJumpLength), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare, 30 * ySquare - yJumpLength), "WallTile", GameWorld.ContentManager));
            }

            if (player == 4)
            {
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare - xJumpLength, 30 * ySquare - yJumpLength), "WallTile", GameWorld.ContentManager));
                GameWorld.wallList.Add(new Wall(new Vector2(30 * xSquare, 30 * ySquare - yJumpLength), "WallTile", GameWorld.ContentManager));
            }


         }
    }
}