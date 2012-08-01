using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Roguelove
{
    public class Room
    {
        public Map map;
        public HashSet<Entity> entities;
        HashSet<Entity> entitiesAdd;

        public Room(Map map, bool left, bool right, bool up, bool down)
        {
            this.map = map;
            this.entities = new HashSet<Entity>();
            this.entitiesAdd = new HashSet<Entity>();

            //GENERATE ALL THE SHIT OUT OF IT WILMER!!!!!!!! XD

            //Generate a map
            Generate(left, right, up, down);
        }

        private void Generate(bool left, bool right, bool up, bool down)
        {
            foreach (var entity in entities) entity.Destroy();

            int width = 29;
            int height = 15;
            Random rand = new Random();

            bool[,] wall = new bool[width, height];

            //Outline
            for (int x = 0; x < width; x++)
            {
                wall[x, 0] = true;
                wall[x, height - 1] = true;
            }
            for (int y = 0; y < height; y++)
            {
                wall[0, y] = true;
                wall[width - 1, y] = true;
            }
            if (left) wall[0, height / 2] = false;
            if (up) wall[width / 2, 0] = false;
            if (right) wall[width - 1, height / 2] = false;
            if (down) wall[width / 2, height - 1] = false;

            //Random walls
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    if (rand.Next(8) == 0)
                    {
                        wall[x, y] = true;
                        if (rand.Next(2) == 0) wall[x - 1, y] = true;
                        if (rand.Next(2) == 0) wall[x + 1, y] = true;
                        if (rand.Next(2) == 0) wall[x, y - 1] = true;
                        if (rand.Next(2) == 0) wall[x, y + 1] = true;
                        if (rand.Next(4) == 0) wall[x - 1, y - 1] = true;
                        if (rand.Next(4) == 0) wall[x - 1, y + 1] = true;
                        if (rand.Next(4) == 0) wall[x + 1, y - 1] = true;
                        if (rand.Next(4) == 0) wall[x + 1, y + 1] = true;
                    }
                }
            }

            //Translate into tiles
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (wall[x, y]) Instantiate(new Block(this, new Vector2(x * 64 + 32, y * 64 + 120)));
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter)) Generate(true, true, true, true);

            //Stuff
            foreach (var entity in entities)
                entity.Update();

            entities.RemoveWhere(e => !e.alive);

            foreach (var entity in entitiesAdd)
                entities.Add(entity);
            entitiesAdd.Clear();
        }

        public void Instantiate(Entity entity)
        {
            entitiesAdd.Add(entity);
        }

        public void Draw()
        {
            //DO SOME TRANSFORM ON THIS LATER!!!
            map.game.spriteBatch.Begin();

            foreach (var entity in entities)
                entity.Draw();

            map.game.spriteBatch.End();
        }
    }
}
