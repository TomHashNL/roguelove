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
        public int touchDamage;

        public Enemy(Room room, Vector2 position, float healthMax, int touchDamage)
            : base(room)
        {
            healthMax *= 1 + .3f * room.map.floor;
            touchDamage = (int)(touchDamage * (1 + .2f * room.map.floor));

            this.position = position;
            this.healthMax = healthMax;
            this.health = healthMax;
            this.touchDamage = touchDamage;
        }

        public void Health(float healthDelta)
        {
            if (healthDelta < 0)
            {
                color = Color.Red;

                healthDelta /= room.map.playersControl.Count;
            }

            health += healthDelta;

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
