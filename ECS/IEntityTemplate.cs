using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS
{
    public interface IEntityTemplate
    {
        Entity Build(Entity e, params object[] args);
    }
}
