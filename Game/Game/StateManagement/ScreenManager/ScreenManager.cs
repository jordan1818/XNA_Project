using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ECS;

namespace Game.StateManagement.ScreenManager
{
    /// <summary>
    /// The screen manager manages one or more GameScreen instances.
    /// It maintains a stack of screens, calls their Update and Draw methods
    /// and automatically routes input to the topmost active screen.
    /// </summary>
    public class ScreenManager
    {
        Microsoft.Xna.Framework.Game game;

        List<GameScreen> screens;
        List<GameScreen> screensToUpdate;

        SpriteBatch spriteBatch;
        ContentManager content;
        GraphicsDevice graphicsDevice;

        Texture2D blankTexture;
        SpriteFont font;

        /// <summary>
        /// If true, the manager prints out a list of all the screens each
        /// time it is update. This can be useful for making sure everything
        /// is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        public ScreenManager(Microsoft.Xna.Framework.Game game)
        {
            this.game       = game;
            screens         = new List<GameScreen>();
            screensToUpdate = new List<GameScreen>();
            spriteBatch     = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            content         = BlackBoard.GetEntry<ContentManager>("ContentManager");
            graphicsDevice  = BlackBoard.GetEntry<GraphicsDevice>("GraphicsDevice");
        }

        void LoadContent()
        {
            font         = content.Load<SpriteFont>("menufont");
            blankTexture = content.Load<Texture2D>("blank");

            foreach (var scr in screens)
            {
                scr.LoadContent();
            }
        }

        void UnloadContent()
        {
            foreach (var scr in screens)
            {
                scr.UnloadContent();
            }
        }

        void Update(GameTime gameTime)
        {
            // TODO: Get input.

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating on screen adds or removes others.
            screensToUpdate.Clear();

            foreach (var scr in screens)
            {
                screensToUpdate.Add(scr);
            }

            bool otherScreenHasFocus = !game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var scr = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                scr.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (scr.ScreenState == ScreenState.TransitionOn ||
                    scr.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a change to handle input.
                    if (!otherScreenHasFocus)
                    {
                        scr.HandleInput(null); // TODO: pass input state.
                        otherScreenHasFocus = false;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!scr.IsPopup)
                    {
                        coveredByOtherScreen = true;
                    }
                }
            }

            if (TraceEnabled)
            {
                TraceScreens();
            }
        }

        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            var scrNames = new List<string>();

            foreach (var scr in screens)
            {
                scrNames.Add(scr.GetType().Name);
            }

            Debug.WriteLine(string.Join(", ", scrNames.ToArray()));
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        /// <param name="gameTime">The XNA GameTime from Update.</param>
        public void Draw(GameTime gameTime)
        {
            foreach (var scr in screens)
            {
                if (scr.ScreenState == ScreenState.Hidden)
                    continue;

                scr.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        /// <param name="screen">The screen to add.</param>
        /// <param name="controllingPlayer">The controlling player, if there is one.</param>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            screen.LoadContent();

            screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            screen.UnloadContent();

            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }

        /// <summary>
        /// Helper to draw a translucent black fullscreen sprite, used for
        /// fading screens in and out, and for darkening the background
        /// behind popups.
        /// </summary>
        /// <param name="alpha">The alpha value.</param>
        public void FadeBackBufferToBlack(float alpha)
        {
            var viewport = graphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.Draw(blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);
            spriteBatch.End();
        }
    }
}
