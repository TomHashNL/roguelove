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
        private float damage;

        public Bullet(Room room, Vector2 position, Vector2 velocity, float damage)
            : base(room)
        {
            this.position = position;
            this.velocity = velocity;
            this.texture = room.map.game.Content.Load<Texture2D>("bullet");

            this.rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            this.origin = new Vector2(texture.Width, texture.Height) / 2;

            this.radius = 24;

            this.damage = damage;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[]
            {
                typeof(Block),
                typeof(WallBlock),
                typeof(IDoor),
                typeof(Enemy),
            }), true);

            if (collisions.Count > 0)
            {
                var enemy = collisions.First() as Enemy;
                if (enemy != null)
                {
                    enemy.Health(-damage);
                    enemy.velocity += Vector2.Normalize(velocity) * 6;
                }
                Destroy();
            }



            position += velocity;

            if (position.X < 0 ||
                position.X > room.tilesWidth * room.tileSize ||
                position.Y < 0 ||
                position.Y > room.tilesHeight * room.tileSize)
                Destroy();
        }
    }
}
