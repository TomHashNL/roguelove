using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Chest : Entity, ISolid
    {
        public bool open;

        public Chest(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("chest");
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
                    if (entity.GetType() == typeof(Player))
                        Open(entity as Player);

            position += velocity;
        }

        private void Open(Player player)
        {
            open = true;
            texture = room.map.game.Content.Load<Texture2D>("chestOpen");

            //instantiate shit ;D
            room.Instantiate(new ItemMoney(room, position, ItemMoneyType.One));
            room.Instantiate(new ItemMoney(room, position, ItemMoneyType.One));
            room.Instantiate(new ItemBomb(room, position, ItemBombType.One));
            room.Instantiate(new ItemHealth(room, position, ItemHealthType.One));
            room.Instantiate(new ItemKey(room, position));
        }
    }
}
