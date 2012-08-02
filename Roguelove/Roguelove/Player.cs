using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Player : Entity
    {
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public PlayerControl playerControl;
        Vector2 velocity;

        public Player(Room room, PlayerControl playerControl, Vector2 position, Vector2 velocity)
            : base(room)
        {
            this.playerControl = playerControl;
            this.position = position;
            this.velocity = velocity;

            this.texture = room.map.game.Content.Load<Texture2D>("player");
            this.origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        protected override void OnDestroy()
        {
            playerControl.player = null;
        }

        public override void Update()
        {
            var playerControlState = playerControl.GetPlayerControlState();

            velocity += playerControlState.position * 2f;
            velocity *= .9f;

            position += velocity;

            if (playerControlState.fire.LengthSquared() > .3 * .3)
                room.Instantiate(new Bullet(room, position, Vector2.Normalize(playerControlState.fire) * 15 + velocity / 2));
        }
    }
}
