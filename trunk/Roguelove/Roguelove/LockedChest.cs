using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class LockedChest : Entity, ISolid
    {
        public bool open;

        public LockedChest(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("lockedChest");
            this.position = position;
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {
            velocity *= .9f;

            var collisions = Collide(new HashSet<Type>(new[]
            {
                typeof(ISolid),
                typeof(IDoor),
            }), true);

            if (!open)
                foreach (var entity in collisions)
                {
                    var player = entity as Player;
                    if (player != null)
                        if (player.playerControl.keys > 0)
                        {
                            player.playerControl.keys--;
                            Open(player);
                        }
                }

            position += velocity;
        }

        private void Open(Player player)
        {
            open = true;
            texture = room.map.game.Content.Load<Texture2D>("lockedChestOpen");

            //instantiate shit ;D
            room.Instantiate(new ItemMoney(room, position, true, ItemMoneyType.Five));
            room.Instantiate(new ItemMoney(room, position, true, ItemMoneyType.Ten));
            room.Instantiate(new ItemMoney(room, position, true, ItemMoneyType.Twentyfive));
            room.Instantiate(new ItemBomb(room, position, true, ItemBombType.Two));
            room.Instantiate(new ItemHealth(room, position, true, ItemHealthType.Two));
        }
    }
}
