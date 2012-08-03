using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public abstract class Entity
    {
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Room room;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public bool alive;
        public Texture2D texture;
        public Vector2 position;
        public Vector2 scale;
        public float rotation;
        public Rectangle? sourceRectangle;
        public SpriteEffects spriteEffects;
        public Color color;
        public Vector2 origin;
        public float layerDepth;
        public bool visible;
        public float radius;
        public Vector2 velocity;
        public bool collidable;

        public Entity(Room room)
        {
            this.room = room;
            this.alive = true;
            this.visible = true;
            this.scale = Vector2.One;
            this.color = Color.White;
            this.radius = room.tileSize / 2;
            this.collidable = true;
        }

        protected abstract void OnDestroy();

        public void Destroy()
        {
            if (alive)
                OnDestroy();
            alive = false;
        }

        public abstract void Update();

        /// <summary>
        /// Standard implementation of drawing le sprite!
        /// </summary>
        public virtual void Draw()
        {
            if (visible)
                if (texture != null)
                    room.map.game.spriteBatch.Draw(
                        texture,
                        position,
                        sourceRectangle,
                        color,
                        rotation,
                        origin,
                        scale,
                        spriteEffects,
                        layerDepth);
        }

        public HashSet<Entity> Collide(HashSet<Type> types, bool solid)
        {
            HashSet<Entity> collisions = new HashSet<Entity>();

            if (collidable)
                foreach (var entity in room.entities)
                    if (entity != this)
                        if (entity.collidable)
                            if (types.FirstOrDefault(e => e.IsAssignableFrom(entity.GetType())) != null)
                            {
                                Vector2 center = entity.position + new Vector2(room.tileSize / 2) - entity.origin;
                                if (position.X + radius > center.X - entity.radius)
                                    if (position.X - radius < center.X + entity.radius)
                                        if (position.Y + radius > center.Y - entity.radius)
                                            if (position.Y - radius < center.Y + entity.radius)
                                            {
                                                float distance = (position - center).Length();
                                                float distanceCollision = radius + entity.radius;

                                                if (distance < distanceCollision)
                                                {
                                                    collisions.Add(entity);

                                                    if (solid)
                                                    {
                                                        double direction = Math.Atan2(position.Y - center.Y, position.X - center.X);

                                                        Vector2 blockVector = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));
                                                        Vector2 force = blockVector * (distanceCollision - distance) / 2;
                                                        if (force.Length() > 5)
                                                            force = Vector2.Normalize(force) * 5;
                                                        velocity += force;

                                                        entity.velocity *= .8f;
                                                    }
                                                }
                                            }
                            }

            return collisions;
        }
    }
}
