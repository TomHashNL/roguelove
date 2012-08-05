using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public class Hatch : Entity
    {
        public Hatch(Room room)
            : base(room)
        {
            this.position = new Vector2(room.tilesWidth / 2, room.tilesHeight / 4) * room.tileSize;
            this.texture = room.map.game.Content.Load<Texture2D>("player");
        }

        protected override void OnDestroy()
        {
            
        }

        public override void Update()
        {
            if (Collide(new HashSet<Type>(new[]
            {
                typeof(Player),
            }), false).Count > 0)
                if (room.entities.Where(e => e is Player).All(e => (e.position - (position + new Vector2(room.tileSize / 2))).LengthSquared() < (room.tileSize * 1.5f) * (room.tileSize * 1.5f)))
                    room.map.FloorAdvance();
        }
    }
}
