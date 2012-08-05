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
        public bool hit;

        public Enemy(Room room, Vector2 position, float healthMax)
            : base(room)
        {
            this.position = position;
            this.healthMax = healthMax;
            this.health = healthMax;
        }

        public void Health(float healthDelta)
        {
            health += healthDelta;

            if (healthDelta < 0)
                color = Color.Red;

            if (health < 0)
                Destroy();
            if (health > healthMax)
                health = healthMax;
        }

        public override void Draw()
        {
 	        base.Draw();
            color = Color.White;
        }
    }
}
