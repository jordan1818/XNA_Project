using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    // The entity world is responsible for managing entities and systems.
    public sealed class EntityWorld
    {
        // A mapping of entities to their unique ID's.
        private Dictionary<long, Entity> entities;

        // The systems which are processed during the update call.
        private List<EntitySystem> updateSystems;

        // The systems which are processed during the draw call.
        private List<EntitySystem> drawSystems;

        // The time elapsed since the last update call.
        public TimeSpan DeltaTime { get; private set; }

        private long nextFreeID = 0;

        public EntityWorld()
        {
            entities      = new Dictionary<long, Entity>();
            updateSystems = new List<EntitySystem>();
            drawSystems   = new List<EntitySystem>();
        }
        
        // Creates an entity which is added to the internal entity pool. 
        public Entity CreateEntity()
        {
            var e = new Entity(nextFreeID++);
            entities[e.UniqueID] = e;
            return e;
        }

        // Returns an entity. Will throw if an invalid ID is passed.
        public Entity GetEntity(long ID)
        {
            return entities[ID];
        }

        // Register a system with the entity world.
        public void RegisterSystem<T>() where T:EntitySystem
        {
            // Create the system.
            var system = Activator.CreateInstance(typeof(T), new object[] { this }) as T;

            // Register it with the world.
            if (system.GameLoopType == GameLoopType.Update)
                updateSystems.Add(system);
            else
                drawSystems.Add(system);
        }

        // Process all update systems.
        public void Update(TimeSpan time)
        {
            DeltaTime = time;

            var entList = entities.Values.ToList();

            foreach (var system in updateSystems)
            {
                system.ProcessEntities(entList);
            }
        }

        // Process all draw systems.
        public void Draw()
        {
            var entList = entities.Values.ToList();

            foreach (var system in drawSystems)
            {
                system.ProcessEntities(entList);
            }
        }
    }
}
