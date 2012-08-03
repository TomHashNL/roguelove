using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class DoorOpen : Entity, IDoor
    {
        public DoorOpen(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
            this.texture = room.map.game.Content.Load<Texture2D>("doorOpen");
        }

        protected override void OnDestroy()
        {
            room.Instantiate(new Door(room, position));
        }

        public override void Update()
        {
            if (room.entities.Count(e => e is Enemy) > 0)
                Destroy();
        }
    }
}
