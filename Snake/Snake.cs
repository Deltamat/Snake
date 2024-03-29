﻿using Microsoft.Xna.Framework;
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
        public static List<Snake> snakeParts = new List<Snake>();
        public Vector2 direction;
        protected int placeInList;
        public Vector2 newPosition;
        public Vector2 oldPosition;
        public float speed = 3;
        public GameObject smallCollisionBox;
        public bool readyToMove;
        
        public Snake(Vector2 position, string spriteName, ContentManager content) : base(position, spriteName, content)
        {
            GameWorld.toBeAdded.Add(this);
            snakeParts.Add(this);
            placeInList = snakeParts.Count - 1;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, CollisionBox, Color.White);
        }
    }
}