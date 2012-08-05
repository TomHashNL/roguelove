using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public abstract class Particle : Entity
    {
        public Particle(Room room, Vector2 position)
            : base(room)
        {
            this.position = position;
        }
    }
}
