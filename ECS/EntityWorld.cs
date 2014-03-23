using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    /// <summary>
    /// The entity world is responsible for managing entities and systems.
    /// </summary>
    public sealed class EntityWorld
    {
        /// <summary>
        /// A mapping of entities to their unique ID's.
        /// </summary>
        private Dictionary<long, Entity> entities;

        /// <summary>
        /// The systems which are processed during the update call.
        /// </summary>
        private List<EntitySystem> updateSystems;

        /// <summary>
        /// The systems which are processed during the draw call.
        /// </summary>
        private List<EntitySystem> drawSystems;

        /// <summary>
        /// The time elapsed since the last update call.
        /// </summary>
        public TimeSpan DeltaTime { get; private set; }

        /// <summary>
        /// The next available ID.
        /// </summary>
        private long nextFreeID = 0;

        public EntityWorld()
        {
            entities      = new Dictionary<long, Entity>();
            updateSystems = new List<EntitySystem>();
            drawSystems   = new List<EntitySystem>();
        }
        
        /// <summary>
        /// Creates an entity which is added to the internal entity pool. 
        /// </summary>
        public Entity CreateEntity()
        {
            var e = new Entity(nextFreeID++);
            entities[e.UniqueID] = e;
            return e;
        }

        /// <summary>
        /// Returns an entity. Will throw if an invalid ID is passed.
        /// </summary>
        /// <param name="ID">The ID to lookup.</param>
        /// <returns></returns>
        public Entity GetEntity(long ID)
        {
            return entities[ID];
        }

        public Entity GetEntityByTag(string tag)
        {
            return  (from entity in entities
                     where entity.Value.Tag == tag
                     select entity).Single().Value;
        }

        /// <summary>
        /// Creates an entity from template.
        /// </summary>
        /// <typeparam name="T">Entity template</typeparam>
        /// <returns>Entity of the template</returns>
        public Entity CreateFromTemplate<T>(params object[] args) where T:IEntityTemplate
        {
            var e = CreateEntity();
            var t = (T) Activator.CreateInstance(typeof(T), null);
            t.Build(e, args);
            return e;	
        }

        /// <summary>
        /// Register a system with the entity world.
        /// </summary>
        /// <typeparam name="T">The system type.</typeparam>
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

        /// <summary>
        /// Process all update systems.
        /// </summary>
        /// <param name="time">Time since last update call.</param>
        public void Update(TimeSpan time)
        {
            DeltaTime = time;

            var entList = entities.Values.ToList();

            foreach (var system in updateSystems)
            {
                system.ProcessEntities(entList);
            }
        }

        /// <summary>
        /// Process all draw systems. 
        /// </summary>
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
