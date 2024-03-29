﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Player : Entity, ISolid
    {
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public PlayerControl playerControl;
        PlayerControlState playerControlState;
        PlayerControlState playerControlStatePrevious;
        int shot;
        int hurt;
        readonly int hurtTime = 120;

        public Player(Room room, PlayerControl playerControl, Vector2 position, Vector2 velocity)
            : base(room)
        {
            this.playerControl = playerControl;
            this.position = position;
            this.velocity = velocity;

            this.texture = room.map.game.Content.Load<Texture2D>("player");
            this.origin = new Vector2(texture.Width, texture.Height) / 2;

            this.radius = 32;

            if (playerControl.inputType == InputType.Gamepad1) texture = room.map.game.Content.Load<Texture2D>("block");
        }

        protected override void OnDestroy()
        {
            playerControl.player = null;
        }

        public override void Update()
        {
            //hurt
            if (hurt > 0)
                hurt--;
            if (hurt > 0)
                color = ((hurt / 10) % 2 == 0) ? Color.White : Color.Black;

            //shot
            if (shot > 0)
                shot--;

            //movement
            playerControlStatePrevious = playerControlState;
            playerControlState = playerControl.GetPlayerControlState();

            velocity += playerControlState.position * 2f;
            velocity *= .8f;

            Bomb();

            //check doors!
            {
                bool changeRoom = false;
                float distanceSquaredMax = 0;
                foreach (var playerControl2 in room.map.playersControl)
                    if (playerControl2.player != null)
                    {
                        float distanceSquared = (position - playerControl2.player.position).LengthSquared();
                        if (distanceSquared > distanceSquaredMax)
                            distanceSquaredMax = distanceSquared;
                    }
                if (distanceSquaredMax < room.tileSize * 4 * room.tileSize * 4)
                    changeRoom = true;

                //left
                if (position.X < (0 + .5f) * room.tileSize)
                {
                    if (changeRoom && room.left != null)
                        room.map.RoomChange(room.left, position + Vector2.UnitX * (room.tilesWidth - 2) * room.tileSize);
                    else
                    {
                        position.X = (0 + .5f) * room.tileSize;
                        velocity.X = 0;
                    }
                }
                //right
                if (position.X > (room.tilesWidth - .5f) * room.tileSize)
                {
                    if (changeRoom && room.right != null)
                        room.map.RoomChange(room.right, position - Vector2.UnitX * (room.tilesWidth - 2) * room.tileSize);
                    else
                    {
                        position.X = (room.tilesWidth - .5f) * room.tileSize;
                        velocity.X = 0;
                    }
                }
                //up
                if (position.Y < (0 + .5f) * room.tileSize)
                {
                    if (changeRoom && room.up != null)
                        room.map.RoomChange(room.up, position + Vector2.UnitY * (room.tilesHeight - 2) * room.tileSize);
                    else
                    {
                        position.Y = (0 + .5f) * room.tileSize;
                        velocity.Y = 0;
                    }
                }
                //down
                if (position.Y > (room.tilesHeight - .5f) * room.tileSize)
                {
                    if (changeRoom && room.down != null)
                        room.map.RoomChange(room.down, position - Vector2.UnitY * (room.tilesHeight - 2) * room.tileSize);
                    else
                    {
                        position.Y = (room.tilesHeight - .5f) * room.tileSize;
                        velocity.Y = 0;
                    }
                }
            }

            //apply physics!
            var collisions = Collide(new HashSet<Type>(new[]
            {
                typeof(ISolid),
                typeof(Door),
                typeof(LockedDoor),
                typeof(Item),
                typeof(Enemy),
            }), true);
            var enemy = collisions.FirstOrDefault(e => e is Enemy) as Enemy;
            if (enemy != null)
                Health(-enemy.touchDamage);

            position += velocity;

            if (playerControlState.fire.LengthSquared() > .3 * .3)
                if (shot == 0)
                {
                    room.Instantiate(new Bullet(room, position, Vector2.Normalize(playerControlState.fire) * 15 + velocity / 2, playerControl.damage));
                    shot = (int)Math.Pow(1.2, 18 -  playerControl.fireRate);
                }
        }

        public void Health(int healthDelta)
        {
            //hurt?
            if (healthDelta < 0)
            {
                if (hurt > 0)
                    return;

                hurt = hurtTime;

                healthDelta *= room.map.playersControl.Count;
            }

            playerControl.health += healthDelta;

            if (playerControl.health <= 0)
            {
                if (room.map.playersControl.Where(e => e.player != null).Count() <= 1)
                    room.map.game.GameStateChange(new Map(room.map.game, 0));
                else
                    Destroy();
            }

            if (playerControl.health > playerControl.healthMax)
                playerControl.health = playerControl.healthMax;
        }

        private void Bomb()
        {
            if (playerControlState.bomb)
                if (!playerControlStatePrevious.bomb)
                    if (playerControl.bombs > 0)
                    {
                        room.Instantiate(new Bomb(room, position));
                        playerControl.bombs--;
                    }
        }
    }
}
