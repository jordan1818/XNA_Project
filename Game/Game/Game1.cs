using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ECS;
using Game.Systems;
using Game.Components;

namespace Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Matrix viewMatrix, projectionMatrix;

        private EntityWorld entityWorld;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InitMatrices();
            InitBlackBoard();
            InitEntityWorld();

            base.Initialize();  // Must be called last.
        }

        private void InitMatrices()
        {
            projectionMatrix = Matrix.CreatePerspective(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 0.01f, 1000f);

            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up);
        }

        private void InitEntityWorld()
        {
            entityWorld = new EntityWorld();

            // Register the systems.
            entityWorld.RegisterSystem<MovementSystem>();
            entityWorld.RegisterSystem<RenderSystem>();
        }

        private void InitBlackBoard()
        {
            BlackBoard.SetEntry("ContentManager"  , Content);
            BlackBoard.SetEntry("GraphicsDevice"  , GraphicsDevice);
            BlackBoard.SetEntry("SpriteBatch"     , spriteBatch);
            BlackBoard.SetEntry("ProjectionMatrix", projectionMatrix);
            BlackBoard.SetEntry("ViewMatrix"      , viewMatrix);
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
                GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Back))
            {
                Exit();
            }
            entityWorld.Update(gameTime.ElapsedGameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            entityWorld.Draw();
            spriteBatch.End();
        }
    }
}
