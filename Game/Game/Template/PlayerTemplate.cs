using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;

namespace Game.Template
{
    class PlayerTemplate : IEntityTemplate
    {
        public PlayerTemplate()
        {
        }

        public Entity Build(Entity e)
        {
            return e;
        }
   
    }
}
