using System;
using Microsoft.Xna.Framework;
using ECS;

namespace Game.Components
{   
    /// <summary>
    /// Provides velocity to an entity.
    /// </summary>
    public class VelocityComponent : IComponent
    {
        /// <summary>
        /// The velocity of the entity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        public VelocityComponent()
        {
            Velocity = Vector3.Zero;
        }
    }
}
