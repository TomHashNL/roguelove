using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Door : Entity, ISolid, IDoor
    {
        public Door(Room room, Vector2 position, float rotation)
            : base(room)
        {
            this.position = position;
            this.rotation = rotation;
            this.texture = room.map.game.Content.Load<Texture2D>("door");
        }

        protected override void OnDestroy()
        {
            room.Instantiate(new DoorOpen(room, position, rotation));
        }

        public override void Update()
        {
            if (room.entities.Count(e => e is Enemy) == 0)
                Destroy();
        }

        public override void Draw()
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            room.map.game.spriteBatch.Draw(texture, position + origin, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);
        }
    }
}
