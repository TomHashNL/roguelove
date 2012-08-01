using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        }

        protected override void OnDestroy()
        {
            playerControl.player = null;
        }

        public override void Update()
        {
            var playerControlState = playerControl.GetPlayerControlState();
            //use it here for input! =)

            position += velocity;
        }
    }
}
