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
        Twentyfive,
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
                    texture = room.map.game.Content.Load<Texture2D>("itemMoneyOne");
                    break;
                case ItemMoneyType.Five:
                    value = 5;
                    texture = room.map.game.Content.Load<Texture2D>("itemMoneyFive");
                    break;
                case ItemMoneyType.Ten:
                    value = 10;
                    texture = room.map.game.Content.Load<Texture2D>("itemMoneyTen");
                    break;
                case ItemMoneyType.Twentyfive:
                    value = 25;
                    texture = room.map.game.Content.Load<Texture2D>("itemMoneyTwentyfive");
                    break;
                default:
                    throw new NotImplementedException("ItemMoneyType not working?!");
            }

            origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Pickup(Player player)
        {
            if (player.playerControl.money < player.playerControl.moneyMax)
            {
                player.playerControl.money += value;
                if (player.playerControl.money > player.playerControl.moneyMax)
                    player.playerControl.money = player.playerControl.moneyMax;

                //play sounds?
                //play effect ;D

                Destroy();
            }
        }

        protected override void OnDestroy()
        {
            
        }
    }
}
