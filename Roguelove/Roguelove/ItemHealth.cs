using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public enum ItemHealthType
    {
        One,
        Two,
    }

    public class ItemHealth : Item
    {
        int value;

        public ItemHealth(Room room, Vector2 position, ItemHealthType itemHealthType)
            : base(room, position)
        {
            switch (itemHealthType)
            {
                case ItemHealthType.One:
                    value = 1;
                    texture = room.map.game.Content.Load<Texture2D>("itemHealthOne");
                    break;
                case ItemHealthType.Two:
                    value = 2;
                    texture = room.map.game.Content.Load<Texture2D>("itemHealthTwo");
                    break;
                default:
                    throw new NotImplementedException("ItemHealthType not supported!");
            }

            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Pickup(Player player)
        {
            if (player.playerControl.health < player.playerControl.healthMax)
            {
                player.playerControl.healthMax += value;
                if (player.playerControl.health > player.playerControl.healthMax)
                    player.playerControl.health = player.playerControl.healthMax;

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
