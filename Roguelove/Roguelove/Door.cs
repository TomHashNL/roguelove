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
        public Door(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.texture = room.map.game.Content.Load<Texture2D>("door");
        }

        protected override void OnDestroy()
        {
            room.Instantiate(new DoorOpen(room, position));
        }

        public override void Update()
        {
            if (room.entities.Count(e => e is Enemy) == 0)
                Destroy();
        }
    }
}
