using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Chest : Entity, ISolid
    {
        bool open;

        public Chest(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("chest");
            this.position = position;
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {
            velocity *= .9f;

            var collisions = Collide(new HashSet<Type>(new[] { typeof(ISolid), }), true);
            if (!open)
                foreach (var entity in collisions)
                    if (entity.GetType() == typeof(Player))
                        Open(entity as Player);

            position += velocity;
        }

        private void Open(Player player)
        {
            open = true;
            texture = room.map.game.Content.Load<Texture2D>("chestOpen");

            //instantiate shit ;D
        }
    }
}
