using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public enum ItemBombType
    {
        One,
        Two,
    }

    public class ItemBomb : Item, ISolid
    {
        int value;

        public ItemBomb(Room room, Vector2 position, bool velocity, ItemBombType itemBombType)
            : base(room, position, velocity)
        {
            switch (itemBombType)
            {
                case ItemBombType.One:
                    value = 1;
                    texture = room.map.game.Content.Load<Texture2D>("itemBombOne");
                    break;
                case ItemBombType.Two:
                    value = 2;
                    texture = room.map.game.Content.Load<Texture2D>("itemBombTwo");
                    break;
                default:
                    throw new NotImplementedException("ItemBombType not supported!");
            }

            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Pickup(Player player)
        {
            if (player.playerControl.bombs < player.playerControl.bombsMax)
            {
                player.playerControl.bombs += value;
                if (player.playerControl.bombs > player.playerControl.bombsMax)
                    player.playerControl.bombs = player.playerControl.bombsMax;

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
