using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public class PlayerControl
    {
        public Player player;
        public int healthMax;
        public int health;
        public int moneyMax;
        public int money;
        public int bombsMax;
        public int bombs;
        public int keysMax;
        public int keys;
        public float damage;
        public float fireRate;
        public int index;
        public InputType inputType;

        public PlayerControl(int index, InputType inputType)
        {
            this.index = index;
            this.inputType = inputType;
            this.healthMax = 6;
            this.health = this.healthMax;
            this.bombsMax = 99;
            this.bombs = 1;
            this.moneyMax = 99;
            this.money = 0;
            this.keysMax = 99;
            this.keys = 0;
            this.damage = 1;
            this.fireRate = 1;
        }

        public PlayerControlState GetPlayerControlState()
        {
            //implement switch here for all types of input =O!!!
            if (inputType == InputType.Keyboard)
            {
                var keyboardState = Keyboard.GetState();
                var mouseState = Mouse.GetState();

                PlayerControlState playerControlState = new PlayerControlState();
                if (keyboardState.IsKeyDown(Keys.A))
                    playerControlState.position.X -= 1;
                if (keyboardState.IsKeyDown(Keys.D))
                    playerControlState.position.X += 1;
                if (keyboardState.IsKeyDown(Keys.W))
                    playerControlState.position.Y -= 1;
                if (keyboardState.IsKeyDown(Keys.S))
                    playerControlState.position.Y += 1;

                if (playerControlState.position.LengthSquared() > 1)
                    playerControlState.position.Normalize();

                if (keyboardState.IsKeyDown(Keys.Left))
                    playerControlState.fire.X -= 1;
                if (keyboardState.IsKeyDown(Keys.Right))
                    playerControlState.fire.X += 1;
                if (keyboardState.IsKeyDown(Keys.Up))
                    playerControlState.fire.Y -= 1;
                if (keyboardState.IsKeyDown(Keys.Down))
                    playerControlState.fire.Y += 1;

                if (mouseState.LeftButton == ButtonState.Pressed)
                    if (player != null)
                    {
                        Vector2 offset = new Vector2(mouseState.X, mouseState.Y) - Vector2.Transform(player.position, player.room.matrix);
                        if (offset == Vector2.Zero)
                            offset = Vector2.UnitX;
                        playerControlState.fire += Vector2.Normalize(offset);
                    }

                playerControlState.item = keyboardState.IsKeyDown(Keys.Space);
                playerControlState.bomb = keyboardState.IsKeyDown(Keys.E);

                return playerControlState;
            }
            else
            {
                PlayerIndex playerIndex;
                switch (inputType)
                {
                    case InputType.Gamepad1:
                        playerIndex = PlayerIndex.One;
                        break;
                    case InputType.Gamepad2:
                        playerIndex = PlayerIndex.Two;
                        break;
                    case InputType.Gamepad3:
                        playerIndex = PlayerIndex.Three;
                        break;
                    case InputType.Gamepad4:
                        playerIndex = PlayerIndex.Four;
                        break;
                    default:
                        throw new Exception("What in the actual fuck!");
                }

                var gamePadState = GamePad.GetState(playerIndex, GamePadDeadZone.Circular);

                PlayerControlState playerControlState = new PlayerControlState();
                playerControlState.position = gamePadState.ThumbSticks.Left;
                playerControlState.position.Y *= -1;
                playerControlState.fire = gamePadState.ThumbSticks.Right;
                playerControlState.fire.Y *= -1;
                playerControlState.item = gamePadState.Buttons.A == ButtonState.Pressed;
                playerControlState.bomb = gamePadState.Buttons.B == ButtonState.Pressed;

                return playerControlState;
            }
        }
    }
}
