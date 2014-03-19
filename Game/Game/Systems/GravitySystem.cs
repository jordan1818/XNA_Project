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
            base(entityWorld, new Type[] { typeof(VelocityComponent), typeof(TransformComponent), typeof(JumpComponent)}, GameLoopType.Update)
        {
        }

        protected override void Process(Entity entity)
        {
            var vel = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var jump = entity.GetComponent<JumpComponent>();

            if (vel.applyGravity)
                vel.Velocity -= new Vector3(0, gravity, 0);

            if (transform.Position.Y <= 0.0f)
            {
                transform.Position = new Vector3(transform.Position.X, 0, transform.Position.Z);
                vel.applyGravity = false;
            }

            if (vel.applyGravity == false)
                vel.Velocity = Vector3.Zero;
        }
    }
}
