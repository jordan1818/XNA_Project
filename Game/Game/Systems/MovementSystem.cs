using System;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Systems
{
    /// <summary>
    /// Provides movement to any entity with a velocity and transform component.
    /// </summary>
    public sealed class MovementSystem : EntitySystem
    {
        public MovementSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(VelocityComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
        }

        /// <summary>
        /// Updates the entities position based on it's velocity.
        /// </summary>
        /// <param name="entity">The entity being processed.</param>
        protected override void Process(Entity entity)
        {
            var vel = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            transform.Position += vel.Velocity * entityWorld.DeltaTime.Milliseconds;
        }
    }
}
