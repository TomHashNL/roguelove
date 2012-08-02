﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Bullet : Entity
    {
        Vector2 velocity;

        public Bullet(Room room, Vector2 position, Vector2 velocity)
            : base(room)
        {
            this.position = position;
            this.velocity = velocity;
            this.texture = room.map.game.Content.Load<Texture2D>("bullet");

            this.rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        protected override void OnDestroy()
        {
            
        }

        public override void Update()
        {
            position += velocity;

            if (position.X < 0 ||
                position.X > room.tilesWidth * room.tileSize ||
                position.Y < 0 ||
                position.Y > room.tilesHeight * room.tileSize)
                Destroy();
        }
    }
}
