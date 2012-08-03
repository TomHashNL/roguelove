using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public enum ItemMoneyType
    {
        One,
        Five,
        Ten,
        TwentyFive,
    }

    public class ItemMoney : Item
    {
        public int value;

        public ItemMoney(Room room, Vector2 position, ItemMoneyType itemMoneyType)
            : base(room, position)
        {
            switch (itemMoneyType)
            {
                case ItemMoneyType.One:
                    value = 1;
                    texture = room.map.game.Content.Load<Texture2D>("player");
                    break;
                case ItemMoneyType.Five:
                    value = 5;
                    texture = room.map.game.Content.Load<Texture2D>("player");
                    break;
                case ItemMoneyType.Ten:
                    value = 10;
                    texture = room.map.game.Content.Load<Texture2D>("player");
                    break;
                case ItemMoneyType.TwentyFive:
                    value = 25;
                    texture = room.map.game.Content.Load<Texture2D>("player");
                    break;
                default:
                    throw new NotImplementedException("ItemMoneyType not working?!");
            }
            origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Pickup(Player player)
        {
            player.playerControl.money += value;
            //play sounds?
            //play effect ;D
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
