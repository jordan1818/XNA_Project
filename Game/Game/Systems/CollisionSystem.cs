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
        Matrix playertranform;
        private Dictionary<string, Model> models;
        private List<TransformComponent> obstaclesTransform;


        bool initialize = true;

        private const int MAXOBSTACLES = 96;//BlackBoard.GetEntry<int>("AmountObstacle");

        //private TimeSpan timeSinceStart;

        public CollisionSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(SpatialFormComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
            Content = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.entityWorld = entityWorld;

            models = new Dictionary<string, Model>();
            obstaclesTransform = new List<TransformComponent>();

            ProcessingStarted += new EventHandler(CollisionSystem_ProcessingStarted);

            models["table"] = FetchModel("table");
            models["flask"] = FetchModel("flask");
            models["RedBall"] = FetchModel("RedBall");
            models["banana"] = FetchModel("banana");
        }

        void CollisionSystem_ProcessingStarted(object sender, EventArgs e)
        {
            if (initialize)
            {
                player = FetchModel(this.entityWorld.GetEntityByTag("PLAYER").GetComponent<SpatialFormComponent>().SpatialFormFile);
                playertranform = this.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>().TransformMatrix;

                for (int i = 1; i < 97; i++)
                {
                    obstaclesTransform.Add(this.entityWorld.GetEntityByTag("obstacle" + i.ToString()).GetComponent<TransformComponent>());
                }

                initialize = false;
            }
        }

        protected override void Process(Entity entity)
        {
            Model obstacles = null;

            foreach (var par in models)
            {
                obstacles = models[par.Key];

                for (int i = 0; i < player.Meshes.Count; i++)
                {
                    BoundingSphere firstSphere = player.Meshes[i].BoundingSphere;
                    playertranform = this.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>().TransformMatrix;
                    firstSphere = firstSphere.Transform(playertranform);

                    for (int k = 0; k < MAXOBSTACLES; k++)
                    {
                        for (int j = 0; j < obstacles.Meshes.Count; j++)
                        {
                            //timeSinceStart += entityWorld.DeltaTime;
                            BoundingSphere secondSphere = obstacles.Meshes[j].BoundingSphere;
                            secondSphere = secondSphere.Transform(obstaclesTransform[k].TransformMatrix);

                            if (firstSphere.Intersects(secondSphere))
                            {
                                obstaclesTransform[k].Position = new Vector3(-50, 0, 0);
                                // do something.
                            }
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
