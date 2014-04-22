using System;
using System.Collections.Generic;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Components
{
    class CollisionComponent : IComponent
    {
        Dictionary<string, Matrix> modelsTransformMatrix;

        public CollisionComponent()
        {

        }
    }
}
