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

        public readonly double blockRate = 0.125;
        public readonly double blockEdgeRate = 0.5;
        public readonly double blockCornerRate = 0.125;

        public Room(Map map)
        {
            this.map = map;
            this.entities = new HashSet<Entity>();
            this.entitiesAdd = new HashSet<Entity>();

        }

        public void Generate(Room left, Room right, Room up, Room down)
        {
            //cleanup shit
            foreach (var entity in entities)
                entity.Destroy();

            //yay go!
            Random rand = new Random();
            bool[,] wall = new bool[tilesWidth, tilesHeight]; ;

            //Main generation loop
            bool done = false;
            while (!done)
            {
                wall = new bool[tilesWidth, tilesHeight];

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

                //Random walls
                for (int x = 1; x < tilesWidth - 1; x++)
                {
                    for (int y = 1; y < tilesHeight - 1; y++)
                    {
                        if (rand.NextDouble() < blockRate)
                        {
                            wall[x, y] = true;
                            if (rand.NextDouble() < blockEdgeRate) wall[x - 1, y] = true;
                            if (rand.NextDouble() < blockEdgeRate) wall[x + 1, y] = true;
                            if (rand.NextDouble() < blockEdgeRate) wall[x, y - 1] = true;
                            if (rand.NextDouble() < blockEdgeRate) wall[x, y + 1] = true;
                            if (rand.NextDouble() < blockCornerRate) wall[x - 1, y - 1] = true;
                            if (rand.NextDouble() < blockCornerRate) wall[x - 1, y + 1] = true;
                            if (rand.NextDouble() < blockCornerRate) wall[x + 1, y - 1] = true;
                            if (rand.NextDouble() < blockCornerRate) wall[x + 1, y + 1] = true;
                        }
                    }
                }

                //Poke door holes in map
                if (left != null) wall[0, tilesHeight / 2] = false;
                if (up != null) wall[tilesWidth / 2, 0] = false;
                if (right != null) wall[tilesWidth - 1, tilesHeight / 2] = false;
                if (down != null) wall[tilesWidth / 2, tilesHeight - 1] = false;

                //Check if accessible between open doors
                bool[,] visited = new bool[tilesWidth, tilesHeight];
                List<Point> newPoints = new List<Point>();

                Point startPoint;
                if (left != null) startPoint = new Point(0, tilesHeight / 2);
                else if (up != null) startPoint = new Point(tilesWidth / 2, 0);
                else if (right != null) startPoint = new Point(tilesWidth / 2, tilesHeight / 2);
                else if (down != null) startPoint = new Point(tilesWidth / 2, tilesHeight - 1);
                else throw new Exception("Failed to generate map, no open doors, derp :3");

                visited[startPoint.X, startPoint.Y] = true;
                newPoints.Add(startPoint);

                while (newPoints.Count != 0)
                {
                    for (int i = 0; i < newPoints.Count; i++)
                    {
                        Point point = newPoints[i];
                        Point[] points = new Point[4] {new Point(point.X - 1, point.Y),
                                            new Point(point.X, point.Y - 1),
                                            new Point(point.X + 1, point.Y),
                                            new Point(point.X, point.Y + 1)};

                        bool endPoint = true;
                        foreach (Point p in points)
                        {
                            if (p.X >= 0 && p.Y >= 0 && p.X < tilesWidth && p.Y < tilesHeight && !visited[p.X, p.Y] && !wall[p.X, p.Y])
                            {
                                newPoints.Add(new Point(p.X, p.Y));
                                visited[p.X, p.Y] = true;
                                endPoint = false;
                            }
                        }

                        if (endPoint)
                        {
                            newPoints.RemoveAt(i);
                            i--;
                        }
                    }
                }

                //Check if everything pwns
                done = true;
                if (left != null && !visited[0, tilesHeight / 2]) done = false;
                if (up != null && !visited[tilesWidth / 2, 0]) done = false;
                if (right != null && !visited[tilesWidth - 1, tilesHeight / 2]) done = false;
                if (down != null && !visited[tilesWidth / 2, tilesHeight - 1]) done = false;

                //bool debugLeft = visited[0, tilesHeight / 2];
                //bool debugUp = visited[tilesWidth / 2, 0];
                //bool debugRight = visited[tilesWidth - 1, tilesHeight / 2];
                //bool debugDown = visited[tilesWidth / 2, tilesHeight - 1];
                //done = true;
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
                Generate(this, this, this, this);

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
