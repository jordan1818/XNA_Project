using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ECS;
using Game.Components;

namespace Game.Systems
{
    /// <summary>
    ///  Renders any entities with a transform and spatial form component.
    /// </summary>
    public sealed class RenderSystem : EntitySystem
    {
        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;
        
        private Matrix projectionMatrix, viewMatrix;

        private Dictionary<string, Model> models;

        public RenderSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(TransformComponent), typeof(SpatialFormComponent) }, GameLoopType.Draw )
        {
            // Load entries from the blackboard.
            contentManager   = BlackBoard.GetEntry<ContentManager>("ContentManager");
            graphicsDevice   = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
            projectionMatrix = BlackBoard.GetEntry<Matrix>("ProjectionMatrix");
            viewMatrix       = BlackBoard.GetEntry<Matrix>("ViewMatrix");

            models = new Dictionary<string, Model>();

            ProcessingStarted += (s, e) =>
                {
                    // Set states ready for 3D  
                    graphicsDevice.BlendState = BlendState.Opaque;
                    graphicsDevice.DepthStencilState = DepthStencilState.Default;
                    // Something resets sampler 0 so this has to be set each frame  
                    graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

                    viewMatrix = BlackBoard.GetEntry<Matrix>("ViewMatrix");
                };
        }

        protected override void Process(Entity entity)
        {
            var spatialFile = entity.GetComponent<SpatialFormComponent>().SpatialFormFile;
            var model       = FetchModel(spatialFile);
            var modelMatrix = entity.GetComponent<TransformComponent>().TransformMatrix;

            // Copy any parent transforms.
            var parentTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(parentTransforms);

            // A model can have multiple meshes, so loop.
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World      = parentTransforms[mesh.ParentBone.Index] * modelMatrix;
                    effect.View       = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
        }

        /// <summary>
        ///  FetchModel provides lazy loading of models.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Model FetchModel(string key)
        {
            Model m;
            if (!models.TryGetValue(key, out m))
            {
                m = contentManager.Load<Model>(key);
                models[key] = m;
            }
            return m;
        }
    }
}
