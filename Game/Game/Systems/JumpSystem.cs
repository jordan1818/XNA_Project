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
            var vel = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var jump = entity.GetComponent<JumpComponent>();

            if (jump.JumpCount > jump.MAXJUMPS)
                jump.WantToJump = false;

            else if (jump.WantToJump && jump.JumpCount < jump.MAXJUMPS)
                jump.JumpCount += 1;
                jump.WantToJump = false;

            if (transform.Position.Y <= 0.0f)
                jump.JumpCount = 0;


        }
    }
}
