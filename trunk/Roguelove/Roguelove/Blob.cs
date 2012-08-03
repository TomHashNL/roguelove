using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Blob : Enemy, ISolid
    {
        public Blob(Room room, Vector2 position)
            : base(room, position, 10)
        {
            texture = room.map.game.Content.Load<Texture2D>("enemy");
            origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[] {
                typeof(ISolid),
                typeof(DoorOpen),
            }), true);

            velocity *= .8f;

            position += velocity;
        }

        protected override void OnDestroy()
        {

        }
    }
}
