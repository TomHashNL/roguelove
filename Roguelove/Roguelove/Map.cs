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
        /// <summary>
        /// DO NOT ADJUST!
        /// </summary>
        public List<PlayerControl> playersControl;
        public readonly double RoomAdjacentSuccessRate = .4;

        public Map(Game1 game, int floor)
            : base(game)
        {
            Constructor(game, floor, new List<PlayerControl>());
        }

        public Map(Game1 game, int floor, List<PlayerControl> playersControl)
            : base(game)
        {
            Constructor(game, floor, playersControl);
        }

        public void Constructor(Game1 game, int floor, List<PlayerControl> playersControl)
        {
            this.floor = floor;
            this.playersControl = playersControl;

            RoomChange(Generate((int)((7 + floor * 2) * (1.0 + .3 * game.random.NextDouble()))));
        }

        public Room Generate(int roomsCountTarget)
        {
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

                    var roomsArray = roomsEvaluate.ToArray();
                    Vector2 room = roomsArray[game.random.Next(roomsArray.Length)];
                    //Vector2 room = roomsEvaluate.First();
                    roomsEvaluate.Remove(room);

                    rooms.Add(room, new Room(this));

                    foreach (var way in ways)
                    {
                        Vector2 roomNew = room + way;
                        if (!roomsFail.Contains(roomNew))
                            if (!rooms.ContainsKey(roomNew))
                                if (!roomsEvaluate.Contains(roomNew))
                                {
                                    if (game.random.NextDouble() < RoomAdjacentSuccessRate)
                                        roomsEvaluate.Add(roomNew);
                                    else
                                        roomsFail.Add(roomNew);
                                }
                    }
                }

                bool roomBoss = false;
                Room roomStart = null;

                {
                    var roomsArray = rooms.ToArray();
                    roomStart = roomsArray[game.random.Next(roomsArray.Length)].Value;
                    roomStart.roomType = RoomType.Start;
                }

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
                        if (doors == 1 && !roomBoss)
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

        /// <summary>
        /// Place players at center of new room with zero velocity!
        /// </summary>
        /// <param name="room"></param>
        public void RoomChange(Room room)
        {
            RoomChange(room, new Vector2(room.tilesWidth * room.tileSize, room.tilesHeight * room.tileSize) / 2);
        }

        /// <summary>
        /// place players at position and velocity in new room!
        /// </summary>
        /// <param name="room"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public void RoomChange(Room room, Vector2 position)
        {
            //remove previous players/bombs/bullets =O
            if (this.room != null)
                foreach (var entity in room.entities)
                    if (entity is Player ||
                        entity is Bomb ||
                        entity is Bullet)
                        entity.Destroy();

            //set new room!
            this.room = room;

            this.room.visited = true;

            //place all the players in the current map =o
            foreach (var playerControl in playersControl)
            {
                //FIND THE MIDDLE OF THE ROOM HERE TO PLACE PLAYERS???
                playerControl.player = new Player(this.room, playerControl, position, Vector2.Zero);
                this.room.Instantiate(playerControl.player);
            }
        }

        public void FloorAdvance()
        {
            game.GameStateChange(new Map(game, floor + 1, playersControl));
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game.Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
            //    FloorAdvance();

            //spawning only for starting room and such =D
            if (floor == 0)
                if (room.roomType == RoomType.Start)
                    if (room.left == null || !room.left.visited)
                        if (room.right == null || !room.right.visited)
                            if (room.up == null || !room.up.visited)
                                if (room.down == null || !room.down.visited)
                                {
                                    //keyboard
                                    if (playersControl.FirstOrDefault(e => e.inputType == InputType.Keyboard) == null)
                                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                                        {
                                            var playerControl = new PlayerControl(playersControl.Count, InputType.Keyboard);
                                            playerControl.player = new Player(this.room, playerControl, new Vector2(room.tilesWidth, room.tilesHeight) * room.tileSize / 2, Vector2.Zero);

                                            playersControl.Add(playerControl);

                                            this.room.Instantiate(playerControl.player);
                                        }
                                    //gamepads
                                    Tuple<PlayerIndex, InputType>[] playerIndexes = new[]
                                    {
                                        new Tuple<PlayerIndex, InputType>(PlayerIndex.One, InputType.Gamepad1),
                                        new Tuple<PlayerIndex, InputType>(PlayerIndex.Two, InputType.Gamepad2),
                                        new Tuple<PlayerIndex, InputType>(PlayerIndex.Three, InputType.Gamepad3),
                                        new Tuple<PlayerIndex, InputType>(PlayerIndex.Four, InputType.Gamepad4),
                                    };
                                    foreach (var playerIndex in playerIndexes)
                                        if (playersControl.FirstOrDefault(e => e.inputType == playerIndex.Item2) == null)
                                            if (GamePad.GetState(playerIndex.Item1).IsButtonDown(Buttons.Start))
                                            {
                                                var playerControl = new PlayerControl(playersControl.Count, playerIndex.Item2);
                                                playerControl.player = new Player(this.room, playerControl, new Vector2(room.tilesWidth, room.tilesHeight) * room.tileSize / 2, Vector2.Zero);

                                                playersControl.Add(playerControl);

                                                this.room.Instantiate(playerControl.player);
                                            }
                                }

            //rooom update ;D
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
