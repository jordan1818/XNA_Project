using System;
using System.Collections.Generic;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Components;

namespace Game.Systems
{
    class JumpSystem : EntitySystem
    {
        public JumpSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(VelocityComponent), typeof(TransformComponent), typeof(JumpComponent) }, GameLoopType.Update)
        {
        }

        protected override void Process(Entity entity)
        {
            var vel       = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var jump      = entity.GetComponent<JumpComponent>();

            // Check if player wants to jump and still can.
            if (jump.WantToJump && jump.JumpCount < JumpComponent.MaxJumps)
            {
                vel.Velocity = new Vector3(vel.Velocity.X, JumpComponent.MaxYVel, vel.Velocity.Z);
                jump.JumpCount++;
                jump.WantToJump = false;
            }
            // Check if the player has landed again and reset his jump count.
            else if (jump.JumpCount > 0 && transform.Position.Y <= 0)
            {
                jump.WantToJump = false;
                jump.JumpCount = 0;
                transform.Position = new Vector3(transform.Position.X, 0, transform.Position.Z);
            }
        }
    }
}
