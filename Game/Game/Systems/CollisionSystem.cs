using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Systems
{
    class CollisionSystem : EntitySystem
    {
        private ContentManager Content;

        private EntityWorld entityWorld;

        public CollisionSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(SpatialFormComponent), typeof(CollisionComponent) }, GameLoopType.Update)
        {
            Content = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.entityWorld = entityWorld;
        }

        protected override void Process(Entity entity)
        {
            var spatialform =   entity.GetComponent<SpatialFormComponent>().SpatialFormFile;
            var player      =   FetchModel(entityWorld.GetEntityByTag("PLAYER").GetComponent<SpatialFormComponent>().SpatialFormFile);
            var obstacles   =   FetchModel(spatialform);

            for(int i = 0; i < player.Meshes.Count; i++)
            {
                BoundingSphere firstSphere = player.Meshes[i].BoundingSphere;
                firstSphere = firstSphere.Transform(entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>().TransformMatrix);

                for (int j = 0; j < obstacles.Meshes.Count; j++)
                {
                    BoundingSphere secondSphere = obstacles.Meshes[i].BoundingSphere;
                    secondSphere = secondSphere.Transform(entity.GetComponent<TransformComponent>().TransformMatrix);

                    if (firstSphere.Intersects(secondSphere))
                    {
                        // Seems to be always colliding...
                        // do something.
                    }
                }
            }
        }

        private Model FetchModel(string key)
        {
            Model m;
            m = Content.Load<Model>(key);
            return m;
        }
    }
}
