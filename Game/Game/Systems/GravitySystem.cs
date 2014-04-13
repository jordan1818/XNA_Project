using System;
using System.Collections.Generic;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Components;

namespace Game.Systems
{
    class GravitySystem : EntitySystem
    {

        public GravitySystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(VelocityComponent) }, GameLoopType.Update)
        {
        }

        protected override void Process(Entity entity)
        {
            var vel = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            // Set y-vel to 0 when at or below ground.
            if (transform.Position.Y <= 0)
            {
                vel.Velocity = new Vector3(vel.Velocity.X, 0, vel.Velocity.Z);
            }
            // Apply gravity.
            else
            {
                vel.Velocity += new Vector3(0f, -0.017f, 0f);
            }
        }
    }
}
