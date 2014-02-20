using System;
using System.Collections.Generic;
using ECS;
using Game.StateManagement.ScreenManager;

namespace Game.Components
{
    /// <summary>
    /// The possible intents for a player.
    /// </summary>
    public enum Intent
    {
        Idle,
        Jump,
        WalkLeft,
        WalkRight
    }

    /// <summary>
    /// Used to mark the player as a controllable entity.
    /// </summary>
    public class InputComponent : IComponent
    {
        public Intent currentIntent = Intent.Idle;
    }
}
