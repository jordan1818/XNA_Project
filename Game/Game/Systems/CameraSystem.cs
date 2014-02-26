using System;
using System.Collections.Generic;
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.Components;

namespace Game.Systems
{
    class CameraSystem : EntitySystem
    {
        Matrix viewMatrix;
        EntityWorld entityWorld;

        public CameraSystem(EntityWorld entityWorld)
            : base(entityWorld, new Type[] { typeof(InputComponent) }, GameLoopType.Update)
        {
            this.entityWorld = entityWorld;
            viewMatrix = BlackBoard.GetEntry<Matrix>("ViewMatrix");

            ProcessingStarted += new EventHandler(CameraSystem_ProcessingStarted);
        }

        protected override void Process(Entity entity)
        {
            // Do nothing for now.
        }

        void CameraSystem_ProcessingStarted(object sender, EventArgs e)
        {
            var player = entityWorld.GetEntityByTag("PLAYER");
            var pos = player.GetComponent<TransformComponent>().Position;
             
            viewMatrix = Matrix.CreateLookAt(new Vector3(pos.X, 45f, 0), pos, Vector3.Up);
            BlackBoard.SetEntry("ViewMatrix", viewMatrix);
        }
    }
}
