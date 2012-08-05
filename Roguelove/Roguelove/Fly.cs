using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Fly : Enemy
    {
        public Fly(Room room, Vector2 position)
            : base(room, position, 3)
        {
            texture = room.map.game.Content.Load<Texture2D>("fly");
            origin = new Vector2(texture.Width, texture.Height) / 2;
            this.radius = 16;
        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[] {
                typeof(WallBlock),
                typeof(IDoor),
                typeof(Player),
            }), true);

            //Find closest player and attack ;D
            var lol = room.map.playersControl.MinT(e => (e.player.position - position).LengthSquared());
            if (lol != null)
                velocity += Vector2.Normalize(lol.player.position - position) * .5f;

            velocity *= .8f;

            position += velocity;
        }

        protected override void OnDestroy()
        {

        }
    }
}
