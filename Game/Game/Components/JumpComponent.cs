using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;

namespace Game.Components
{
    class JumpComponent : IComponent
    {
        public const int MaxJumps = 1;

        public const float MaxYVel = 0.35f;

        public int JumpCount { set; get; }
        public bool WantToJump { set; get; }

        public bool IsJumping
        {
            get
            {
                return JumpCount > 0;
            }
        }
    }
}
