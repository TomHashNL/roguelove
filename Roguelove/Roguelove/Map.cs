using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelove
{
    public class Map : GameState
    {
        public int floor;
        Room room;

        public Map(Game1 game, int floor)
            : base(game)
        {
            this.floor = floor;

            this.room = new Room(this, false, false, false, false);
            //algorithm to generate all the rooms on this floor!!!!
            //generate the map!!!!
            //then generate all the rooms in it ;D
        }

        public override void Update()
        {
            room.Update();
        }

        public override void Draw()
        {
            room.Draw();
        }
    }
}
