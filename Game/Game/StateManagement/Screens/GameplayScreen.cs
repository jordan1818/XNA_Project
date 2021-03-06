﻿/********************************************************************************************************
 * Lab Runner created by Jonathan, Jordan, Zac
 * 
 * Our Main Game
 * 
 * Module Name: Lab Runner (XNA Project)
 * 
 * Inputs: Xbox control/keyboard (jump = a or spacebar), (Movement = left joystick or arrow keys)
 * 
 * Date: April 23, 2014.
 * 
 * 
 * ******************************************************************************************************/


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

        private bool done;

        public GameplayScreen()
        {
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
        }
    /// <summary>
    /// Update function of main game play screen
    /// </summary>
    /// <param name="gameTime">Actual time passed</param>
    /// <param name="otherScreenHasFocus"></param>
    /// <param name="coveredByOtherScreen"></param>
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
    /// <summary>
    /// Draw
    /// </summary>
    /// <param name="gameTime">Time since last frame</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            entityWorld.Draw();
        }
        /// <summary>
        /// Loads content of our screen
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            graphicsDevice = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
            spriteBatch    = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");

            done = false;
            BlackBoard.SetEntry("Done", done);

            InitMatrices();
            InitSounds();
            InitEntityWorld();
            CreateBackground();
            CreateObstacles();

            entityWorld.CreateFromTemplate<PlayerTemplate>();

            // Added controls.
            var inputSystem = BlackBoard.GetEntry<InputSystem>("InputSystem");

            // Sets up movement/jump actions when pressed.
            inputSystem.MoveIntent += (s, e) =>
                {
                    var transform = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<TransformComponent>();
                    if (transform.Position.Z <= -55)
                    {
                        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, -55);
                    }

                    if (transform.Position.Z >= 0)
                    {
                        transform.Position = new Vector3(transform.Position.X, transform.Position.Y, 0);
                    }

                    if (transform.Position.X >= 2450)
                    {
                        transform.Position = new Vector3(2450, transform.Position.Y, transform.Position.Z);
                        done = true;
                        BlackBoard.SetEntry("Done", done);
                    }

                    if (e.Direction.X < -0.1)
                    {
                        transform.Position += new Vector3(0, 0f, -e.Direction.Y);
                    }

                    else
                    {
                        transform.Position += new Vector3(e.Direction.X, 0f, -e.Direction.Y);

                    }
                };
            // jump system is called when the inputSystem gives the right return
            inputSystem.JumpIntent += (s, e) =>
                {
                    var jump = e.entityWorld.GetEntityByTag("PLAYER").GetComponent<JumpComponent>();
                    soundBank.PlayCue("Jump");
                    jump.WantToJump = true;
                };
        }
        /// <summary>
        /// initialized Sounds
        /// </summary>
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
        /// <summary>
        /// Load background models
        /// </summary>
        private void CreateBackground()
        {
            // Background.
            var background = entityWorld.CreateEntity();
            background.AddComponent(new SpatialFormComponent("hospital2.0"));
            background.AddComponent(new TransformComponent());
  
            var transform = background.GetComponent<TransformComponent>();
            transform.Position = new Vector3(-50.0f, 0, -25.0f);
            transform.Scale = new Vector3(0.15f, 0.15f, 0.15f);
        }
        /// <summary>
        /// Create all Obstacles
        /// </summary>
        private void CreateObstacles()
        {
            int MAXITEMS = 10;
            const int MAXDIST = 2450;
            int obstacleIndex = 0;
            Random random = new Random();

            for (int i = 0; i < MAXITEMS; i++)
            {
                Vector3 tempPos = new Vector3((float)random.Next(50, MAXDIST), 0.0f, (float)random.Next(-50, -10));

                obstacleIndex++;

                // Table obstacle.
                var table = entityWorld.CreateEntity();
                table.Tag = "obstacle" + obstacleIndex.ToString();
                table.AddComponent(new SpatialFormComponent("table"));
                table.AddComponent(new TransformComponent());

                var transformTable = table.GetComponent<TransformComponent>();
                transformTable.Position = tempPos;
                transformTable.Scale = new Vector3(0.055f, 0.055f, 0.075f);
            }

            for (int i = 0; i < MAXITEMS; i++)
            {
                Vector3 tempPos = new Vector3((float)random.Next(50, MAXDIST), 0.0f, (float)random.Next(-50, -10));

                obstacleIndex++;

                // Flask obstacle.
                var flask = entityWorld.CreateEntity();
                flask.Tag = "obstacle" + obstacleIndex.ToString();
                flask.AddComponent(new SpatialFormComponent("flask"));
                flask.AddComponent(new TransformComponent());

                var transformFlask = flask.GetComponent<TransformComponent>();
                transformFlask.Position = tempPos;
                transformFlask.Scale = new Vector3(0.03f, 0.03f, 0.03f);
            }

            for (int i = 0; i < MAXITEMS; i++)
            {
                Vector3 tempPos = new Vector3((float)random.Next(50, MAXDIST), 0.0f, (float)random.Next(-50, -10));

                obstacleIndex++;

                // Red ball obstacle.
                var ball = entityWorld.CreateEntity();
                ball.Tag = "obstacle" + obstacleIndex.ToString();
                ball.AddComponent(new SpatialFormComponent("RedBall"));
                ball.AddComponent(new TransformComponent());

                var transformBall = ball.GetComponent<TransformComponent>();
                transformBall.Position = tempPos;
                transformBall.Scale = new Vector3(0.05f, 0.05f, 0.05f);
            }

            for (int i = 0; i < MAXITEMS; i++)
            {
                Vector3 tempPos = new Vector3((float)random.Next(50, MAXDIST), 0.0f, (float)random.Next(-50, -10));

                obstacleIndex++;

                // Banana obstacle.
                var banana = entityWorld.CreateEntity();
                banana.Tag = "obstacle" + obstacleIndex.ToString();
                banana.AddComponent(new SpatialFormComponent("banana"));
                banana.AddComponent(new TransformComponent());

                var transformBanana = banana.GetComponent<TransformComponent>();
                transformBanana.Position = tempPos;
                transformBanana.Scale = new Vector3(0.03f, 0.03f, 0.03f);
            }

            BlackBoard.SetEntry("AmountObstacle", obstacleIndex);
        }
        /// <summary>
        /// Unload Content 
        /// </summary>
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        /// <summary>
        /// initialized Matrices
        /// </summary>
        private void InitMatrices()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, graphicsDevice.Viewport.AspectRatio, 1.0f, 1000.0f);
            viewMatrix = Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up);

            // Add to blackboard.
            BlackBoard.SetEntry("ProjectionMatrix", projectionMatrix);
            BlackBoard.SetEntry("ViewMatrix", viewMatrix);
        }
        /// <summary>
        /// initialized The ECS Systems
        /// </summary>
        private void InitEntityWorld()
        {
            entityWorld = new EntityWorld();

            // Register the systems.
            entityWorld.RegisterSystem<InputSystem>();
            entityWorld.RegisterSystem<WayPointSystem>();
            entityWorld.RegisterSystem<JumpSystem>();
            entityWorld.RegisterSystem<CollisionSystem>();
            entityWorld.RegisterSystem<MovementSystem>();
            entityWorld.RegisterSystem<GravitySystem>();
            entityWorld.RegisterSystem<CameraSystem>();
            entityWorld.RegisterSystem<RenderSystem>();
            entityWorld.RegisterSystem<TextSystem>();
        }
    }
}
