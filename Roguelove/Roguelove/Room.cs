using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Room
    {
        public Map map;
        public HashSet<Entity> entities;
        HashSet<Entity> entitiesAdd;
        public readonly int tileSize = 64;
        public readonly int HUDheight = 120;
        public readonly int tilesWidth = 29;
        public readonly int tilesHeight = 15;
        public readonly int viewWidth = 1920;
        public readonly int viewHeight = 1080;

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
            //cleanup shit
            foreach (var entity in entities)
                entity.Destroy();
            
            //yay go!
            Random rand = new Random();

            bool[,] wall = new bool[tilesWidth, tilesHeight];

            //Outline
            for (int x = 0; x < tilesWidth; x++)
            {
                wall[x, 0] = true;
                wall[x, tilesHeight - 1] = true;
            }
            for (int y = 0; y < tilesHeight; y++)
            {
                wall[0, y] = true;
                wall[tilesWidth - 1, y] = true;
            }
            if (left) wall[0, tilesHeight / 2] = false;
            if (up) wall[tilesWidth / 2, 0] = false;
            if (right) wall[tilesWidth - 1, tilesHeight / 2] = false;
            if (down) wall[tilesWidth / 2, tilesHeight - 1] = false;

            //Random walls
            for (int x = 1; x < tilesWidth - 1; x++)
            {
                for (int y = 1; y < tilesHeight - 1; y++)
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
            for (int x = 0; x < tilesWidth; x++)
                for (int y = 0; y < tilesHeight; y++)
                    if (wall[x, y])
                        Instantiate(new Block(this, new Vector2(x * tileSize, y * tileSize)));
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Enter))
                Generate(true, true, true, true);

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
            Vector2 screen = new Vector2(
                map.game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                map.game.GraphicsDevice.PresentationParameters.BackBufferHeight);

            Matrix matrix;
            Vector2 offset;
            float scale;
            if (screen.X / screen.Y > (float)viewWidth / (float)viewHeight)
            {
                offset = new Vector2((viewHeight * (screen.X / screen.Y) - viewWidth) / 2, 0);
                scale = screen.Y / viewHeight;
            }
            else
            {
                offset = new Vector2(0, (viewWidth / (screen.X / screen.Y) - viewHeight) / 2);
                scale = screen.X / viewWidth;
            }

            //GAME SCREEN
            matrix = Matrix.CreateTranslation(new Vector3(offset, 0))
                * Matrix.CreateTranslation((viewWidth - tilesWidth * tileSize) / 2, HUDheight, 0)
                * Matrix.CreateScale(scale);

            map.game.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

            foreach (var entity in entities)
                entity.Draw();

            map.game.spriteBatch.End();

            //HUD!!!
            matrix = Matrix.CreateTranslation(new Vector3(offset, 0))
                * Matrix.CreateScale(scale);

            map.game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, matrix);

            map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("block"), new Rectangle(0, 0, viewWidth, HUDheight), Color.Red);

            map.game.spriteBatch.End();
        }
    }
}
