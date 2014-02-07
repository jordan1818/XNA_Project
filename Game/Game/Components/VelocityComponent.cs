using System;
using Microsoft.Xna.Framework;
using ECS;

namespace Game.Components
{
    // Provides velocity to an entity.
    public class VelocityComponent : IComponent
    {
        // The velocity of the entity.
        public Vector3 Velocity { get; set; }

        public VelocityComponent()
        {
            Velocity = Vector3.Zero;
        }
    }
}
