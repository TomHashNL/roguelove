using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        }

        public void Update()
        {
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
            foreach (var entity in entities)
                entity.Draw();
        }
    }
}
