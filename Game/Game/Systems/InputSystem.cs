using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ECS;
using Game;
using Game.Components;
using Game.StateManagement.ScreenManager;

namespace Game.Systems
{
    public class InputEventArgs : EventArgs
    {
        /// <summary>
        /// Modifier value.
        /// 
        /// Example usage would be for analog joystick movement, to represent
        /// how much the stick is pushed on a 0f..1f scale.
        /// </summary>
        public float modifier;

        /// <summary>
        /// A read only reference to the input system which triggered the event.
        /// </summary>
        public InputState input;
    }


    /// <summary>
    /// Provides a mapping of input sources to intents, such as jump or move left.
    /// </summary>
    public class InputSystem : EntitySystem
    {
        /// <summary>
        /// The active input class.
        /// </summary>
        private InputState input;

        /// <summary>
        /// A list of all the input mapping functions.
        /// </summary>
        private List<Action<PlayerIndex, GamePadThumbSticks, GamePadDPad>> inputMappings;

        public event EventHandler<InputEventArgs> JumpIntent;
        public event EventHandler<InputEventArgs> MoveLeftIntent;
        public event EventHandler<InputEventArgs> MoveRightIntent;
        // TODO: Implement Move{Up,Down}Intent
        // TODO: Implement StopMove*Intent

        public InputSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(InputComponent) }, GameLoopType.Update)
        {
            // Add ourselves to the blackboard.
            BlackBoard.SetEntry("InputSystem", this);

            // Grab the input from the blackboard.
            input = BlackBoard.GetEntry<InputState>("Input");

            // Hook into the ProcessEntities call.
            ProcessingStarted += new EventHandler(InputSystem_ProcessingStarted);

            // Register the mappings
            inputMappings = new List<Action<PlayerIndex, GamePadThumbSticks, GamePadDPad>>
            {
                MapJump, MapLeft, MapRight
            };
        }

        protected override void Process(Entity entity)
        {
            // Do nothing for now.
        }

        /// <summary>
        /// Acts as an 'update' method which is called once per frame by
        /// hooking into the ProcessEntities call.
        /// </summary>
        void InputSystem_ProcessingStarted(object sender, EventArgs e)
        {
            var pIndex = PlayerIndex.One;
            var sticks = input.CurrentGamePadStates[(int)pIndex].ThumbSticks;
            var dPad   = input.CurrentGamePadStates[(int)pIndex].DPad;

            // Normalize inputs
            sticks.Left.Normalize();
            sticks.Right.Normalize();

            // Process all the mappings.
            foreach (var map in inputMappings)
            {
                if (map != null)
                {
                    map(pIndex, sticks, dPad);
                }
            }
        }

        /// <summary>
        /// Map the inputs to a jump intent.
        /// </summary>
        /// <param name="pIndex">The PlayerIndex used to query the input class.</param>
        /// <param name="sticks">The normalized thumb stick vectors.</param>
        /// <param name="dPad">The dpad.</param>
        private void MapJump(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (JumpIntent != null && (input.IsNewButtonPress(Buttons.A, pIndex, out pIndex) ||
                                       input.IsNewKeyPress(Keys.Space, pIndex, out pIndex)))
            {
                JumpIntent(this, new InputEventArgs { input = this.input });
            }
        }

        /// <summary>
        /// Map the inputs to a move left intent.
        /// </summary>
        /// <param name="pIndex">The PlayerIndex used to query the input class.</param>
        /// <param name="sticks">The normalized thumb stick vectors.</param>
        /// <param name="dPad">The dpad.</param>
        private void MapLeft(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (MoveLeftIntent != null && (input.IsNewKeyPress(Keys.Left, pIndex, out pIndex) ||
                                           dPad.Left == ButtonState.Pressed))
            {
                // Keyboard or DPad
                MoveLeftIntent(this, new InputEventArgs
                {
                    input = this.input,
                    modifier = 1f
                });
            }
            else if (MoveLeftIntent != null && sticks.Left.X < 0)
            {
                // Left stick
                MoveLeftIntent(this, new InputEventArgs
                {
                    input = this.input,
                    modifier = sticks.Left.X
                });
            }
        }

        /// <summary>
        /// Map the inputs to a move right intent.
        /// </summary>
        /// <param name="pIndex">The PlayerIndex used to query the input class.</param>
        /// <param name="sticks">The normalized thumb stick vectors.</param>
        /// <param name="dPad">The dpad.</param>
        private void MapRight(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (MoveRightIntent != null && (input.IsNewKeyPress(Keys.Right, pIndex, out pIndex) ||
                                           dPad.Right == ButtonState.Pressed))
            {
                // Keyboard or DPad
                MoveRightIntent(this, new InputEventArgs
                {
                    input = this.input,
                    modifier = 1f
                });
            }
            else if (MoveRightIntent != null && sticks.Left.X > 0)
            {
                // Left stick
                MoveRightIntent(this, new InputEventArgs
                {
                    input = this.input,
                    modifier = sticks.Left.X
                });
            }
        }
    }
}
