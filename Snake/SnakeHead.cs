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
    public class SnakeHead : Snake
    {
        public static Vector2 savedDirection;
        public static bool Alive = true;

        public SnakeHead(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            smallCollisionBox = new GameObject(new Vector2(position.X + 10, position.Y + 10), "Snake_Collision", content);
            GameWorld.toBeAdded.Add(smallCollisionBox);
            direction = new Vector2(1, 0);
            switch (GameWorld.Player)
            {
                case 1:
                    newPosition = GameWorld.TileSet[9, 3].position;
                    oldPosition = GameWorld.TileSet[8, 3].position;
                    break;
                case 2:
                    newPosition = GameWorld.TileSet[40, 3].position;
                    oldPosition = GameWorld.TileSet[39, 3].position;
                    break;
                case 3:
                    newPosition = GameWorld.TileSet[8, 21].position;
                    oldPosition = GameWorld.TileSet[7, 21].position;
                    break;
                case 4:
                    newPosition = GameWorld.TileSet[40, 21].position;
                    oldPosition = GameWorld.TileSet[39, 21].position;
                    break;
            }
            this.position = oldPosition;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Alternate versions for E, S and W is available
            //Change respectively to Ea, Sa and Wa
            if (direction == new Vector2(0, -1))
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("Snake_Head_N");
            }
            else if (direction == new Vector2(1, 0))
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("Snake_Head_Ea");
            }
            else if (direction == new Vector2(0, 1))
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("Snake_Head_Sa");
            }
            else if (direction == new Vector2(-1, 0))
            {
                sprite = GameWorld.ContentManager.Load<Texture2D>("Snake_Head_Wa");
            }
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {   
            #region head-movement
            if (position == newPosition)
            {
                if (savedDirection != Vector2.Zero)
                {
                    direction = savedDirection;
                    savedDirection = Vector2.Zero;
                }
                oldPosition = newPosition;
                newPosition += direction * 30;
            }
            if (Alive)
            {
                position += direction * speed;
            }
            
            #endregion

            #region input-handling
            if (Keyboard.GetState().IsKeyDown(Keys.W)) //Moves up
            {
                if (direction != new Vector2(0, 1))
                {
                    savedDirection = new Vector2(0, -1);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) //Moves down
            {
                if (direction != new Vector2(0, -1))
                {
                    savedDirection = new Vector2(0, 1);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A)) //Moves left
            {
                if (direction != new Vector2(1, 0))
                {
                    savedDirection = new Vector2(-1, 0);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D)) //Moves right
            {
                if (direction != new Vector2(-1, 0))
                {
                    savedDirection = new Vector2(1, 0);
                }
            }
            #endregion

            #region collision
            //Updates the positon of smallCollisionBox to be the same as this object shifted 10 pixels over and down
            smallCollisionBox.position = new Vector2(position.X + 10, position.Y + 10);
            foreach (GameObject obj in GameWorld.gameObjects)
            {
                try
                {
                    //Checks collision between smallCollisionBox and every other object, that's not an apple or a wall, in the game. Excludes itself, its own smallCollisionBox and the first SnakeBody part
                    if (smallCollisionBox.CollisionBox.Intersects(obj.CollisionBox) && obj != this && obj != smallCollisionBox && obj != snakeParts[1])
                    {
                        Alive = false;
                    }
                }
                catch (Exception)
                {

                }
                
            }

            foreach (Apple apple in Apple.AppleList)
            {
                //Checks collision between smallCollisionBox and all apples
                if (smallCollisionBox.CollisionBox.Intersects(apple.CollisionBox))
                {
                    new SnakeBody(Vector2.Zero, "Snake_Body1", GameWorld.ContentManager); //Adds a new bodypart
                    
                    Apple.oldApplePos = apple.position;
                    Apple.ChangeApplePosition(GameWorld.Player);
                    switch (GameWorld.Player)
                    {
                        case 1:
                            GameWorld.SendTCPApple(Apple.oldApplePos, GameWorld.apple1.position);
                            break;
                        case 2:
                            GameWorld.SendTCPApple(Apple.oldApplePos, GameWorld.apple2.position);
                            break;
                        case 3:
                            GameWorld.SendTCPApple(Apple.oldApplePos, GameWorld.apple3.position);
                            break;
                        case 4:
                            GameWorld.SendTCPApple(Apple.oldApplePos, GameWorld.apple4.position);
                            break;
                    }
                }
            }

            lock (GameWorld.ghostPartsLock)
            {
                foreach (Wall wall in GameWorld.wallList)
                {
                    //Checks collision between smallCollisionBox and all walls
                    if (wall != null && smallCollisionBox.CollisionBox.Intersects(wall.CollisionBox))
                    {
                        Alive = false;
                    }
                }
            }
            
            #endregion
            base.Update(gameTime);
        }
    }
}