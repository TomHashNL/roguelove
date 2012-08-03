using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Roguelove
{
    public abstract class Enemy : Entity
    {
        public float health;
        public float healthMax;

        public Enemy(Room room, Vector2 position, float healthMax)
            : base(room)
        {
            this.position = position;
            this.healthMax = healthMax;
            health = healthMax;
        }

        public void Health(float healthDelta)
        {
            health += healthDelta;

            if (health < 0)
                Destroy();
            if (health > healthMax)
                health = healthMax;
        }
    }
}
