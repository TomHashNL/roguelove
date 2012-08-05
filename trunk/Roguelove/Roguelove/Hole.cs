using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Hole : Entity, ISolid
    {
        Texture2D edgeHorTexture;
        Texture2D edgeVertTexture;
        Texture2D edgeWallTexture;
        bool[] solid;

        public Hole(Room room, Vector2 position)
            : base(room)
        {
            texture = room.map.game.Content.Load<Texture2D>("holeBase");
            edgeHorTexture = room.map.game.Content.Load<Texture2D>("holeHorEdge");
            edgeVertTexture = room.map.game.Content.Load<Texture2D>("holeVertEdge");
            edgeWallTexture = room.map.game.Content.Load<Texture2D>("holeEdgeWall");
            this.position = position;

            this.layerDepth = 1;

            solid = new bool[4];
        }

        public void autoTile(Entity[,] grid)
        {
            solid = new bool[4];

            int x = (int)position.X / room.tileSize;
            int y = (int)position.Y / room.tileSize;

            if (!(grid[x - 1, y] is Hole))
                solid[0] = true;
            if (!(grid[x, y - 1] is Hole))
                solid[1] = true;
            if (!(grid[x + 1, y] is Hole))
                solid[2] = true;
            if (!(grid[x, y + 1] is Hole))
                solid[3] = true;
        }

        protected override void OnDestroy()
        {

        }

        public override void Update()
        {

        }

        public override void Draw()
        {
            base.Draw();

            Color col = Color.Gray;

            if (solid[1]) room.map.game.spriteBatch.Draw(edgeWallTexture, position, Color.White);

            if (solid[0]) room.map.game.spriteBatch.Draw(edgeVertTexture, position + new Vector2(-32, 0), col);
            if (solid[1]) room.map.game.spriteBatch.Draw(edgeHorTexture, position + new Vector2(0, -32), col);
            if (solid[2]) room.map.game.spriteBatch.Draw(edgeVertTexture, position + new Vector2(32, 0), col);
            if (solid[3]) room.map.game.spriteBatch.Draw(edgeHorTexture, position + new Vector2(0, 32), col);
        }
    }
}
