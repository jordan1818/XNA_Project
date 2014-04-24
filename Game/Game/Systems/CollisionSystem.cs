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

        // All models/transform variables.  
        Model player;
        TransformComponent playertranform;
        private Dictionary<int, Model> models;
        private List<TransformComponent> obstaclesTransform;

        bool initialize = true;
        private int MAXOBSTACLES = 40;
        bool collide = false;

        public CollisionSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(SpatialFormComponent), typeof(TransformComponent) }, GameLoopType.Update)
        {
            Content = BlackBoard.GetEntry<ContentManager>("ContentManager");
            this.entityWorld = entityWorld;

            // Sets up variables.
            models = new Dictionary<int, Model>();
            obstaclesTransform = new List<TransformComponent>();
            BlackBoard.SetEntry("collide", collide);

            ProcessingStarted += new EventHandler(CollisionSystem_ProcessingStarted);

            models[0] = FetchModel("table");
            models[1] = FetchModel("flask");
            models[2] = FetchModel("RedBall");
            models[3] = FetchModel("banana");
        }

        void CollisionSystem_ProcessingStarted(object sender, EventArgs e)
        {
            // Allows to set the variables once.
            if (initialize)
            {
                player = FetchModel(this.entityWorld.GetEntityByTag("PLAYER").GetComponent<SpatialFormComponent>().SpatialFormFile);

                for (int i = 1; i < MAXOBSTACLES + 1; i++)
                {
                    obstaclesTransform.Add(this.entityWorld.GetEntityByTag("obstacle" + i.ToString()).GetComponent<TransformComponent>());
                }

                initialize = false;
            }

            playertranform = this.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>();
        }

        public bool Collision(int Index)
        {
            Model obstacles = null;

            // Checks a specific spot to check for collision.
            if ((playertranform.Position.X + 10) >= obstaclesTransform[Index].Position.X && 
                obstaclesTransform[Index].Position.X <= (obstaclesTransform[Index].Position.X + 5))
            {
                for (var par = 0; par < models.Count; par++)
                {
                    obstacles = models[par];

                    for (int i = 0; i < player.Meshes.Count; i++)
                    {
                        BoundingSphere firstSphere = player.Meshes[i].BoundingSphere;
                        firstSphere = firstSphere.Transform(playertranform.TransformMatrix);

                        for (int j = 0; j < obstacles.Meshes.Count; j++)
                        {
                            BoundingSphere secondSphere = obstacles.Meshes[j].BoundingSphere;
                            secondSphere = secondSphere.Transform(obstaclesTransform[Index].TransformMatrix);

                            if (firstSphere.Intersects(secondSphere))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        protected override void Process(Entity entity)
        {
            // Checks all obstacles in the world.
            for (int x = 0; x < MAXOBSTACLES; x++)
            {
                // Checks collision.
                if (Collision(x))
                {
                    // Sets a flag for collision.
                    collide = true;
                    BlackBoard.SetEntry("collide", collide);
                    obstaclesTransform[x].Position = new Vector3(-50, 0, 0);
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
