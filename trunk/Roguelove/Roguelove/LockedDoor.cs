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
        public LockedDoor(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.texture = room.map.game.Content.Load<Texture2D>("lockedDoor");
        }

        protected override void OnDestroy()
        {
            room.Instantiate(new DoorOpen(room, position));
        }

        public override void Update()
        {
            if (room.entities.Count(e => e is Enemy) == 0)
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
    }
}
