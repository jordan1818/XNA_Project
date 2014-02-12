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
using Game.Template;

namespace Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Matrix viewMatrix, projectionMatrix;

        private EntityWorld entityWorld;

        private Entity player;

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

        /// <summary>
        /// Calculate the view and projection matrices.
        /// </summary>
        private void InitMatrices()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, GraphicsDevice.Viewport.AspectRatio, 1.0f, 1000f);

            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up);
        }

        /// <summary>
        /// Create the entity world and register systems.
        /// </summary>
        private void InitEntityWorld()
        {
            entityWorld = new EntityWorld();

            // Register the systems.
            entityWorld.RegisterSystem<MovementSystem>();
            entityWorld.RegisterSystem<RenderSystem>();
        }

        /// <summary>
        /// Register objects needed across multiple systems.
        /// </summary>
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

            player = entityWorld.CreateFromTemplate<PlayerTemplate>();
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            entityWorld.Draw();
            spriteBatch.End();
        }
    }
}
