﻿using System;
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

        public GameplayScreen()
        {
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                var quit = BlackBoard.GetEntry<Action>("QuitFunc");
                quit();
                
                // TODO: This should open a menu or title screen.
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
            spriteBatch    = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");

            InitMatrices();
            InitEntityWorld();
            CreateBackground();

            entityWorld.CreateFromTemplate<PlayerTemplate>();

            // Add temp debug controls.
            var inputSystem = BlackBoard.GetEntry<InputSystem>("InputSystem");

            inputSystem.MoveIntent += (s, e) =>
                {
                    var transform = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>();
                    transform.Position += new Vector3(e.Direction.X, 0f, -e.Direction.Y);
                };

            inputSystem.JumpIntent += (s, e) =>
                {
                    var jump = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<JumpComponent>();
                    jump.WantToJump = true;
                };
        }

        private void CreateBackground()
        {
            var background = entityWorld.CreateEntity();
            background.AddComponent(new SpatialFormComponent("hospital"));
            background.AddComponent(new TransformComponent());
  
            var transform = background.GetComponent<TransformComponent>();
            transform.Position = new Vector3(0, 0, -25.0f);
            transform.Scale = new Vector3(0.15f, 0.15f, 0.15f);
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
            entityWorld.RegisterSystem<InputSystem>();
            entityWorld.RegisterSystem<WayPointSystem>();
            entityWorld.RegisterSystem<JumpSystem>();
            entityWorld.RegisterSystem<MovementSystem>();
            entityWorld.RegisterSystem<GravitySystem>();
            entityWorld.RegisterSystem<CameraSystem>();
            entityWorld.RegisterSystem<RenderSystem>();
            entityWorld.RegisterSystem<TextSystem>();
        }
    }
}
