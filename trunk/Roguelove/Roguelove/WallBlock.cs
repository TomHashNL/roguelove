using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class WallBlock : Entity
    {
        public WallBlock(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("block");
            this.position = position;
            color = Color.Gray;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {

        }
    }
}
