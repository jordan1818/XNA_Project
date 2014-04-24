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
        private bool collide = false;
        private bool done = false;
        private TimeSpan finalTime;
        bool snapShot = true;

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

            done = BlackBoard.GetEntry<bool>("Done");

            spriteBatch.Begin();
            if (done)
            {
                if (snapShot)
                {
                    finalTime = timeSinceStart;
                    snapShot = false;
                }

                spriteBatch.DrawString(spriteFont, "Your time was " + finalTime.TotalSeconds.ToString("0") + " Seconds!", new Vector2(150, 100), Color.Blue,
                                    0.0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
                spriteBatch.DrawString(spriteFont, "Press back to exit!", new Vector2(125, 150), Color.Crimson,
                                    0.0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            }
            else if (!done)
            {
                // Displays the time it will take to run through the game.
                spriteBatch.DrawString(spriteFont, "Time: " + timeSinceStart.TotalSeconds.ToString("0"), new Vector2(325, 0), Color.Black,
                                        0.0f, new Vector2(0, 0), 0.75f, SpriteEffects.None, 0);
            }

            // Gets collisions for all obstacles.
            collide = BlackBoard.GetEntry<bool>("collide");
            if (collide)
            {
                // Adds time for colliding with object.
                timeSinceStart += TimeSpan.FromSeconds(10);
                collide = false;
                BlackBoard.SetEntry("collide", collide);
            }

            spriteBatch.End();  
        }

        protected override void Process(Entity entity)
        {
            // Does nothing..
        }
    }
}
