using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Bomb : Entity, ISolid
    {
        int countDown;
        float blastRadius;
        private float damage;

        public Bomb(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.texture = room.map.game.Content.Load<Texture2D>("player");
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
            this.color = new Color(new Vector3(.3f));
            this.collidable = false;

            this.blastRadius = room.tileSize * 4f;

            this.damage = 6;

            this.countDown = 120;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {
            countDown--;

            if (!collidable)
                if (room.map.playersControl.Where(e => e.player != null && (position - e.player.position).LengthSquared() < room.tileSize * room.tileSize).Count() == 0)
                    collidable = true;

            velocity *= .95f;

            foreach (var entity in Collide(new HashSet<Type>(new[]
            {
                typeof(ISolid),
                typeof(Bullet),
                typeof(IDoor),
            }), true))
                if (entity is Bullet)
                    entity.Destroy();

            position += velocity;

            //bomb destroy
            if (countDown <= 0)
            {
                Destroy();
                foreach (var entity in room.entities)
                    if (entity is ISolid)
                    {
                        float distance = (position - (entity.position + new Vector2(entity.radius))).Length();
                        if (distance < blastRadius)
                        {
                            Vector2 offset = entity.position - position;
                            if (offset == Vector2.Zero) offset = Vector2.UnitX;
                            offset.Normalize();
                            entity.velocity += offset * (blastRadius - distance) / 8;

                            if (entity is Enemy) (entity as Enemy).Health(-damage);

                            {//player
                                var player = entity as Player;
                                if (player != null)
                                    player.Health(-2);
                            }

                            if (entity is Block && distance < blastRadius / 2)
                            {
                                entity.Destroy();

                                //Fill holes
                                Vector2 gridPos = new Vector2((int)((position.X) / room.tileSize) * room.tileSize, (int)((position.Y) / room.tileSize) * room.tileSize);

                                Vector2 delta = entity.position - gridPos;
                                if (delta.Length() == room.tileSize)
                                {
                                    var hole = room.entities.FirstOrDefault(e => e.position == entity.position + delta && e is Hole) as Hole;
                                    if (hole != null)
                                    {
                                        hole.Destroy();
                                        room.Instantiate(new ParticleHole(room, hole));
                                    }
                                }
                            }

                        }
                    }
            }
        }
    }
}
