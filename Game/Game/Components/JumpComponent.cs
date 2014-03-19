using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;

namespace Game.Components
{
    class JumpComponent : IComponent
    {
        public int MAXJUMPS = 2;
        public int JumpCount { set; get; }
        public bool IsJumping
        {
            get
            {
                return JumpCount < MAXJUMPS;
            }
        }

        public bool WantToJump { set; get; }
    }
}
