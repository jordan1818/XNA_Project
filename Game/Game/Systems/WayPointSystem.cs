using System;
using System.Collections.Generic;
using ECS;
using Game.Components;
using Microsoft.Xna.Framework;

namespace Game.Systems
{
    public sealed class WayPointSystem : EntitySystem
    {
        private static float minDistToWayPoint = 2.5f;

        public WayPointSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { 
                typeof(TransformComponent), 
                typeof(WayPointComponent),
                typeof(VelocityComponent)
            }, GameLoopType.Update)
        {

        }

        /// <summary>
        /// Move an entity around a set of way-points.
        /// </summary>
        /// <param name="entity">The entity being processed.</param>
        protected override void Process(Entity entity)
        {
            var transform = entity.GetComponent<TransformComponent>();
            var vel       = entity.GetComponent<VelocityComponent>();
            var wayPoint  = entity.GetComponent<WayPointComponent>();

            if (wayPoint.TargetWayPoint == null)
                wayPoint.TargetWayPoint = wayPoint.WayPoints[0];

            // Check if we are at, or almost at, the current target.
            var distToTarget = wayPoint.TargetWayPoint - transform.Position;
            if (distToTarget.Length() < minDistToWayPoint)
            {
                // Update the target to the next way-point.
                var oldTargetIndex = wayPoint.WayPoints.IndexOf(wayPoint.TargetWayPoint);
                var newTargetIndex = (oldTargetIndex + 1) % wayPoint.WayPoints.Count;
                wayPoint.TargetWayPoint = wayPoint.WayPoints[newTargetIndex];
            } 
            else
            {
                // Keep the entity moving towards his target.
                distToTarget.Normalize();
                vel.Velocity = distToTarget * wayPoint.MoveSpeed;
            }
        }
    }
}
