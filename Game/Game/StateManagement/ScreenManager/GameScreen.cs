using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Game.StateManagement.ScreenManager
{
    /// <summary>
    /// Describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    /// <summary>
    /// A screen is a single layer that has update and draw logic, and which
    /// can be combined with other layers to build up a complex menu system.
    /// For instance the main menu, the options menu, the "are you sure you
    /// want to quit" message box, and the main game itself are all implemented
    /// as screens.
    /// </summary>
    public abstract class GameScreen
    {
        #region Properties

        /// <summary>
        /// If true, screens beneath this one do not need to transition off.
        /// </summary>
        public bool IsPopup { get; set; }

        /// <summary>
        /// Indicates how long the screen takes to transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime { get; set; }

        /// <summary>
        /// Indicates how long the screen takes to transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime { get; set; }

        /// <summary>
        /// Gets the current position of the screen transition, ranging from
        /// zero (fully active, no transition) to one (transitioned fully off to nothing).
        /// </summary>
        public float TransitionPosition { get; protected set; }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging from
        /// 1 (fully active, no transition) to 0 (transitioned fully off to nothing). 
        /// </summary>
        public float TransitionAlpha
        {
            get { return 1f - TransitionPosition; }
        }

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState { get; protected set; }

        /// <summary>
        /// True if the screen is exiting for good and not just going
        /// away to make room for another screen on top of it.
        /// </summary>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                    (this.ScreenState == ScreenState.TransitionOn || this.ScreenState == ScreenState.Active);
            }
        }

        private bool otherScreenHasFocus;

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the index of the player who is currently controlling this screen,
        /// or null if it is accepting input from any player. This is used to lock
        /// the game to a specific player profile. The main menu responds to input
        /// from any connected gamepad, but whichever player makes a selection from
        /// this menu is given control over all subsequent screens, so other gamepads
        /// are inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer { get; internal set; }

        #endregion

        public GameScreen()
        {
            IsPopup            = false;
            TransitionOnTime   = TimeSpan.Zero;
            TransitionOffTime  = TimeSpan.Zero;
            TransitionPosition = 1f;
            this.ScreenState   = ScreenState.TransitionOn;
            IsExiting          = false;
        }

        #region Initialization

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() { }

        #endregion

        #region Update and Draw

        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                this.ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // Still busy transitioning.
                    this.ScreenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished.
                    this.ScreenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, TransitionOnTime, -1))
                {
                    // Still busy transitioning.
                    this.ScreenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished.
                    this.ScreenState = ScreenState.Active;
                }
            }
        }

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        /// <param name="gameTime">The XNA GameTime from Update.</param>
        /// <param name="time">The transition time.</param>
        /// <param name="direction">The transition direction.</param>
        /// <returns></returns>
        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
        
            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (TransitionPosition <= 0)) ||
                ((direction > 0) && (TransitionPosition >= 1)))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }
            
            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        /// <param name="input">The input state.</param>
        public virtual void HandleInput(InputState input) { }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        /// <param name="gameTime">The XNA GameTime from Update.</param>
        public virtual void Draw(GameTime gameTime) { }


        #endregion

        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                IsExiting = true;
            }
        }

    }
}
