using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        private AudioEngine gameSoundEngine;
        private SoundBank soundBank;
        private WaveBank waveBank;

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
            InitSounds();
            InitEntityWorld();
            CreateBackground();
            CreateObstacles();

            entityWorld.CreateFromTemplate<PlayerTemplate>();

            // Add temp debug controls.
            var inputSystem = BlackBoard.GetEntry<InputSystem>("InputSystem");

            // Sets up movement/jump actions when pressed.
            inputSystem.MoveIntent += (s, e) =>
                {
                    var transform = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>();
                    transform.Position += new Vector3(e.Direction.X, 0f, -e.Direction.Y);
                };

            inputSystem.JumpIntent += (s, e) =>
                {
                    var jump = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<JumpComponent>();
                    soundBank.PlayCue("Jump");
                    jump.WantToJump = true;
                };
        }

        private void InitSounds()
        {
            #if XBOX
                gameSoundEngine = new AudioEngine(@"Content\Sounds\Xbox\GameSounds.xgs");
                waveBank = new WaveBank(gameSoundEngine, @"Content\Sounds\Xbox\Wave Bank.xwb");
                soundBank = new SoundBank(gameSoundEngine, @"Content\Sounds\Xbox\Sound Bank.xsb");
            #elif WINDOWS
                gameSoundEngine = new AudioEngine(@"Content\Sounds\Win\GameSounds.xgs");
                waveBank = new WaveBank(gameSoundEngine, @"Content\Sounds\Win\Wave Bank.xwb");
                soundBank = new SoundBank(gameSoundEngine, @"Content\Sounds\Win\Sound Bank.xsb");
            #endif

                soundBank.PlayCue("Horror Music");
        }

        private void CreateBackground()
        {
            // Background.
            var background = entityWorld.CreateEntity();
            background.AddComponent(new SpatialFormComponent("hospital"));
            background.AddComponent(new TransformComponent());
  
            var transform = background.GetComponent<TransformComponent>();
            transform.Position = new Vector3(270.0f, 0, -25.0f);
            transform.Scale = new Vector3(0.15f, 0.15f, 0.15f);
        }

        private void CreateObstacles()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 6);

            int startPos = random.Next(1, 6);
            startPos = 2;

            // Table obstacle.
            var table = entityWorld.CreateEntity();
            table.AddComponent(new SpatialFormComponent("table"));
            table.AddComponent(new TransformComponent());

            var transformTable = table.GetComponent<TransformComponent>();
            //transformTable.Position = new Vector3(50.0f, 0.0f, -15.0f);
            transformTable.Scale = new Vector3(0.055f, 0.055f, 0.075f);

            // Flask obstacle.
            var flask = entityWorld.CreateEntity();
            flask.AddComponent(new SpatialFormComponent("flask"));
            flask.AddComponent(new TransformComponent());

            var transformFlask = flask.GetComponent<TransformComponent>();
            //transformFlask.Position = new Vector3(30.0f, 0.0f, -1.0f);
            transformFlask.Scale = new Vector3(0.03f, 0.03f, 0.03f);

            // Red ball obstacle.
            var ball = entityWorld.CreateEntity();
            ball.AddComponent(new SpatialFormComponent("RedBall"));
            ball.AddComponent(new TransformComponent());

            var transformBall = ball.GetComponent<TransformComponent>();
            transformBall.Position = new Vector3(85.0f, 0.0f, -45.0f);
            transformBall.Scale = new Vector3(0.05f, 0.05f, 0.05f);

            // Banana obstacle.
            var banana = entityWorld.CreateEntity();
            banana.AddComponent(new SpatialFormComponent("banana"));
            banana.AddComponent(new TransformComponent());

            var transformBanana = banana.GetComponent<TransformComponent>();
            transformBanana.Scale = new Vector3(0.03f, 0.03f, 0.03f);

            switch (startPos)
            {
                case 1:
                    transformTable.Position = new Vector3(50.0f, 0.0f, -15.0f);
                    transformFlask.Position = new Vector3(30.0f, 0.0f, -11.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);
                    break;
                case 2:
                    transformTable.Position = new Vector3(30.0f, 0.0f, -35.0f);
                    transformFlask.Position = new Vector3(300.0f, 0.0f, -30.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);
                    break;
                case 3:
                    transformTable.Position = new Vector3(50.0f, 0.0f, -25.0f);
                    transformFlask.Position = new Vector3(60.0f, 0.0f, -1.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);
                    break;
                case 4:
                    transformTable.Position = new Vector3(100.0f, 0.0f, -15.0f);
                    transformFlask.Position = new Vector3(100.0f, 0.0f, -1.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);

                    break;
                case 5:
                    transformTable.Position = new Vector3(0.0f, 0.0f, -15.0f);
                    transformFlask.Position = new Vector3(25.0f, 0.0f, -1.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);

                    break;
                case 6:
                    transformTable.Position = new Vector3(20.0f, 0.0f, -15.0f);
                    transformFlask.Position = new Vector3(60.0f, 0.0f, -10.0f);
                    transformBanana.Position = new Vector3(45.0f, 0.0f, -30.0f);

                    break;
            }
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
