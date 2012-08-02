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
            //movement
            var playerControlState = playerControl.GetPlayerControlState();

            velocity += playerControlState.position * 2f;
            velocity *= .9f;

            //foreach (var entity in room.entities)
            //{
            //    Type type = entity.GetType();
            //    if (type == typeof(Block) ||
            //        type == typeof(Hole))
            //    {
            //        if ((position - entity.position + new Vector2(32)).Length() < 64)
            //            velocity *= -1;
            //    }

            //    //if (position + velocity)
            //}

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
                if (distanceSquaredMax < room.tileSize * room.tileSize)
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
            position += velocity;

            if (playerControlState.fire.LengthSquared() > .3 * .3)
                room.Instantiate(new Bullet(room, position, Vector2.Normalize(playerControlState.fire) * 15 + velocity / 2));
        }
    }
}
