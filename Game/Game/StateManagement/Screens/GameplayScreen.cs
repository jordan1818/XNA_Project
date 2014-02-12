using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using ECS;
using Game.Components;
using Game.Systems;
using Game.Template;
using Game.StateManagement.ScreenManager;


namespace Game.StateManagement.Screens
{
    public class GameplayScreen : GameScreen
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        private Matrix viewMatrix, projectionMatrix;

        private EntityWorld entityWorld;

        private Entity player;


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                Environment.Exit(0);
            }

            // Debug controls. Input will be moved into the ECS in the future.
            var transform = player.GetComponent<TransformComponent>();
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                transform.Position += new Vector3(0, 0, -1);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                transform.Position += new Vector3(0, 0, 1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                var rot = Quaternion.CreateFromYawPitchRoll(0, 0, 0.25f);
                transform.Rotation *= rot;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                var rot = Quaternion.CreateFromYawPitchRoll(0, 0, -0.25f);
                transform.Rotation *= rot;
            }

            entityWorld.Update(gameTime.ElapsedGameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            entityWorld.Draw();
        }

        public override void LoadContent()
        {
            base.LoadContent();

            graphicsDevice = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");

            InitMatrices();
            InitEntityWorld();

            player = entityWorld.CreateFromTemplate<PlayerTemplate>();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        private void InitMatrices()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, graphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up);

            // Add to blackboard.
            BlackBoard.SetEntry("ProjectionMatrix", projectionMatrix);
            BlackBoard.SetEntry("ViewMatrix", viewMatrix);
        }

        private void InitEntityWorld()
        {
            entityWorld = new EntityWorld();

            // Register the systems.
            entityWorld.RegisterSystem<MovementSystem>();
            entityWorld.RegisterSystem<RenderSystem>();
        }
    }
}
