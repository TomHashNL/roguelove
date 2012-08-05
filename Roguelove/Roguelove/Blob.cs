using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Roguelove
{
    public class Blob : Enemy, ISolid
    {
        public Blob(Room room, Vector2 position)
            : base(room, position, 10, 2)
        {
            texture = room.map.game.Content.Load<Texture2D>("enemy");
            origin = new Vector2(texture.Width, texture.Height) / 2;
        }

        public override void Update()
        {
            var collisions = Collide(new HashSet<Type>(new[] {
                typeof(ISolid),
                typeof(DoorOpen),
            }), true);

            //Find closest player
            Point startPoint = new Point();
            Vector2? foundPoint = null;
            var players = room.entities.Where(e => e is Player);
            if (players.Count() > 0)
            {
                float minDis = float.PositiveInfinity;
                foreach (Player player in players)
                {
                    float dis = Vector2.Distance(player.position, position);
                    if (dis < minDis)
                    {
                        minDis = dis;
                        startPoint = new Point((int)(player.position.X + .5f) / room.tileSize, (int)((player.position.Y + .5f) / room.tileSize));
                    }
                }

                //Generate grid thingie
                Entity[,] grid = new Entity[room.tilesWidth, room.tilesHeight];
                var kewlstuff = room.entities.Where(e => e is IDoor || e is Block || e is WallBlock || e is Hole);
                foreach (var shit in kewlstuff)
                    grid[(int)shit.position.X / room.tileSize, (int)shit.position.Y / room.tileSize] = shit;

                //Traverse
                bool[,] visited = new bool[room.tilesWidth, room.tilesHeight];
                List<Point> newPoints = new List<Point>();

                Point targetPoint = new Point((int)(position.X + .5f) / room.tileSize, (int)((position.Y + .5f) / room.tileSize));

                visited[startPoint.X, startPoint.Y] = true;
                newPoints.Add(startPoint);

                while (newPoints.Count != 0)
                {
                    for (int i = 0; i < newPoints.Count; i++)
                    {
                        Point point = newPoints[i];
                        Point[] points = new Point[4]
                        {
                            new Point(point.X - 1, point.Y),
                            new Point(point.X + 1, point.Y),
                            new Point(point.X, point.Y - 1),
                            new Point(point.X, point.Y + 1),
                        };

                        bool endPoint = true;
                        foreach (Point p in points)
                        {
                            //If target found
                            if (p == targetPoint)
                            {
                                foundPoint = new Vector2((point.X + .5f) * room.tileSize, (point.Y + .5f) * room.tileSize);
                                break;
                            }
                            else if (p.X >= 0 && p.Y >= 0 && p.X < room.tilesWidth && p.Y < room.tilesHeight && !visited[p.X, p.Y] && !(grid[p.X, p.Y] is IDoor || grid[p.X, p.Y] is ISolid))
                            {
                                newPoints.Add(new Point(p.X, p.Y));
                                visited[p.X, p.Y] = true;
                                endPoint = false;
                            }
                        }
                        if (foundPoint.HasValue) break;

                        if (endPoint)
                        {
                            newPoints.RemoveAt(i);
                            i--;
                        }
                    }
                    if (foundPoint.HasValue) break;
                }
            }

            //GET TO DUH CHOPPAH
            if (foundPoint.HasValue)
            {
                Vector2 delta = new Vector2(foundPoint.Value.X - position.X, foundPoint.Value.Y - position.Y);
                delta.Normalize();

                velocity += delta * 0.5f;
            }

            velocity *= .8f;

            position += velocity;
        }

        protected override void OnDestroy()
        {

        }
    }
}
