using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Systems
{
    class CollisionSystem : EntitySystem
    {
        private EntityWorld entityWorld;

        public CollisionSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(VelocityComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
            this.entityWorld = entityWorld;
        }

        protected override void Process(Entity entity)
        {

        }
    }
}
