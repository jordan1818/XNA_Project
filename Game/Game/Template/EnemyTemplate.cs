using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Template
{
    class EnemyTemplate : IEntityTemplate
    {
        public EnemyTemplate()
        {
        }

        public Entity Build(Entity e, params object[] args)
        {
            e.Tag = (string)args[1];
            e.AddComponent(new SpatialFormComponent((string)args[0]));
            e.AddComponent(new TransformComponent());
            e.AddComponent(new VelocityComponent());
            return e;
        }
    }
}
