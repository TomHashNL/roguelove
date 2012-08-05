using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class LockedDoor : Entity, ISolid, IDoor
    {
        public LockedDoor(Room room, Vector2 position, float rotation)
            : base(room)
        {
            this.position = position;
            this.rotation = rotation;
            this.texture = room.map.game.Content.Load<Texture2D>("lockedDoor");
        }

        protected override void OnDestroy()
        {
            room.Instantiate(new DoorOpen(room, position, rotation));
        }

        public override void Update()
        {
            if (room.clear)
            {
                //get a player who is colliding with more than 0 keys
                var collision = Collide(new HashSet<Type>(new[] { typeof(Player), }), false)
                    .FirstOrDefault(e => (e as Player).playerControl.keys > 0) as Player;

                if (collision != null)//player collision
                {
                    collision.playerControl.keys--;
                    Destroy();
                }
            }
        }

        public override void Draw()
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            room.map.game.spriteBatch.Draw(texture, position + origin, sourceRectangle, color, rotation, origin, scale, spriteEffects, layerDepth);
        }
    }
}
