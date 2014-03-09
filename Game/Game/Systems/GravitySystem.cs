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
        private float gravity = 0.00098f;

            public GravitySystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(VelocityComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
        }

        protected override void Process(Entity entity)
        {
            var vel = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            transform.Position += vel.Velocity * entityWorld.DeltaTime.Milliseconds;

            if (vel.applyGravity)
                vel.Velocity -= new Vector3(0, gravity, 0);

            if (transform.Position.Y <= 0.0f)
                vel.applyGravity = false;

            if (vel.applyGravity == false)
                vel.Velocity = Vector3.Zero;
        }
    }
}
