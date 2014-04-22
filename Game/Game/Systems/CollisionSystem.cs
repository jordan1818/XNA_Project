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

        Model player;
        private Dictionary<string, Model> models;
        private Dictionary<string, Matrix> obstaclesPos;

        private const int MAXOBSTACLES = 24;//96;

        private TimeSpan timeSinceStart;

        public CollisionSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(SpatialFormComponent), typeof(CollisionComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
            Content = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.entityWorld = entityWorld;

            player = FetchModel(entityWorld.GetEntityByTag("PLAYER").GetComponent<SpatialFormComponent>().SpatialFormFile);
            models["table"] = FetchModel("table");
            models["flask"] = FetchModel("flask");
            models["RedBall"] = FetchModel("RedBall");
            models["banana"] = FetchModel("banana");
        }

        protected override void Process(Entity entity)
        {
            Model obstacles = null;

            foreach (var par in models)
            {
                obstacles = models[par.Key];
            }

            
                for (int i = 0; i < player.Meshes.Count; i++)
                {
                    BoundingSphere firstSphere = player.Meshes[i].BoundingSphere;
                    firstSphere = firstSphere.Transform(entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>().TransformMatrix);

                    for (int k = 0; k < MAXOBSTACLES; k++)
                    {
                        var Transform = entityWorld.GetEntityByTag("obstacle" + k.ToString()).GetComponent<TransformComponent>();

                        for (int j = 0; j < obstacles.Meshes.Count; j++)
                        {
                            timeSinceStart += entityWorld.DeltaTime;
                            BoundingSphere secondSphere = obstacles.Meshes[j].BoundingSphere;

                            // THIS WORKS
                            secondSphere = secondSphere.Transform(entityWorld.GetEntityByTag("obstacle" + k.ToString()).GetComponent<TransformComponent>().TransformMatrix);
                            if (firstSphere.Intersects(secondSphere))
                            {
                                Transform.Position = new Vector3(-50, 0, 0);
                                // do something.
                            }
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
