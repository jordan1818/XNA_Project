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
using Game.StateManagement.ScreenManager;
using Game.StateManagement.Screens;

namespace Game
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private ScreenManager screenManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InitBlackBoard();
            screenManager = new ScreenManager(this);
            base.Initialize();  // Must be called last.
        }


        /// <summary>
        /// Register objects needed across multiple systems.
        /// </summary>
        private void InitBlackBoard()
        {
             // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            BlackBoard.SetEntry("ContentManager"  , Content);
            BlackBoard.SetEntry("GraphicsDevice"  , GraphicsDevice);
            BlackBoard.SetEntry("SpriteBatch"     , spriteBatch);
            BlackBoard.SetEntry("QuitFunc"        , (Action)this.Exit);
        }

        protected override void LoadContent()
        {
            screenManager.LoadContent();

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void UnloadContent()
        {
            screenManager.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            screenManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            screenManager.Draw(gameTime);
        }
    }
}
