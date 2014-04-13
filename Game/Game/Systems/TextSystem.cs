using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ECS;
using Game.Components;

namespace Game.Systems
{
    class TextSystem : EntitySystem
    {
        private EntityWorld entityWorld;
        private TimeSpan timeSinceStart;

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public TextSystem(EntityWorld entityWorld) :
            base(entityWorld, new Type[] { typeof(TransformComponent) }, GameLoopType.Draw)
        {
            this.entityWorld = entityWorld;

            spriteBatch = BlackBoard.GetEntry<SpriteBatch>("SpriteBatch");
            spriteFont = BlackBoard.GetEntry<SpriteFont>("SpriteFont");

            ProcessingStarted += new EventHandler(TextSystem_ProcessingStarted);
        }

        void TextSystem_ProcessingStarted(object sender, EventArgs e)
        {
            // Keeps track of time that has past in game.
            timeSinceStart += entityWorld.DeltaTime;

            spriteBatch.Begin();
            // Displays the time it will take to run through the game.
            spriteBatch.DrawString(spriteFont, "Game Time " + timeSinceStart.TotalSeconds.ToString("0.00"), new Vector2(0, 0), Color.DarkGray,
                                    0.0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            /*
            if (collide)
            {
                timeSinceStart += TimeSpan.FromSeconds(10);
            }
            */

            spriteBatch.End();  
        }

        protected override void Process(Entity entity)
        {
            // Does nothing..
        }
    }
}
