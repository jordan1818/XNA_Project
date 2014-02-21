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
    using MappingFunc = Action<PlayerIndex, GamePadThumbSticks, GamePadDPad>;
    using Intent = EventHandler<InputEventArgs>;

    public class InputEventArgs : EventArgs
    {
        /// <summary>
        /// The InputState which triggered the event.
        /// </summary>
        public InputState Input { get; set; }

        /// <summary>
        /// The EntityWorld the InputSystem belongs to.
        /// </summary>
        public EntityWorld entityWorld { get; set; }

        private Vector2 direction;

        /// <summary>
        /// The normalized input direction for move events.
        /// </summary>
        public Vector2 Direction
        {
            set 
            { 
                direction = value; 
            }

            get
            {
                direction.Normalize();
                return direction;
            }
        }
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
        private List<MappingFunc> inputMappings;

        public event Intent JumpIntent;
        public event Intent MoveIntent;

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
            inputMappings = new List<MappingFunc>
            {
                MapArrows, MapDpad, MapLeftStick, MapJump
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
            foreach (var mapping in inputMappings)
            {
                mapping(pIndex, sticks, dPad);
            }
        }

        #region Mappings

        private void MapJump(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (JumpIntent != null && (input.IsNewButtonPress(Buttons.A, pIndex, out pIndex) ||
                                       input.IsNewKeyPress(Keys.Space, pIndex, out pIndex)))
            {
                JumpIntent(this, new InputEventArgs { Input = this.input, entityWorld = entityWorld });
            }
        }
  
        private void MapDpad(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (MoveIntent == null)
                return;

            // TODO: Check for new dPad presses only.

            Vector2 direction = new Vector2();

            // Check X Axis
            if (dPad.Left == ButtonState.Pressed)
                direction.X = -1;
            else if (dPad.Right == ButtonState.Pressed)
                direction.X = 1;

            // Check Y Axis
            if (dPad.Up == ButtonState.Pressed)
                direction.Y = 1;
            else if (dPad.Down == ButtonState.Pressed)
                direction.Y = -1;


            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                MoveIntent(this, new InputEventArgs { Direction = direction, Input = input, entityWorld = entityWorld });
            }
        }

        private void MapArrows(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (MoveIntent == null)
                return;

            Vector2 direction = new Vector2();

            // Check X Axis
            if (input.IsNewKeyPress(Keys.Left, pIndex, out pIndex))
                direction.X = -1;
            else if (input.IsNewKeyPress(Keys.Right, pIndex, out pIndex))
                direction.X = 1;
            
            // Check Y Axis
            if (input.IsNewKeyPress(Keys.Up, pIndex, out pIndex))
                direction.Y = 1;
            else if (input.IsNewKeyPress(Keys.Down, pIndex, out pIndex))
                direction.Y = -1;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                MoveIntent(this, new InputEventArgs { Direction = direction, Input = input, entityWorld = entityWorld });
            }
        }

        private void MapLeftStick(PlayerIndex pIndex, GamePadThumbSticks sticks, GamePadDPad dPad)
        {
            if (MoveIntent != null && sticks.Left != Vector2.Zero)
            {
                MoveIntent(this, new InputEventArgs { Direction = sticks.Left, Input = input, entityWorld = entityWorld });
            }
        }

        #endregion
    }
}
