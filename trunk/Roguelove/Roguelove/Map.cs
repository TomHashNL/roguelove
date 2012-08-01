using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Roguelove
{
    public class Map : GameState
    {
        public int floor;
        Room room;
        List<PlayerControl> playersControl;

        public Map(Game1 game, List<PlayerControl> playersControl, int floor)
            : base(game)
        {
            this.playersControl = playersControl;
            this.floor = floor;
            
            RoomChange(Generate(5 + floor * 2));
        }

        public Room Generate(int roomsCountTarget)
        {
            Random random = new Random();

            //try to make a map with a valid bossroom and such
            while (true)
            {
                Vector2[] ways = new[]
                {
                    new Vector2(-1, 0),
                    new Vector2(+1, 0),
                    new Vector2(0, -1),
                    new Vector2(0, +1),
                };

                Dictionary<Vector2, Room> rooms = new Dictionary<Vector2, Room>();//rooms added
                HashSet<Vector2> roomsFail = new HashSet<Vector2>();//roomsfailed, should not add anymoar
                HashSet<Vector2> roomsEvaluate = new HashSet<Vector2>();//rooms to evaluate
                roomsEvaluate.Add(Vector2.Zero);

                while (rooms.Count < roomsCountTarget)
                {
                    if (roomsEvaluate.Count == 0)
                    {
                        var fail = roomsFail.First();
                        roomsEvaluate.Add(fail);
                        roomsFail.Remove(fail);
                    }

                    Vector2 room = roomsEvaluate.First();
                    roomsEvaluate.Remove(room);

                    rooms.Add(room, new Room(this));

                    foreach (var way in ways)
                    {
                        Vector2 roomNew = room + way;
                        if (!roomsFail.Contains(roomNew))
                            if (!rooms.ContainsKey(roomNew))
                                if (!roomsEvaluate.Contains(roomNew))
                                {
                                    if (random.NextDouble() < .4)
                                        roomsEvaluate.Add(roomNew);
                                    else
                                        roomsFail.Add(roomNew);
                                }
                    }
                }

                bool roomBoss = false;
                Room roomStart = null;
                foreach (var room in rooms)
                {
                    int doors = 0;
                    if (rooms.TryGetValue(room.Key - Vector2.UnitX, out room.Value.left))
                        doors++;
                    if (rooms.TryGetValue(room.Key + Vector2.UnitX, out room.Value.right))
                        doors++;
                    if (rooms.TryGetValue(room.Key - Vector2.UnitY, out room.Value.up))
                        doors++;
                    if (rooms.TryGetValue(room.Key + Vector2.UnitY, out room.Value.down))
                        doors++;
                    if (room.Value.roomType == RoomType.Enemy)
                        if (roomStart == null)
                        {
                            room.Value.roomType = RoomType.Start;
                            roomStart = room.Value;
                        }
                        else if (doors == 1 && !roomBoss)
                        {
                            room.Value.roomType = RoomType.Boss;
                            roomBoss = true;
                        }
                    //else if ()//for other types of rooms
                }

                //continue!!!! ;D
                if (roomBoss)
                {
                    //GENERATE AAAAAAALL THE ROOMS!!!!
                    foreach (var room in rooms)
                        room.Value.Generate();

                    return roomStart;
                }
            }
        }

        public void RoomChange(Room room)
        {
            this.room = room;

            //place all the players in the current map =o
            foreach (var playerControl in playersControl)
            {
                //FIND THE MIDDLE OF THE ROOM HERE TO PLACE PLAYERS???
                playerControl.player = new Player(this.room, playerControl, Vector2.Zero, Vector2.Zero);
                this.room.Instantiate(playerControl.player);
            }
        }

        public void FloorAdvance()
        {
            game.GameStateChange(new Map(game, playersControl, floor + 1));
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                RoomChange(Generate(10));

            room.Update();
        }

        public override void Draw()
        {
            room.Draw();

            //DRAW THE HUD HERE
            //game.spriteBatch.Begin();
            ////BLARHGH
            //game.spriteBatch.End();
        }
    }
}
