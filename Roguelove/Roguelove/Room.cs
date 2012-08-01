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
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Map map;
        /// <summary>
        /// DO NOT SET/ADJUST
        /// </summary>
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

        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Room left;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Room right;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Room up;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Room down;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public RoomType roomType;

        public Room(Map map)
        {
            this.map = map;
            this.entities = new HashSet<Entity>();
            this.entitiesAdd = new HashSet<Entity>();
        }

        public void Generate()
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
                Generate();
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
            Vector2 menuOffset = new Vector2((viewWidth - tilesWidth * tileSize) / 2, HUDheight);
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
            {
                matrix = Matrix.CreateTranslation(new Vector3(offset, 0))
                    * Matrix.CreateTranslation(new Vector3(menuOffset, 0))
                    * Matrix.CreateScale(scale);

                map.game.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, matrix);

                //draw background
                Rectangle rectangleBackground = new Rectangle((int)(-offset.X - menuOffset.X), (int)(-offset.Y - menuOffset.Y), (int)(screen.X / scale), (int)(screen.Y / scale));
                map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("background"), rectangleBackground, rectangleBackground, Color.White);

                //draw all entities
                foreach (var entity in entities)
                    entity.Draw();

                map.game.spriteBatch.End();
            }

            //DRAW THE HUD!!!
            {
                matrix = Matrix.CreateTranslation(new Vector3(offset, 0))
                    * Matrix.CreateScale(scale);

                map.game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, matrix);

                //draw rooms
                //map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("block"), new Rectangle(0, 0, viewWidth, HUDheight), Color.Red);
                {
                    Dictionary<Vector2, Room> rooms = new Dictionary<Vector2, Room>();
                    HashSet<Vector2> used = new HashSet<Vector2>();
                    rooms.Add(new Vector2(500), this);
                    while (rooms.Count > 0)
                    {
                        var room = rooms.First();
                        rooms.Remove(room.Key);
                        used.Add(room.Key);

                        if (room.Value.left != null)
                        {//left
                            Vector2 key = room.Key - Vector2.UnitX * tileSize;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.left);
                        }
                        if (room.Value.right != null)
                        {//right
                            Vector2 key = room.Key + Vector2.UnitX * tileSize;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.right);
                        }
                        if (room.Value.up != null)
                        {//up
                            Vector2 key = room.Key - Vector2.UnitY * tileSize;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.up);
                        }
                        if (room.Value.down != null)
                        {//down
                            Vector2 key = room.Key + Vector2.UnitY * tileSize;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.down);
                        }
                        Color color;
                        switch (room.Value.roomType)
                        {
                            case RoomType.Enemy:
                                color = Color.Orange;
                                break;
                            case RoomType.Start:
                                color = Color.Green;
                                break;
                            case RoomType.Boss:
                                color = Color.Red;
                                break;
                            default:
                                throw new Exception("WTFBLARGH");
                        }
                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("block"), room.Key, color);
                    }
                }

                map.game.spriteBatch.End();
            }
        }
    }
}
