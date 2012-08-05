using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class ParticleHole : Particle
    {
        Hole hole;

        public ParticleHole(Room room, Hole hole)
            : base(room, hole.position)
        {
            this.hole = hole;
            this.texture = room.map.game.Content.Load<Texture2D>("particleHole");

            this.layerDepth = 1;
        }

        protected override void OnDestroy()
        {
            
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {
            hole.Draw();
            base.Draw();
        }
    }
}
