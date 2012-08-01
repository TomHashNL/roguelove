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
        public int lifeMax;
        public int life;
        public int money;
        public int bombs;
        public int keys;
        public float damage;
        public float fireRate;
        public int index;
        InputType inputType;

        public PlayerControl(int index, InputType inputType)
        {
            this.index = index;
            this.lifeMax = 3;
            this.life = 3;
            this.bombs = 1;
            this.damage = 1;
            this.fireRate = 1;
        }

        public PlayerControlState GetPlayerControlState()
        {
            //implement switch here for all types of input =O!!!
            if (inputType == InputType.Keyboard)
            {
                //var keyboardState = Keyboard.GetState();
                //GET A GOOD STATE HERE!!!
                return new PlayerControlState();
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
                }

                //var gamePadState = GamePad.GetState(playerIndex);
                //GET A GOOD STATE HERE!!!
                return new PlayerControlState();
            }
        }
    }
}
