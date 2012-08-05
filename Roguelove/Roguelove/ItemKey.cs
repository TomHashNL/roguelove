using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class ItemKey : Item
    {
        public ItemKey(Room room, Vector2 position, bool velocity)
            : base(room, position, velocity)
        {
            texture = room.map.game.Content.Load<Texture2D>("itemKey");

            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Pickup(Player player)
        {
            if (player.playerControl.keys < player.playerControl.keysMax)
            {
                player.playerControl.keys++;
                if (player.playerControl.keys > player.playerControl.keysMax)
                    player.playerControl.keys = player.playerControl.keysMax;

                //play sounds
                //effect!

                Destroy();
            }
        }

        protected override void OnDestroy()
        {

        }
    }
}
