using System;
using System.Collections.Generic;
using ECS;
using Microsoft.Xna.Framework;


namespace Game.Components
{
    class WayPointComponent : IComponent
    {
        /// <summary>
        /// The way-points for the entity to cycle between.
        /// </summary>
        public List<Vector3> WayPoints { get; set; }

        /// <summary>
        /// The way-point the entity is currently moving towards.
        /// </summary>
        public Vector3 TargetWayPoint { get; set; }

        /// <summary>
        /// How fast the entity should move towards it's target.
        /// </summary>
        public float MoveSpeed { get; set; }

        public WayPointComponent()
        {
            WayPoints = new List<Vector3>();
        }
    }
}
