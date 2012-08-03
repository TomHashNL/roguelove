using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Block : Entity, ISolid
    {
        public Block(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("block");
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
