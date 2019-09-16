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
        GameObject newTile = GameWorld.TileSet[4, 3];
        Vector2 newTileCoords = new Vector2(4, 3);
        GameObject oldTile = GameWorld.TileSet[3, 3];

        public Snakehead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            if (position == newTile.position)
            {
                if (savedDirection != Vector2.Zero)
                {
                    direction = savedDirection;
                    savedDirection = Vector2.Zero;
                }

                oldTile = newTile;
            }

            position += direction;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                newTileCoords.Y -= 1;
                savedDirection = new Vector2(0, -1);
                newTile = GameWorld.TileSet[(int)newTileCoords.X, (int)newTileCoords.Y];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                newTileCoords.Y += 1;
                savedDirection = new Vector2(0, 1);
                newTile = GameWorld.TileSet[(int)newTileCoords.X, (int)newTileCoords.Y];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                newTileCoords.X -= 1;
                savedDirection = new Vector2(-1, 0);
                newTile = GameWorld.TileSet[(int)newTileCoords.X, (int)newTileCoords.Y];
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                newTileCoords.Y += 1;
                savedDirection = new Vector2(1, 0);
                newTile = GameWorld.TileSet[(int)newTileCoords.X, (int)newTileCoords.Y];
            }
            base.Update(gameTime);
        }
    }
}
