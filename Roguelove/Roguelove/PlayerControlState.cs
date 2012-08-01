using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public struct PlayerControlState
    {
        /// <summary>
        /// NORMALIZED if length > 1
        /// </summary>
        public Vector2 position;
        /// <summary>
        /// NORMALIZED if length > 1
        /// </summary>
        public Vector2 fire;
        public bool item;
        public bool bomb;
    }
}
