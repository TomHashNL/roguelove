using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roguelove
{
    public abstract class GameState
    {
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Game1 game;

        public GameState(Game1 game)
        {
            this.game = game;
        }

        public abstract void Update();

        public abstract void Draw();
    }
}
