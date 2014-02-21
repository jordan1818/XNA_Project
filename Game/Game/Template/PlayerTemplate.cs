using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Template
{
    class PlayerTemplate : IEntityTemplate
    {
        public PlayerTemplate()
        {
        }

        public Entity Build(Entity e)
        {
            e.AddComponent(new SpatialFormComponent("minimon for upload"));
            e.AddComponent(new TransformComponent());
            e.AddComponent(new VelocityComponent());
            e.AddComponent(new InputComponent());
            e.GetComponent<TransformComponent>().Position = new Vector3(0, 0, -45);
            return e;
        }
   
    }
}
