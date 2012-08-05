using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public abstract class Item : Entity
    {
        public Item(Room room, Vector2 position, bool velocity)
            : base(room)
        {
            this.position = position;
            this.radius = 24;

            if (velocity)
            {
                double rotation = room.map.game.random.NextDouble() * Math.PI;
                float speed = (float)room.map.game.random.NextDouble() * .1f;
                this.velocity = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * speed;
            }
        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[]
            {
                typeof(Block),
                typeof(WallBlock),
                typeof(Hole),
                typeof(Door),
                typeof(Player),
            }), true);

            Player player = collisions.FirstOrDefault(e => e is Player) as Player;
            if (player != null)
                Pickup(player);

            velocity *= .97f;

            position += velocity;
        }

        public abstract void Pickup(Player player);
    }
}
