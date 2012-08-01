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

        public Entity(Room room)
        {
            this.room = room;
            this.alive = true;
            this.visible = true;
            this.scale = Vector2.One;
            this.color = Color.White;
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
    }
}
