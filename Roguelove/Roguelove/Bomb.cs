using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Bomb : Entity
    {
        int frames;
        float blastRadius;

        public Bomb(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.texture = room.map.game.Content.Load<Texture2D>("player");
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
            this.color = new Color(new Vector3(.3f));
            this.collidable = false;

            this.blastRadius = room.tileSize * 2f;
        }

        protected override void OnDestroy()
        {
            
        }

        public override void Update()
        {
            frames++;

            if (!collidable)
                if (room.map.playersControl.Where(e => e.player != null && (position - e.player.position).LengthSquared() < room.tileSize * room.tileSize).Count() == 0)
                    collidable = true;

            velocity *= .95f;

            Collide(new HashSet<Type>(new[]
            {
                typeof(Block),
                typeof(WallBlock),
                typeof(Hole),
                typeof(Bomb),
                typeof(Player),
                typeof(Bullet),
            }), true);

            position += velocity;

            if (frames > 200)
            {
                Destroy();
                foreach (var entity in room.entities)
                {
                    Type type = entity.GetType();

                    if (type == typeof(Block))
                        if ((position - (entity.position + new Vector2(entity.radius))).LengthSquared() < blastRadius * blastRadius)
                            entity.Destroy();
                }
            }
        }
    }
}
