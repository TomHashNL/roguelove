using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Roguelove
{
    public class Map : GameState
    {
        public int floor;
        Room room;
        List<PlayerControl> playersControl;

        public Map(Game1 game, List<PlayerControl> playersControl, int floor)
            : base(game)
        {
            this.playersControl = playersControl;
            this.floor = floor;

            var room = new Room(this, false, false, false, false);
            //algorithm to generate all the rooms on this floor!!!!
            //generate the map!!!!
            //then generate all the rooms in it ;D

            RoomChange(room);
        }

        public void RoomChange(Room room)
        {
            this.room = room;

            //place all the players in the current map =o
            foreach (var playerControl in playersControl)
            {
                //FIND THE MIDDLE OF THE ROOM HERE TO PLACE PLAYERS???
                playerControl.player = new Player(this.room, playerControl, Vector2.Zero, Vector2.Zero);
                this.room.Instantiate(playerControl.player);
            }
        }

        public void FloorAdvance()
        {
            game.GameStateChange(new Map(game, playersControl, floor + 1));
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            room.Update();
        }

        public override void Draw()
        {
            room.Draw();

            //DRAW THE HUD HERE
            //game.spriteBatch.Begin();
            ////BLARHGH
            //game.spriteBatch.End();
        }
    }
}
