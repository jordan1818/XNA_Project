﻿using System;
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
        private Model models;
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
            var entityONE   =   FetchModel(spatialform);
            var entityTWO   =   FetchModel(spatialform);

            /*
            for(int i = 0; i < entityONE.Meshes.Count; i++)
            {
                BoundingBox firstSphere = entityONE.Meshes[i].BoundingSphere;

                for (int j = 0; j < entityTWO.Meshes.Count; j++)
                {
                    BoundingSphere secondSphere = entityTWO.Meshes[i].BoundingSphere;

                    if (firstSphere.Intersects(secondSphere))
                    {
                        
                    }
                }
            }
             */
        }

        private Model FetchModel(string key)
        {
            Model m;
            m = Content.Load<Model>(key);
            return m;
        }
    }
}
