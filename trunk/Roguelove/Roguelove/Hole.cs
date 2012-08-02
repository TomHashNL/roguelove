using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Hole : Entity
    {
        public Hole(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("hole");
            this.position = position;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {

        }
    }
}
