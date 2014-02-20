using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    /// <summary>
    /// Type type of system. Either an update or draw system.
    /// </summary>
    public enum GameLoopType { Update, Draw };

    /// <summary>
    /// Base class for all systems.
    /// </summary>
    public abstract class EntitySystem
    {
        /// <summary>
        /// Read only ref to the EntityWorld this systems belongs to.
        /// </summary>
        protected readonly EntityWorld entityWorld;

        /// <summary>
        /// The list of component types this system operates on.
        /// </summary>
        public Type[] Types { get; protected set; }

        /// <summary>
        /// Which loop this systems gets called in.
        /// </summary>
        public GameLoopType GameLoopType { get; protected set; }

        /// <summary>
        /// Called when the system starts processing the list of entities.
        /// </summary>
        public event EventHandler ProcessingStarted;

        /// <summary>
        /// Called when the system finishes processing the list of entities.
        /// </summary>
        public event EventHandler ProcessingFinished;

        public EntitySystem(EntityWorld entityWorld, Type[] types, GameLoopType gameLoopType)
        {
            this.entityWorld  = entityWorld;
            this.Types        = types;
            this.GameLoopType = gameLoopType;
        }

        /// <summary>
        /// Process a valid entity for this system. To be implemented by the actual system.
        /// </summary>
        /// <param name="entity">The entity to be processed.</param>
        protected abstract void Process(Entity entity);

        /// <summary>
        /// Process a single entity if it has all required components.
        /// </summary>
        /// <param name="entity">The entity to process.</param>
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

        /// <summary>
        /// Process a list of entities.
        /// </summary>
        /// <param name="entities">The list of entities.</param>
        internal void ProcessEntities(List<Entity> entities)
        {
            if (ProcessingStarted != null)
            {
                ProcessingStarted(this, null);
            }

            foreach (var e in entities)
            {
                ProcessEntity(e);
            }

            if (ProcessingFinished != null)
            {
                ProcessingFinished(this, null);
            }
        }
    }
}
