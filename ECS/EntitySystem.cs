using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    public enum GameLoopType { Update, Draw };

    public abstract class EntitySystem
    {
        protected readonly EntityWorld entityWorld;

        // The list of component types this system operates on.
        public Type[] Types { get; protected set; }

        // Which loop this system gets called in.
        public GameLoopType GameLoopType { get; protected set; }

        public EntitySystem(EntityWorld entityWorld, Type[] types, GameLoopType gameLoopType)
        {
            this.entityWorld  = entityWorld;
            this.Types        = types;
            this.GameLoopType = gameLoopType;
        }

        // Process a valid entity for this system.
        protected abstract void Process(Entity entity);

        // Process a single entity if it has all required components.
        internal void ProcessEntity(Entity entity)
        {
            // Make sure it has all required components.
            foreach (var t in Types)
            {
                if (!entity.HasComponent(t))
                {
                    return;
                }
            }
            Process(entity);
        }

        // Process the list of entities.
        internal void ProcessEntities(List<Entity> entities)
        {
            foreach (var e in entities)
            {
                ProcessEntity(e);
            }
        }
    }
}
