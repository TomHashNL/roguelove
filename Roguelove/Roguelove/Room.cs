﻿using System;
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
        public readonly int HUDheight = 248;
        public readonly int tilesWidth = 23;
        public readonly int tilesHeight = 13;
        public readonly int viewWidth = 1472;
        public readonly int viewHeight = 1080;

        public readonly double blockRate = 0.0625;
        public readonly double blockEdgeRate = 0.5;
        public readonly double blockCornerRate = 0.125;

        public readonly double blockHoleRate = 0.0625;
        public readonly double holeEdgeRate = 0.5;
        public readonly double holeCornerRate = 0.125;
        public readonly int minHoleSpreadCount = 4;
        public readonly int maxHoleSpreadCount = 200;
        public readonly int maxHoleBruteForceTries = 100;

        public readonly double chestRate = 0.25;
        public readonly double chestLockedRate = 0.25;

        public readonly double minimumAccessibility = 0.25;

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
        public bool visited;
        /// <summary>
        /// DO NOT SET
        /// </summary>
        public Matrix matrix;

        /// <summary>
        /// DO NOT SET
        /// </summary>
        public bool clear;

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
            Random random = map.game.random;
            Entity[,] grid = new Entity[tilesWidth, tilesHeight];

            switch (roomType)
            {
                case (RoomType.Start):
                    grid = GenerateStartRoom(random);
                    break;
                case (RoomType.Boss):
                    grid = GenerateBossRoom(random);
                    break;
                case (RoomType.Enemy):
                    grid = GenerateEnemyRoom(random);
                    break;
                default:
                    throw new NotImplementedException("RoomType not implemented yet!");
            }

            //Translate into tiles
            for (int x = 0; x < tilesWidth; x++)
                for (int y = 0; y < tilesHeight; y++)
                    if (grid[x, y] != null)
                    {
                        //Instantiate
                        Instantiate(grid[x, y]);

                        //Auto tile
                        if (grid[x, y] is Hole) (grid[x, y] as Hole).autoTile(grid);
                    }
        }

        //generate start room
        Entity[,] GenerateStartRoom(Random random)
        {
            return GenerateDefaultRoom(random);

            //blargh instructions in here ;D
        }

        //generate boss room
        Entity[,] GenerateBossRoom(Random random)
        {
            var grid = GenerateDefaultRoom(random);

            //blargh boss here ;D
            //Spawn some mobs?!
            int count = 10 + map.floor * 1 + map.game.random.Next(4 + map.floor * 2);
            for (int i = 0; i < count; i++)
            {
                List<Point> potentialPlaces = new List<Point>();
                for (int x = 1; x < tilesWidth - 2; x++)
                    for (int y = 1; y < tilesHeight - 2; y++)
                        if (grid[x, y] == null)
                        {
                            if (Vector2.Distance(new Vector2(x, y), new Vector2(0, tilesHeight / 2)) > 4)
                                if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth / 2, 0)) > 4)
                                    if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth - 1, tilesHeight / 2)) > 4)
                                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth / 2, tilesHeight - 1)) > 4)
                                            potentialPlaces.Add(new Point(x, y));
                        }

                if (potentialPlaces.Count == 0)
                    break;

                Point spawnPoint = potentialPlaces[random.Next(potentialPlaces.Count() - 1)];
                Vector2 position = new Vector2(spawnPoint.X * tileSize, spawnPoint.Y * tileSize) + new Vector2(tileSize / 2);
                if (map.game.random.Next(2) == 0)
                    grid[spawnPoint.X, spawnPoint.Y] = new Blob(this, position);
                else
                    grid[spawnPoint.X, spawnPoint.Y] = new Fly(this, position);
            }

            return grid;
        }

        //Generate default room
        private Entity[,] GenerateDefaultRoom(Random random)
        {
            Entity[,] grid = new Entity[tilesWidth, tilesHeight];

            //Outline
            for (int x = 0; x < tilesWidth; x++)
            {
                grid[x, 0] = new WallBlock(this, new Vector2(x * tileSize, 0));
                grid[x, tilesHeight - 1] = new WallBlock(this, new Vector2(x * tileSize, (tilesHeight - 1) * tileSize));
            }
            for (int y = 0; y < tilesHeight; y++)
            {
                grid[0, y] = new WallBlock(this, new Vector2(0, y * tileSize));
                grid[tilesWidth - 1, y] = new WallBlock(this, new Vector2((tilesWidth - 1) * tileSize, y * tileSize));
            }

            //Poke door holes in map
            if (left != null) grid[0, tilesHeight / 2] = new Door(this, new Vector2(0, tilesHeight / 2 * tileSize), 0.0f * (float)Math.PI);
            if (up != null) grid[tilesWidth / 2, 0] = new Door(this, new Vector2(tilesWidth / 2 * tileSize, 0), 0.5f * (float)Math.PI);
            if (right != null) grid[tilesWidth - 1, tilesHeight / 2] = new Door(this, new Vector2((tilesWidth - 1) * tileSize, tilesHeight / 2 * tileSize), 1.0f * (float)Math.PI);
            if (down != null) grid[tilesWidth / 2, tilesHeight - 1] = new Door(this, new Vector2(tilesWidth / 2 * tileSize, (tilesHeight - 1) * tileSize), 1.5f * (float)Math.PI);

            //Done
            return grid;
        }
        //Generate enemy room
        private Entity[,] GenerateEnemyRoom(Random random)
        {
            Entity[,] grid = null;

            //Main generation loop
            bool done = false;
            while (!done)
            {
                grid = GenerateDefaultRoom(random);

                //Random walls
                for (int x = 1; x < tilesWidth - 2; x++)
                {
                    for (int y = 1; y < tilesHeight - 2; y++)
                    {
                        if (random.NextDouble() < blockRate)
                        {
                            if (random.NextDouble() < blockHoleRate)
                            {
                                grid[x, y] = new Hole(this, new Vector2(x * tileSize, y * tileSize));

                                int wantedHoles = random.Next(minHoleSpreadCount, maxHoleSpreadCount);

                                int holeX = x;
                                int holeY = y;
                                for (int i = 0; i < wantedHoles; i++)
                                {
                                    int prevX = holeX;
                                    int prevY = holeY;
                                    int dir = random.Next(0, 4);

                                    if (dir == 0 && holeX > 1)
                                        holeX--;
                                    if (dir == 1 && holeY > 1)
                                        holeY--;
                                    if (dir == 2 && holeX < tilesWidth - 2)
                                        holeX++;
                                    if (dir == 3
                                        && holeY < tilesHeight - 2)
                                        holeY++;

                                    if (grid[holeX, holeY] == null)
                                        grid[holeX, holeY] = new Hole(this, new Vector2(holeX * tileSize, holeY * tileSize));
                                    else
                                    {
                                        holeX = prevX;
                                        holeY = prevY;

                                        wantedHoles++;
                                        if (wantedHoles > maxHoleSpreadCount + maxHoleBruteForceTries) break;
                                    }

                                }
                            }
                            else
                            {
                                if (grid[x, y] == null)
                                {
                                    grid[x, y] = new Block(this, new Vector2(x * tileSize, y * tileSize));

                                    if (x != 1 && grid[x - 1, y] == null && random.NextDouble() < blockEdgeRate)
                                        grid[x - 1, y] = new Block(this, new Vector2((x - 1) * tileSize, y * tileSize));

                                    if (y != 1 && grid[x, y - 1] == null && random.NextDouble() < blockEdgeRate)
                                        grid[x, y - 1] = new Block(this, new Vector2(x * tileSize, (y - 1) * tileSize));

                                    if (x != tilesWidth - 2 && grid[x + 1, y] == null && random.NextDouble() < blockEdgeRate)
                                        grid[x + 1, y] = new Block(this, new Vector2((x + 1) * tileSize, y * tileSize));

                                    if (y != tilesHeight - 2 && grid[x, y + 1] == null && random.NextDouble() < blockEdgeRate)
                                        grid[x, y + 1] = new Block(this, new Vector2(x * tileSize, (y + 1) * tileSize));

                                    if (x != 1 && y != 1 && grid[x - 1, y - 1] == null && random.NextDouble() < blockCornerRate)
                                        grid[x - 1, y - 1] = new Block(this, new Vector2((x - 1) * tileSize, (y - 1) * tileSize));

                                    if (x != 1 && y != tilesHeight - 2 && grid[x - 1, y + 1] == null && random.NextDouble() < blockCornerRate)
                                        grid[x - 1, y + 1] = new Block(this, new Vector2((x - 1) * tileSize, (y + 1) * tileSize));

                                    if (x != tilesWidth - 2 && y != 1 && grid[x + 1, y - 1] == null && random.NextDouble() < blockCornerRate)
                                        grid[x + 1, y - 1] = new Block(this, new Vector2((x + 1) * tileSize, (y - 1) * tileSize));

                                    if (x != tilesWidth - 2 && y != tilesHeight - 2 && grid[x + 1, y + 1] == null && random.NextDouble() < blockCornerRate)
                                        grid[x + 1, y + 1] = new Block(this, new Vector2((x + 1) * tileSize, (y + 1) * tileSize));
                                }
                            }
                        }
                    }
                }

                //Make a hole in the middle for the rewards or something lolwut i dont even...
                grid[tilesWidth / 2, tilesHeight / 2] = null;

                //Check if accessible between open doors
                bool[,] visited = new bool[tilesWidth, tilesHeight];
                List<Point> newPoints = new List<Point>();

                Point startPoint;
                if (left != null) startPoint = new Point(0, tilesHeight / 2);
                else if (up != null) startPoint = new Point(tilesWidth / 2, 0);
                else if (right != null) startPoint = new Point(tilesWidth - 1, tilesHeight / 2);
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
                            if (p.X >= 0 && p.Y >= 0 && p.X < tilesWidth && p.Y < tilesHeight && !visited[p.X, p.Y] && (grid[p.X, p.Y] == null || grid[p.X, p.Y] is IDoor))
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

                //Place chest at an non avalible area
                if (random.NextDouble() < chestRate)
                {
                    List<Point> randomPlaces = new List<Point>();
                    for (int x = 1; x < tilesWidth - 2; x++)
                        for (int y = 1; y < tilesHeight - 2; y++)
                            if (!visited[x, y] && grid[x, y] == null) randomPlaces.Add(new Point(x, y));

                    if (randomPlaces.Count != 0)
                    {
                        Point point = randomPlaces[random.Next(randomPlaces.Count - 1)];
                        if (random.NextDouble() < chestLockedRate)
                            grid[point.X, point.Y] = new LockedChest(this, new Vector2((point.X + .5f) * tileSize, (point.Y + .5f) * tileSize));
                        else
                            grid[point.X, point.Y] = new Chest(this, new Vector2((point.X + .5f) * tileSize, (point.Y + .5f) * tileSize));
                    }
                }

                //Spawn some mobs?!
                int count = map.floor + map.game.random.Next(4 + 1 * map.floor);
                for (int i = 0; i < count; i++)
                {
                    List<Point> potentialPlaces = new List<Point>();
                    for (int x = 1; x < tilesWidth - 2; x++)
                        for (int y = 1; y < tilesHeight - 2; y++)
                            if (visited[x, y] && grid[x, y] == null)
                            {
                                if (Vector2.Distance(new Vector2(x, y), new Vector2(0, tilesHeight / 2)) > 4)
                                    if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth / 2, 0)) > 4)
                                        if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth - 1, tilesHeight / 2)) > 4)
                                            if (Vector2.Distance(new Vector2(x, y), new Vector2(tilesWidth / 2, tilesHeight - 1)) > 4)
                                                potentialPlaces.Add(new Point(x, y));
                            }

                    if (potentialPlaces.Count == 0)
                        break;

                    Point spawnPoint = potentialPlaces[random.Next(potentialPlaces.Count() - 1)];
                    Vector2 position = new Vector2(spawnPoint.X * tileSize, spawnPoint.Y * tileSize) + new Vector2(tileSize / 2);
                    if (map.game.random.Next(2) == 0)
                        grid[spawnPoint.X, spawnPoint.Y] = new Blob(this, position);
                    else
                        grid[spawnPoint.X, spawnPoint.Y] = new Fly(this, position);
                }

                //Check if everything pwns
                int openDoors = 0;
                if (left != null) openDoors++;
                if (up != null) openDoors++;
                if (right != null) openDoors++;
                if (down != null) openDoors++;

                if (openDoors == 1)
                {
                    int accessibleTiles = 0;

                    foreach (bool tileVisited in visited) if (tileVisited) accessibleTiles++;

                    double accessability = (double)accessibleTiles / (double)(tilesWidth * tilesHeight);

                    done = (accessability >= minimumAccessibility);
                }
                else
                {
                    done = true;
                    if (left != null && !visited[0, tilesHeight / 2]) done = false;
                    if (up != null && !visited[tilesWidth / 2, 0]) done = false;
                    if (right != null && !visited[tilesWidth - 1, tilesHeight / 2]) done = false;
                    if (down != null && !visited[tilesWidth / 2, tilesHeight - 1]) done = false;
                }
            }
            return grid;
        }

        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.P))
                Generate();
            //Stuff
            foreach (var entity in entities)
                entity.Update();

            entities.RemoveWhere(e => !e.alive);

            foreach (var entity in entitiesAdd)
                entities.Add(entity);
            entitiesAdd.Clear();

            //clear
            {
                bool clearPrevious = clear;
                clear = entities.FirstOrDefault(e => e is Enemy) == null;
                //drop
                if (clear)
                    if (!clearPrevious)
                    {
                        Vector2 position = new Vector2(tilesWidth / 2 +.5f, tilesHeight / 2 +.5f) * tileSize;

                        switch (roomType)
                        {
                            case RoomType.Start:
                                //do nothing =P
                                break;
                            case RoomType.Enemy:
                                ClearEnemy(position);
                                break;
                            case RoomType.Boss:
                                ClearBoss(position);
                                break;
                            default:
                                throw new NotImplementedException("RoomType not implemented!");
                        }
                    }
            }
        }

        private void ClearBoss(Vector2 position)
        {
            //spawn awesome pickup ;D
            foreach (var playerControl in map.playersControl)
            {
                playerControl.healthMax++;
                if (playerControl.health > 0)
                    playerControl.health++;

                playerControl.fireRate++;
                playerControl.damage += .3f;
            }

            Instantiate(new Hatch(this));
        }

        void ClearEnemy(Vector2 position)
        {
            switch (map.game.random.Next(12))
            {
                case 0:
                    if (map.game.random.Next(8) == 0)// 1/8 spawn 5 money
                        Instantiate(new ItemMoney(this, position, false, ItemMoneyType.Five));
                    else
                        Instantiate(new ItemMoney(this, position, false, ItemMoneyType.One));
                    break;
                case 1:
                    if (map.game.random.Next(4) == 0)// 1/4 spawn 2 bombs
                        Instantiate(new ItemBomb(this, position, false, ItemBombType.Two));
                    else
                        Instantiate(new ItemBomb(this, position, false, ItemBombType.One));
                    break;
                case 2:
                    if (map.game.random.Next(2) == 0)// 1/2 spawn 2 health
                        Instantiate(new ItemHealth(this, position, false, ItemHealthType.Two));
                    else
                        Instantiate(new ItemHealth(this, position, false, ItemHealthType.One));
                    break;
                case 3:
                    Instantiate(new ItemKey(this, position, false));
                    break;
                default:
                    //DONT DO ANYTHING ;D hihihi
                    break;
            }
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
                this.matrix = matrix;

                map.game.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, matrix);

                //draw background
                Rectangle rectangleBackground = new Rectangle((int)(-offset.X - menuOffset.X), (int)(-offset.Y - menuOffset.Y), (int)(screen.X / scale), (int)(screen.Y / scale));
                map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("background"), rectangleBackground, rectangleBackground, Color.White);

                map.game.spriteBatch.End();

                map.game.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

                if (map.floor == 0)
                    if (roomType == RoomType.Start)
                    {
                        SpriteFont spriteFont = map.game.Content.Load<SpriteFont>("font");
                        string tutorial = "Press [Start] or [Enter] to play!\r\n[WASD] or (LS) Move\r\n[Arrow Keys] or (RS) Shoot\r\n[E] or (B) Drop bomb";
                        map.game.spriteBatch.DrawString(spriteFont, tutorial, (new Vector2(tilesWidth, tilesHeight) * tileSize - spriteFont.MeasureString(tutorial)) / 2, Color.White * .5f);
                    }

                //draw all entities
                foreach (var entity in entities.OrderBy(e => new Tuple<float, float>(-e.layerDepth, e.position.Y)))
                    entity.Draw();

                map.game.spriteBatch.End();
            }

            //DRAW THE HUD!!!
            {
                matrix = Matrix.CreateTranslation(new Vector3(offset, 0))
                    * Matrix.CreateScale(scale);

                map.game.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

                map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("hud"), Vector2.Zero, Color.White);

                //draw HUDS
                int hudPlayerWidth = 200;
                Vector2[] hudPlayerOffsets = new[]
                {
                    new Vector2(0, 0),
                    new Vector2(hudPlayerWidth, 0),
                    new Vector2(viewWidth - hudPlayerWidth * 2, 0),
                    new Vector2(viewWidth - hudPlayerWidth, 0),
                };
                SpriteFont spriteFont = map.game.Content.Load<SpriteFont>("font");
                //floor
                map.game.spriteBatch.DrawString(spriteFont, "floor: " + map.floor, new Vector2(20, viewHeight - 50), Color.White);
                //playerHUDS
                foreach (var playerControl in map.playersControl)
                {
                    Vector2 hudPlayerOffset = hudPlayerOffsets[playerControl.index];

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("lifeMax: ").Append(playerControl.healthMax).Append("\r\n");
                    stringBuilder.Append("life: ").Append(playerControl.health).Append("\r\n");
                    stringBuilder.Append("moneys: ").Append(playerControl.money).Append("\r\n");
                    stringBuilder.Append("bombs: ").Append(playerControl.bombs).Append("\r\n");
                    stringBuilder.Append("keys: ").Append(playerControl.keys).Append("\r\n");

                    map.game.spriteBatch.DrawString(spriteFont, stringBuilder.ToString(), hudPlayerOffset + new Vector2(20), Color.White);
                }

                //draw rooms
                //map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("block"), new Rectangle(0, 0, viewWidth, HUDheight), Color.Red);
                {
                    Vector2 offsetMap = new Vector2(viewWidth / 2, 8 * tileSize / 2 / 2) - new Vector2(tileSize / 2 / 2);

                    Dictionary<Vector2, Room> rooms = new Dictionary<Vector2, Room>();
                    HashSet<Vector2> used = new HashSet<Vector2>();
                    rooms.Add(Vector2.Zero, this);
                    while (rooms.Count > 0)
                    {
                        var room = rooms.First();
                        rooms.Remove(room.Key);
                        used.Add(room.Key);

                        //add neighbors!
                        if (room.Value.left != null)
                        {//left
                            Vector2 key = room.Key - Vector2.UnitX;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.left);
                        }
                        if (room.Value.right != null)
                        {//right
                            Vector2 key = room.Key + Vector2.UnitX;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.right);
                        }
                        if (room.Value.up != null)
                        {//up
                            Vector2 key = room.Key - Vector2.UnitY;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.up);
                        }
                        if (room.Value.down != null)
                        {//down
                            Vector2 key = room.Key + Vector2.UnitY;
                            if (!used.Contains(key))
                                if (!rooms.ContainsKey(key))
                                    rooms.Add(key, room.Value.down);
                        }

                        //if in view
                        //if visited or adjacent to visited to room, THEN DRAW
                        if (room.Key.X > -8 && room.Key.X < +8 && room.Key.Y > -4 && room.Key.Y < +4)
                            if (room.Value.visited ||
                                (room.Value.left != null && room.Value.left.visited) ||
                                (room.Value.right != null && room.Value.right.visited) ||
                                (room.Value.up != null && room.Value.up.visited) ||
                                (room.Value.down != null && room.Value.down.visited))
                            {
                                Color color = new Color(new Vector3(.4f));
                                if (room.Value == this)
                                    color = Color.White;
                                if (!room.Value.visited)
                                    color = new Color(new Vector3(.1f));
                                map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("room"), offsetMap + room.Key * tileSize / 2, color);
                                switch (room.Value.roomType)
                                {
                                    case RoomType.Boss:
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomBoss"), offsetMap + room.Key * tileSize / 2, Color.White);
                                        break;
                                    //add more cases later!
                                }
                                if (room.Value.visited)
                                {
                                    if (room.Value.entities.FirstOrDefault(e => e is ItemKey) != null)
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomKey"), offsetMap + room.Key * tileSize / 2, Color.White);
                                    if (room.Value.entities.FirstOrDefault(e => e is ItemBomb) != null)
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomBomb"), offsetMap + room.Key * tileSize / 2, Color.White);
                                    if (room.Value.entities.FirstOrDefault(e => e is ItemMoney) != null)
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomMoney"), offsetMap + room.Key * tileSize / 2, Color.White);
                                    if (room.Value.entities.FirstOrDefault(e => e is ItemHealth) != null)
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomHealth"), offsetMap + room.Key * tileSize / 2, Color.White);
                                    if (room.Value.entities.FirstOrDefault(e => e is Chest && !(e as Chest).open || e is LockedChest && !(e as LockedChest).open) != null)
                                        map.game.spriteBatch.Draw(map.game.Content.Load<Texture2D>("roomChest"), offsetMap + room.Key * tileSize / 2, Color.White);
                                }
                            }
                    }
                }

                map.game.spriteBatch.End();
            }
        }
    }
}
