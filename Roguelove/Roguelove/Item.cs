using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public abstract class Item : Entity, ISolid
    {
        public Item(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.radius = 24;
        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[] { typeof(ISolid), typeof(Door), }), true);

            Player player = collisions.FirstOrDefault(e => e is Player) as Player;
            if (player != null)
                Pickup(player);

            velocity *= .9f;

            position += velocity;
        }

        public abstract void Pickup(Player player);
    }
}
